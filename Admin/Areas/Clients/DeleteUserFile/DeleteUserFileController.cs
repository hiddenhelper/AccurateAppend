using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Data;
using AccurateAppend.Operations;
using AccurateAppend.Operations.DataAccess;
using EntityFramework.Extensions;

namespace AccurateAppend.Websites.Admin.Areas.Clients.DeleteUserFile
{
    /// <summary>
    /// Used to delete files that were uploaded in the admin using the clients.xxx.com/net url
    /// </summary>
    [Authorize()]
    public class DeleteUserFileController : Controller
    {
        #region Fields

        private readonly DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteUserFileController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        public DeleteUserFileController(DefaultContext context)
        {
            this.context = context;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Deletes a file by the system file name value.
        /// </summary>
        public virtual async Task<ActionResult> Index(Guid id, CancellationToken cancellation)
        {
            try
            {
                var ra = await this.context.Set<UserFile>().Where(f => f.Id == id || f.FileName == id.ToString()).DeleteAsync();
            }
            catch (DbException ex)
            {
                return this.Json(new {success = false, message = ex.Message}, JsonRequestBehavior.AllowGet);
            }
            
            return this.Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}