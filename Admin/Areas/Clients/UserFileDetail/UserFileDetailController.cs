using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.Operations;
using AccurateAppend.Operations.DataAccess;
using AccurateAppend.Websites.Admin.Areas.Clients.DownloadUserFile;
using AccurateAppend.Websites.Admin.Areas.Clients.UserDetail;
using AccurateAppend.Websites.Admin.Navigator;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Admin.Areas.Clients.UserFileDetail
{
    /// <summary>
    /// Controller performing detail operation of <see cref="UserFile"/> entities.
    /// </summary>
    [Authorize()]
    public class UserFileDetailController : Controller
    {
        #region Fields

        private readonly DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UserFileDetailController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> to use for this controller instance.</param>
        public UserFileDetailController(DefaultContext context)
        {
            this.context = context;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Json object containing single UserFile
        /// </summary>
        public virtual async Task<ActionResult> Index(Guid id, CancellationToken cancellation)
        {
            var file = await this.context
                .Set<UserFile>()
                .Where(s => s.Id == id)
                .Join(this.context.Set<ClientRef>(), f => f.Owner, c => c.UserId, (f, u) => new {File = f, User = u})
                .Select(r => new
                {
                    FileId = r.File.Id,
                    PublicKey = r.File.Id,
                    DateAdded = r.File.CreatedDate,
                    r.File.CustomerFileName,
                    SystemFileName = r.File.FileName,
                    Size = r.File.FileSize,
                    r.User.UserId,
                    r.User.UserName,
                    r.File.UploadBy,
                    Applicationid = r.User.ApplicationId
                })
                .FirstAsync(cancellation);

            var url = file.Applicationid == WellKnownIdentifiers.TwentyTwentyId
                ? Properties.Settings.Default.url_Clients2020
                : Properties.Settings.Default.url_ClientsAccurateAppend;

            var builder = new UriBuilder(url)
            {
                Path = $@"/Public/File/Download/{file.FileId}"
            };
            if (builder.Uri.IsDefaultPort) builder.Port = -1;

            var result = new JsonNetResult(DateTimeKind.Local)
            {
                Data = new
                {
                    file.FileId,
                    file.PublicKey,
                    DateAdded = file.DateAdded.ToUserLocal(),
                    file.CustomerFileName,
                    file.SystemFileName,
                    file.Size,
                    file.UserId,
                    file.UserName,
                    file.UploadBy,
                    file.Applicationid,
                    Links = new
                    {
                        DownloadUri = this.Url.BuildFor<DownloadUserFileController>().Download(file.PublicKey),
                        PublicDownloadUri = file.UploadBy == ContentSource.Admin ? builder.ToString() : null,
                        UserDetail = this.Url.BuildFor<UserDetailController>().ToDetail(file.UserId)
                    },
                    Actions = new
                    {
                        NewJob = this.Url.Action("FromAdminFile", "Batch", new { area = String.Empty, fileId = file.FileId }),
                        Delete = this.Url.Action("Index", "DeleteUserFile", new { area = "Clients", id = file.FileId })
                    }
                }
            };
            return result;
        }

        #endregion
    }
}