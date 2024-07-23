using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.CustomerManagement.Contracts;
using AccurateAppend.Standardization;
using AccurateAppend.Websites.Clients.Areas.Public.LeadsApi.Models;
using AccurateAppend.ZenDesk.Contracts.Support;
using EventLogger;
using NServiceBus;
using Application = AccurateAppend.Core.Definitions.Application;

namespace AccurateAppend.Websites.Clients.Areas.Public.LeadsApi
{
    [AllowAnonymous()]
    public class LeadsApiController : Controller
    {
        #region Fields

        private readonly INameStandardizer parser;
        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        public LeadsApiController(INameStandardizer parser, IMessageSession bus)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            Contract.EndContractBlock();

            this.parser = parser;
            this.bus = bus;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Called from FormStack forms
        /// </summary>
        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> FormStack([ModelBinder(typeof(NameLookupBinder))] FormStackLeadViewModel formStackLeadViewModel, CancellationToken cancellation)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    var errorMessage = this.ModelState.First(m => m.Value.Errors.Any()).Value.Errors.First().ErrorMessage;
                    Logger.LogEvent("FormStack Web hooks validation error", Severity.High, Application.Clients, this.Request.UserHostAddress, errorMessage);
                    
                    return this.Json(
                        new
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            Message = errorMessage
                        }, JsonRequestBehavior.AllowGet);
                }

                var requestType = RequestType.Other;
                
                switch (formStackLeadViewModel.RequestType)
                {
                    case "Service":
                        requestType = RequestType.Service;
                        break;
                    case "Sales / Pricing":
                        requestType = RequestType.Sales;
                        break;
                    case "Other":
                        requestType = RequestType.Other;
                        break;
                }

                if (requestType == RequestType.Service)
                {
                    await this.CreateTicket(formStackLeadViewModel);
                }
                else
                {
                    await this.CreateLead(formStackLeadViewModel);
                }
            }
            catch (Exception regException)
            {
                Logger.LogEvent(regException, Severity.High, Application.Clients, this.Request.UserHostAddress, "FormStack Web hooks failing");
                return this.Json(
                    new
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        regException.Message
                    }, JsonRequestBehavior.AllowGet
                );
            }

            return this.Json(
                   new
                   {
                       StatusCode = HttpStatusCode.OK,
                       Message = String.Empty
                   }, JsonRequestBehavior.AllowGet
               );
        }

        /// <summary>
        /// Called from Google leads.
        /// </summary>
        [ValidateInput(false)]
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Google(GoogleLeadViewModel googleLeadViewModel, CancellationToken cancellation)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    var errorMessage = this.ModelState.First(m => m.Value.Errors.Any()).Value.Errors.First().ErrorMessage;
                    Logger.LogEvent("Google Web hooks validation error", Severity.High, Application.Clients, this.Request.UserHostAddress, errorMessage);

                    return this.Json(
                        new
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            Message = errorMessage
                        }, JsonRequestBehavior.AllowGet);
                }

                await this.CreateLead(googleLeadViewModel);
            }
            catch (Exception regException)
            {
                Logger.LogEvent(regException, Severity.High, Application.Clients, this.Request.UserHostAddress, "Google Web hooks failing");
                return this.Json(
                    new
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        regException.Message
                    }, JsonRequestBehavior.AllowGet
                );
            }

            return this.Json(
                   new
                   {
                       StatusCode = HttpStatusCode.OK,
                       Message = String.Empty
                   }, JsonRequestBehavior.AllowGet
               );
        }

        #endregion

        #region Helpers

        private RecordCount AttemptToParse(String recordCount)
        {
            if (String.Equals("Less than 4,000 records", recordCount, StringComparison.OrdinalIgnoreCase)) return RecordCount.LessThan4K;
            if (String.Equals("Less than 10,000 records", recordCount, StringComparison.OrdinalIgnoreCase)) return RecordCount.LessThan10K;
            if (String.Equals("More than 10,000 records", recordCount, StringComparison.OrdinalIgnoreCase)) return RecordCount.GreaterThan10K;

            return RecordCount.Unknown;
        }

        private String ParseName(String name)
        {
            var firstname = String.Empty;
            var lastname = String.Empty;

            if (!String.IsNullOrEmpty(name))
            {
                var response = this.parser.Parse(name);
                firstname = response.FirstName;
                lastname = response.LastName;
                // lead requires first + last but sometimes parser gets confused and doesn't return both
                if (String.IsNullOrEmpty(firstname)) firstname = lastname;
                if (String.IsNullOrEmpty(lastname)) lastname = firstname;
            }

            return $"{firstname} {lastname}".Trim();
        }

        private async Task CreateTicket(FormStackLeadViewModel formStackLeadViewModel)
        {
            var name = formStackLeadViewModel.Name ?? String.Empty;
            var email = (formStackLeadViewModel.Email ?? String.Empty).Trim();
            var phone = formStackLeadViewModel.Phone ?? String.Empty;
            var comments = formStackLeadViewModel.Comments ?? String.Empty;
            var ticketType = formStackLeadViewModel.TicketType == "Question" ? TicketType.Question : TicketType.Problem;

            name = this.ParseName(name);

            var createTicketCommand = new CreateTicketCommand
            {
                RequestedBy = name,
                Subject = "Service Request",
                Description = !String.IsNullOrEmpty(phone) ? $"Phone: {phone}; " + comments : comments,
                Type = ticketType,
                Priority = TicketPriority.Normal
            };
            if (!String.IsNullOrEmpty(email)) createTicketCommand.EmailAddress.Add(email);

            await this.bus.Send(createTicketCommand);
        }

        private async Task CreateLead(FormStackLeadViewModel formStackLeadViewModel)
        {
            var name = formStackLeadViewModel.Name ?? String.Empty;
            var email = formStackLeadViewModel.Email ?? String.Empty;
            var phone = formStackLeadViewModel.Phone ?? String.Empty;
            var comments = formStackLeadViewModel.Comments ?? String.Empty;
            var referrer = formStackLeadViewModel.Referrer ?? String.Empty;
            var ip = formStackLeadViewModel.Ip ?? String.Empty;
            var companyname = formStackLeadViewModel.Company ?? String.Empty;
            var firstname = "";
            var lastname = "";
            var productInterest = formStackLeadViewModel.ProductInterest;
            var requestType = RequestType.Other;
            
            if (!String.IsNullOrEmpty(name))
            {
                var response = this.parser.Parse(name);
                firstname = response.FirstName;
                lastname = response.LastName;
                // lead requires first + last but sometimes parser gets confused and doesn't return both
                if (String.IsNullOrEmpty(firstname)) firstname = lastname;
                if (String.IsNullOrEmpty(lastname)) lastname = firstname;
            }

            var sb = new StringBuilder();
            if (!String.IsNullOrEmpty(comments)) sb.AppendLine($"Comments: {comments}").AppendLine();
            if (!String.IsNullOrEmpty(formStackLeadViewModel.RecordCount)) sb.AppendLine($"Estimated Record Count: {formStackLeadViewModel.RecordCount}");
            if (!String.IsNullOrEmpty(formStackLeadViewModel.ProductInterest)) sb.AppendLine($"Product Interest: {formStackLeadViewModel.ProductInterest}");
            if (!String.IsNullOrEmpty(formStackLeadViewModel.LandingPageUrl)) sb.AppendLine($"Landing Page Url: {formStackLeadViewModel.LandingPageUrl}");

            var command = new CreateFormstackLeadCommand
            {
                Company = companyname,
                FirstName = firstname,
                LastName = lastname,
                ApplicationId = WellKnownIdentifiers.AccurateAppendId,
                Comments = sb.ToString(),
                Email = email,
                ProductInterest = productInterest,
                Ip = ip,
                Phone = phone,
                Referrer = referrer,
                RequestType = requestType,
                EstimatedCount = this.AttemptToParse(formStackLeadViewModel.RecordCount)
            };

            await this.bus.SendLocal(command);
        }

        private async Task CreateLead(GoogleLeadViewModel googleLeadViewModel)
        {
            var firstname = "";
            var lastname = "";

            var name = googleLeadViewModel.user_column_data
                           .FirstOrDefault(a => a.column_id == UserDataKeys.FullName)
                           ?.string_value ?? String.Empty;
            name = this.ParseName(name);

            if (!String.IsNullOrEmpty(name))
            {
                var response = this.parser.Parse(name);
                firstname = response.FirstName;
                lastname = response.LastName;
                // lead requires first + last but sometimes parser gets confused and doesn't return both
                if (String.IsNullOrEmpty(firstname)) firstname = lastname;
                if (String.IsNullOrEmpty(lastname)) lastname = firstname;
            }

            var sb = new StringBuilder();
            sb.AppendLine("Google lead parameters:\r\n");
            sb.AppendLine($"lead_id: {googleLeadViewModel.lead_id}");
            sb.AppendLine($"form_id: {googleLeadViewModel.form_id}");
            sb.AppendLine($"campaign_id: {googleLeadViewModel.campaign_id}");
            sb.AppendLine($"google_key: {googleLeadViewModel.google_key}");
            sb.AppendLine($"gcl_id: {googleLeadViewModel.gcl_id}");
            sb.AppendLine($"is_test: {googleLeadViewModel.is_test}");

            // When we get this settled out, we'll create a new command but we'll temp sidestep any initial grinding then add when we're sorted
            var command = new CreateFormstackLeadCommand
            {
                FirstName = firstname,
                LastName = lastname,
                ApplicationId = WellKnownIdentifiers.AccurateAppendId,
                Comments = sb.ToString(),
                Email = googleLeadViewModel.Email,
                Phone = googleLeadViewModel.Phone
            };

            await this.bus.SendLocal(command);
        }

        #endregion
    }
}
