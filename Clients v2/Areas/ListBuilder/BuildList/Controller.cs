using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.ListBuilder.Models;
using AccurateAppend.Websites.Clients.Areas.ListBuilder.BuildList.Messaging;
using DomainModel.ActionResults;
using DomainModel.JsonNET;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.ListBuilder.BuildList
{
    /// <summary>
    /// Builds a list using a given criteria.
    /// </summary>
    [Authorize()]
    public class Controller : System.Web.Mvc.Controller
    {
        #region Fields

        private readonly IMessageSession bus;
        private readonly Sales.DataAccess.DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller" /> class.
        /// </summary>
        public Controller(IMessageSession bus, Sales.DataAccess.DefaultContext context)
        {
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.bus = bus;
            this.context = context;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Generates list and then starts the dynamic append process
        /// </summary>
        /// <param name="listCriteria">The criteria from the GUI to create the list from.</param>
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Create(
            [ModelBinder(typeof(FormCollectionJsonBinder))] ListCriteria listCriteria, CancellationToken cancellation)
        {
            listCriteria = listCriteria ?? new ListCriteria();

            try
            {
                var errors = listCriteria.Validate().FirstOrDefault();
                if (errors != null)
                {
                    return new JsonNetResult
                    {
                        Data = new
                        {
                            HttpStatusCodeResult = (Int32)HttpStatusCode.BadRequest,
                            Message = errors.ErrorMessage,
                            Count = 0
                        }
                    };
                }

                var command = new GenerateListCommand();
                command.Criteria = listCriteria;
                await this.bus.Send(command);

                return new JsonNetResult
                {
                    Data = new
                    {
                        HttpStatusCodeResult = (Int32)HttpStatusCode.OK,
                        DownloadUri = this.Url.Action("FromListBuilder", "Order", new { Area = "ListBuilder", id = listCriteria.RequestId }),
                        Message = String.Empty
                    }
                };
            }
            catch (Exception ex)
            {
                EventLogger.Logger.LogEvent(ex, Severity.Low, Application.AccurateAppend_Admin);
                return new JsonNetResult
                {
                    Data = new
                    {
                        HttpStatusCodeResult = (Int32)HttpStatusCode.InternalServerError,
                        Message = "Unable to retrieve list",
                        FileName = String.Empty
                    }
                };
            }
        }

        public virtual async Task<ActionResult> CheckFileStatus(Guid id, CancellationToken cancellation)
        {
            var ready = await this.context
                .SetOf<Sales.Cart>()
                .AnyAsync(c => c.Id == id, cancellation);

            return this.Json(new {Complete = ready}, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}