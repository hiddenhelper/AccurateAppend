using AccurateAppend.Core.Definitions;
using DomainModel.ActionResults;
using DomainModel.JsonNET;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.ListBuilder.DataSources.ConsumerProfile;
using ListCriteria = AccurateAppend.ListBuilder.Models.ListCriteria;

namespace AccurateAppend.Websites.Admin.Areas.ListBuilder.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Generates list record counts based on a given criteria
    /// </summary>
    [Authorize()]
    public class GenerateListCountController : Controller
    {
        #region Fields

        private readonly IDataAccess data;

        #endregion

        #region Constructor

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AccurateAppend.Websites.Admin.Areas.ListBuilder.Controllers.GenerateListCountController" /> class.
        /// </summary>
        public GenerateListCountController(IDataAccess data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            this.data = data;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Generates list record counts based on a given criteria
        /// </summary>
        /// <param name="listCriteria"></param>
        /// <returns></returns>
        // GET: ListBuilder/GenerateListCount
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Query([ModelBinder(typeof(FormCollectionJsonBinder))] ListCriteria listCriteria, CancellationToken cancellation)
        {
            listCriteria = listCriteria ?? new ListCriteria();

            try
            {
                var errors = listCriteria.Validate().FirstOrDefault();
                if (errors != null)
                {
                    return new JsonNetResult
                    {
                        Data = new { HttpStatusCodeResult = (Int32)HttpStatusCode.BadRequest, Message = errors.ErrorMessage, Count = 0 }
                    };
                }

                var count = await this.data.GetCountAsync(listCriteria, cancellation);
                return new JsonNetResult
                {
                    Data = new { HttpStatusCodeResult = (Int32)HttpStatusCode.OK, Message = String.Empty, Count = count }
                };
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached) Debugger.Break();

                EventLogger.Logger.LogEvent(ex, Severity.Low, Application.AccurateAppend_Admin);
                return new JsonNetResult
                {
                    Data = new { HttpStatusCodeResult = (Int32)HttpStatusCode.InternalServerError, Message = "Unable to retrieve count", Count = 0 }
                };
            }
        }

        #endregion
    }
}