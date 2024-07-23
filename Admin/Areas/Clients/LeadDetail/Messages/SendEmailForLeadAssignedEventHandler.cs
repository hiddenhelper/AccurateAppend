using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AccurateAppend.Accounting;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.CustomerManagement.Contracts;
using AccurateAppend.Data;
using AccurateAppend.Operations.Contracts;
using AccurateAppend.Websites.Admin.Navigator;
using EventLogger;
using NServiceBus;
using DefaultContext = AccurateAppend.Accounting.DataAccess.DefaultContext;

namespace AccurateAppend.Websites.Admin.Areas.Clients.LeadDetail.Messages
{
    /// <summary>
    /// Handler to dispatch a lead assignment email notification when a <see cref="LeadAssignedEvent"/> occurs.
    /// </summary>
    public class SendEmailForLeadAssignedEventHandler : IHandleMessages<LeadAssignedEvent>
    {
        #region Fields

        private readonly DefaultContext dataContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SendEmailForLeadAssignedEventHandler"/> class.
        /// </summary>
        public SendEmailForLeadAssignedEventHandler(DefaultContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));

            this.dataContext = dataContext;
        }

        #endregion

        #region IHandleMessages<LeadAssignedEvent> Members

        /// <inheritdoc />
        public virtual async Task Handle(LeadAssignedEvent message, IMessageHandlerContext context)
        {
            using (new Correlation(message.PublicKey))
            {
                Logger.LogEvent($"Lead {message.PublicKey} assigned to {message.AssignedTo.UserName}", Severity.None, Application.AccurateAppend_Admin);

                if (message.AssignedTo.UserId == WellKnownIdentifiers.SystemUserId) return;

                var leadId = await this.dataContext
                    .SetOf<Lead>()
                    .Where(l => l.PublicKey == message.PublicKey)
                    .Where(l => l.Status != LeadStatus.ConvertedToCustomer)
                    .Select(l => l.Id)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);
                if (leadId == null) return;

                var command = new SendEmailCommand
                {
                    Track = false,
                    Body = $"A lead has been assigned to you. \r\n{this.GenerateHelpLink(leadId.Value)}",
                    Subject = "Lead assignment",
                    IsHtmlContent = false,
                    MessageKey = message.PublicKey
                };
                command.To.Add(message.AssignedTo.UserName);
                command.SendFrom = "support@accurateappend.com";

                await context.Send(command);
            }
        }

        #endregion

        /// <summary>
        /// Since MVC url routing components are not set up by default in a POCO,
        /// this method performs a basic hack to create a <see cref="UrlHelper"/>
        /// that can be used to generate the appropriate paths.
        /// </summary>
        /// <param name="leadId">The identifier of the lead to generate a detail view for.</param>
        /// <returns>The path to the detail view of the lead.</returns>
        protected virtual String GenerateHelpLink(Int32 leadId)
        {
            var request = new HttpRequest("/", "http://admin.accurateappend.com", String.Empty);
            var response = new HttpResponse(new StringWriter());
            var httpContext = new HttpContext(request, response);
            var httpContextBase = new HttpContextWrapper(httpContext);

            var routeData = new RouteData();
            var myHostContext = new RequestContext(httpContextBase, routeData);

            var url = new UrlHelper(myHostContext);
            var link = new UrlBuilder<LeadDetailController>(url).ToDetail(leadId, Uri.UriSchemeHttps);

            return link;
        }
    }
}