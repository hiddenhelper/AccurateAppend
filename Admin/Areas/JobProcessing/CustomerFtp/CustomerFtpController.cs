using AccurateAppend.Websites.Admin.Controllers;
using System.Web.Mvc;
using GleamTech.FileSystems.Physical;
using GleamTech.FileUltimate.AspNet.UI;

namespace AccurateAppend.Websites.Admin.Areas.JobProcessing.CustomerFtp
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize()]
    public class CustomerFtpController : ActivityLoggingController2
    {
        #region Action Methods

        /// <summary>
        /// As the site is built on master pages and webforms but GleamTech only
        /// works with razor, we have to use an indirection via a wrapper view.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return this.View();
        }

        /// <summary>
        /// Displays the root FTP folder.
        /// </summary>
        public ActionResult Root()
        {
            this.OnEvent("Customer Ftp viewed");

            //var rootFolder = new FileManagerRootFolder
            //{
            //    Name = "Azure Blob Storage",

            //    Location = new AzureBlobLocation
            //    {
            //        //Leave path empty to connect to the root of the container. 
            //        //For subfolders, path should be specified as a relative path
            //        //without leading slash (eg. "some/folder")
            //        Path = "",

            //        //Get these values from your Azure Portal (Storage Account -> Access Keys -> Connection String)
            //        Container = "myContainerName",
            //        AccountName = "myAccountName",
            //        AccountKey = "myAccountKey",

            //        //Optional:
            //        //These are the default values, usually you don't need to set/change them
            //        UseHttps = true,
            //        EndpointSuffix = "core.windows.net"
            //    }
            //};

            var rootFolder = new FileManagerRootFolder
            {
                Name = "FTP",
                Location = new PhysicalLocation
                {
                    Path = @"\\c4d1\FTPRoot"
                }
            };
            rootFolder.AccessControls.Add(new FileManagerAccessControl
            {
                Path = @"\",
                AllowedPermissions = FileManagerPermissions.ReadOnly
            });

            return this.View(rootFolder);
        }

        #endregion
    }
}