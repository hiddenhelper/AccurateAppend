using AccurateAppend.Core.Definitions;
using DomainModel.ActionResults;
using DomainModel.JsonNET;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.ListBuilder.Controllers;
using AccurateAppend.ListBuilder.DataSources.ConsumerProfile;
using AccurateAppend.ListBuilder.Models;
using AccurateAppend.Plugin.Storage;

namespace AccurateAppend.Websites.Admin.Areas.ListBuilder.Controllers
{
    /// <summary>
    /// Builds a list using a given criteria
    /// </summary>
    [Authorize()]
    public class BuildListController : BuildListControllerBase
    {
        #region Fields

        private readonly AzureBlobStorageLocation temp;
        private readonly AzureBlobStorageLocation rawCustomerInputFiles;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildListController" /> class.
        /// </summary>
        public BuildListController(AzureBlobStorageLocation temp, AzureBlobStorageLocation rawCustomerInputFiles, IDataAccess data) : base(data)
        {
            if (temp == null) throw new ArgumentNullException(nameof(temp));
            if (rawCustomerInputFiles == null) throw new ArgumentNullException(nameof(rawCustomerInputFiles));
            Contract.EndContractBlock();

            this.temp = temp;
            this.rawCustomerInputFiles = rawCustomerInputFiles;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Generates list record counts based on a given criteria
        /// </summary>
        /// <param name="listCriteria"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public virtual async Task<ActionResult> Query(
            [ModelBinder(typeof(FormCollectionJsonBinder))] ListCriteria listCriteria)
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
                            HttpStatusCodeResult = (Int32) HttpStatusCode.BadRequest,
                            Message = errors.ErrorMessage,
                            Count = 0
                        }
                    };
                }

                // Lets push this GUID into the critiera.
                var filename = Guid.NewGuid().ToString();

                var file = this.temp.CreateBlobInstance(filename);

                await this.GenerateList(file, listCriteria);
                
                return new JsonNetResult
                {
                    Data = new
                    {
                        HttpStatusCodeResult = (Int32) HttpStatusCode.OK,
                        ListDownloadUri = file.GetSharedAccessUri(DateTime.UtcNow.AddMinutes(10), SharedAccessBlobPermissions.Read, $"{file.Name}.csv"),
                        ListDefintionDownloadUri = String.Empty,  //download.ListDefintion.GetSharedAccessUri(DateTime.UtcNow.AddMinutes(10), SharedAccessBlobPermissions.Read, $"{download.ListDefintion.Name}.csv"),
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
                        HttpStatusCodeResult = (Int32) HttpStatusCode.InternalServerError,
                        Message = "Unable to retrieve list",
                        FileName = String.Empty
                    }
                };
            }
        }

        /// <summary>
        /// Generates list and then starts the dynamic append process
        /// </summary>
        /// <param name="listCriteria">The criteria from the GUI to create the list from.</param>
        [AcceptVerbs(HttpVerbs.Post)]
        public async Task<ActionResult> Create(
            [ModelBinder(typeof(FormCollectionJsonBinder))] ListCriteria listCriteria)
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
                            HttpStatusCodeResult = (Int32) HttpStatusCode.BadRequest,
                            Message = errors.ErrorMessage,
                            Count = 0
                        }
                    };
                }

                // Lets push this GUID into the critiera.
                var filename = Guid.NewGuid().ToString();

                var file = this.rawCustomerInputFiles.CreateBlobInstance(filename);

                await this.GenerateList(file, listCriteria);

                return new JsonNetResult
                {
                    Data = new
                    {
                        HttpStatusCodeResult = (Int32) HttpStatusCode.OK,
                        DownloadUri = this.Url.Action("FromListBuilder", "Batch", new {Area = "", Id = file.Name}),
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
                        HttpStatusCodeResult = (Int32) HttpStatusCode.InternalServerError,
                        Message = "Unable to retrieve list",
                        FileName = String.Empty
                    }
                };
            }
        }

        #endregion
    }
}