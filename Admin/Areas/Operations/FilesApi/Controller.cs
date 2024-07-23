using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Operations;
using AccurateAppend.Operations.DataAccess;
using AccurateAppend.Websites.Admin.Areas.Clients.DownloadUserFile;
using AccurateAppend.Websites.Admin.Areas.Operations.FilesApi.Models;
using AccurateAppend.Websites.Admin.Navigator;
using DomainModel.ActionResults;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace AccurateAppend.Websites.Admin.Areas.Operations.FilesApi
{
    /// <summary>
    /// Controller providing API to <see cref="UserFile"/> data.
    /// </summary>
    [Authorize()]
    public class Controller : System.Web.Mvc.Controller
    {
        #region Fields

        private readonly DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Controller"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> dal component.</param>
        public Controller(DefaultContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            this.context = context;
        }

        #endregion

        #region Action Methods

        protected virtual IQueryable<FileDetail> CraftBaseQuery(Guid? userId = null, Guid? correlation = null, Guid? applicationid = null, ContentSource? uploadedBy = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            // Base  query
            var query = this.context
                .Set<UserFile>()
                .Join(this.context.Set<ClientRef>(), f => f.Owner, c => c.UserId, (f, c) => new { File = f, User = c });
            if (userId != null) query = query.Where(f => f.File.Owner == userId.Value);
            if (correlation != null) query = query.Where(f => f.File.CorrelationId == correlation.Value);
            if (applicationid != null) query = query.Where(f => f.User.ApplicationId == applicationid.Value);
            if (uploadedBy != null) query = query.Where(f => f.File.UploadBy == uploadedBy);
            if (startDate != null)
            {
                startDate = startDate.Value.FromUserLocal();
                query = query.Where(f => f.File.CreatedDate >= startDate);
            }

            if (endDate != null)
            {
                endDate = endDate.Value.FromUserLocal().ToEndOfDay();
                query = query.Where(f => f.File.CreatedDate <= endDate);
            }

            return query.Select(f=>
                new FileDetail
                {
                    ApplicationId = f.User.ApplicationId,
                    UserId = f.User.UserId,
                    UserName = f.User.UserName,
                    CorrelationId = f.File.CorrelationId,
                    CustomerFileName = f.File.CustomerFileName,
                    SystemFileName = f.File.FileName,
                    CreatedDate = f.File.CreatedDate,
                    FileId = f.File.Id,
                    FileSize = f.File.FileSize
                });
        }

        public virtual async Task<ActionResult> List([DataSourceRequest] DataSourceRequest request, Guid? applicationid, Guid ? userId, ContentSource? uploadedBy, DateTime? startDate, DateTime? endDate, CancellationToken cancellation)
        {
            var query = this.CraftBaseQuery(userId, applicationid: applicationid, uploadedBy: uploadedBy, startDate: startDate, endDate: endDate);

            var data = (await query
                    .OrderByDescending(f => f.CreatedDate)
                    .ToArrayAsync(cancellation))
                    .ToDataSourceResult(request, f =>
                    new
                    {
                        Id = f.FileId,
                        f.CorrelationId,
                        f.UserId,
                        f.UserName,
                        f.ApplicationId,
                        f.CustomerFileName,
                        f.SystemFileName,
                        CreatedDate = f.CreatedDate.ToUserLocal(),
                        FileSize = IntegerExtensions.FormatBytes(f.FileSize),
                        Links = new
                        {
                            UserDetail = this.Url.Action("Index", "UserDetail", new { Area = "Clients", f.UserId }),
                            DownloadUri = this.DownloadFileLink(f.CreatedDate, f.FileId),
                            Correlate = (String)null
                        },
                        Actions = new
                        {
                            NewJob = this.NewJobLink(f.CreatedDate, f.FileId),
                            NewDeal = this.NewDealLink(f.CreatedDate, f.UserId, f.FileId),
                            ItemDetail = this.Url.Action("Index", "UserFileDetail", new { Area = "Clients", Id = f.FileId })
                        }
                    });

            var jsonNetResult = new JsonNetResult(DateTimeKind.Local)
            {
                Data = data
            };
            return jsonNetResult;
        }

        public virtual async Task<ActionResult> ForCorrelationId([DataSourceRequest] DataSourceRequest request, Guid id, ContentSource? uploadedBy, CancellationToken cancellation)
        {
            var query = this.CraftBaseQuery(correlation: id, uploadedBy: uploadedBy);

            var data = (await query
                    .OrderByDescending(f => f.CreatedDate)
                    .ToArrayAsync(cancellation))
                    .ToDataSourceResult(request, f =>
                    new
                    {
                        Id = f.FileId,
                        f.CorrelationId,
                        f.UserId,
                        f.UserName,
                        f.ApplicationId,
                        f.CustomerFileName,
                        f.SystemFileName,
                        CreatedDate = f.CreatedDate.ToUserLocal(),
                        FileSize = IntegerExtensions.FormatBytes(f.FileSize),
                        Links = new
                        {
                            UserDetail = this.Url.Action("Index", "UserDetail", new {Area = "Clients", f.UserId}),
                            DownloadUri = this.DownloadFileLink(f.CreatedDate, f.FileId),
                            Correlate = (String) null
                        },
                        Actions = new
                        {
                            NewJob = this.NewJobLink(f.CreatedDate, f.FileId),
                            NewDeal = this.NewDealLink(f.CreatedDate, f.UserId, f.FileId),
                            ItemDetail = this.Url.Action("Index", "UserFileDetail", new { Area = "Clients", Id = f.FileId })
                        }
                    });

            var jsonNetResult = new JsonNetResult(DateTimeKind.Local)
            {
                Data = data
            };
            return jsonNetResult;
        }

        public virtual async Task<ActionResult> Uncorrelated([DataSourceRequest] DataSourceRequest request, Guid? userId, ContentSource? uploadedBy, DateTime? startDate, DateTime? endDate, CancellationToken cancellation)
        {
            var query = this.CraftBaseQuery(userId, uploadedBy: uploadedBy, startDate: startDate, endDate: endDate);
            query = query.Where(f => f.CorrelationId == null);

            var data = (await query
                    .OrderByDescending(f => f.CreatedDate)
                    .ToArrayAsync(cancellation))
                    .ToDataSourceResult(request, f =>
                    new
                    {
                        Id = f.FileId,
                        f.CorrelationId,
                        f.UserId,
                        f.UserName,
                        f.ApplicationId,
                        f.CustomerFileName,
                        f.SystemFileName,
                        CreatedDate = f.CreatedDate.ToUserLocal(),
                        FileSize = IntegerExtensions.FormatBytes(f.FileSize),
                        Links = new
                        {
                            UserDetail = this.Url.Action("Index", "UserDetail", new { Area = "Clients", f.UserId }),
                            DownloadUri = this.DownloadFileLink(f.CreatedDate, f.FileId),
                            Correlate = (String)null
                        },
                        Actions = new
                        {
                            NewJob = this.NewJobLink(f.CreatedDate, f.FileId),
                            NewDeal = this.NewDealLink(f.CreatedDate, f.UserId, f.FileId),
                            ItemDetail = this.Url.Action("Index", "UserFileDetail", new { Area = "Clients", Id = f.FileId })
                        }
                    });

            var jsonNetResult = new JsonNetResult(DateTimeKind.Local)
            {
                Data = data
            };
            return jsonNetResult;
        }

        #endregion

        #region Helpers

        protected virtual Boolean CheckFileAge(DateTime createdDate)
        {
            return DateTime.UtcNow - createdDate.Coerce() < TimeSpan.FromDays(21);
        }

        protected virtual String NewJobLink(DateTime createdDate, Guid id)
        {
            return this.CheckFileAge(createdDate)
                ? this.Url.Action("FromAdminFile", "Batch", new { area = String.Empty, fileId = id })
                : null;
        }

        protected virtual String DownloadFileLink(DateTime createdDate, Guid id)
        {
            return this.CheckFileAge(createdDate)
                ? this.Url.BuildFor<DownloadUserFileController>().Download(id)
                : null;
        }

        protected virtual String NewDealLink(DateTime createdDate, Guid userId, Guid fileId)
        {
            return this.CheckFileAge(createdDate)
                ? this.Url.Action("Create", "NewDeal", new { Area = "Sales", userId })
                : null;
        }

        #endregion
    }
}