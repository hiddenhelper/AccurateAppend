using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.Areas.Public.Documentation
{
    /// <summary>
    /// Responsible for managing customer facing documentation.
    /// </summary>
    [AllowAnonymous()]
    public class Controller : System.Web.Mvc.Controller
    {
        #region Action Methods

        /// <summary>
        /// Action method to display the match codes.
        /// </summary>
        public ActionResult ProcessingCodes()
        {
            return this.View();
        }

        /// <summary>
        /// Action method to display batch processing overview
        /// </summary>
        public ActionResult BatchApiOverview()
        {
            return this.View();
        }

        /// <summary>
        /// Action method to display ftp batch processing
        /// </summary>
        public ActionResult FtpBatchApi()
        {
            return this.View();
        }

        /// <summary>
        /// Action method to display smtp batch processing
        /// </summary>
        public ActionResult SmtpBatchApi()
        {
            return this.View();
        }

        /// <summary>
        /// Action method to display batch processing file formatting overview
        /// </summary>
        public ActionResult BatchApi_FileFormatting()
        {
            return this.View();
        }

        /// <summary>
        /// Action method to display batch processing file mapping overview
        /// </summary>
        public ActionResult BatchApi_FileFormatting_Header()
        {
            return this.View();
        }

        /// <summary>
        /// Action method to display Nation Builder account creation
        /// </summary>
        public ActionResult NationBuilder_Account_Setup()
        {
            return this.View();
        }
        
        #endregion
    }
}