using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AccurateAppend.Accounting;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Websites.Admin.Controllers;
using DomainModel;
using EventLogger;
using Application = AccurateAppend.Core.Definitions.Application;

namespace AccurateAppend.Websites.Admin.Areas.Clients.AdminFile
{
    /// <summary>
    /// Functionality to save and manage customer documents
    /// </summary>
    public class AdminFileController : ContextBoundController
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminFileController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> dal component.</param>
        public AdminFileController(ISessionContext context) : base(context)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns summary of admin files for user
        /// </summary>
        public virtual ActionResult Summary(Guid userid)
        {
            var files = this.Context.SetOf<AdminDocument>().Where(f => f.For.Id == userid)
                .Include(f => f.For)
                .Select(a => new
                {
                    FileId = a.LegacyId,
                    a.CreatedDate,
                    CustomerFileName = a.FileName,
                    a.FileSize,
                    UserId = a.For.Id,
                    a.Notes
                })
                .OrderByDescending(o => o.CreatedDate).ToArray();

                var data = files.Select(a => new
                {
                    a.FileId,
                    DateAdded = a.CreatedDate.ToUserLocal().ToString(CultureInfo.CurrentUICulture),
                    a.CustomerFileName,
                    Size = Core.IntegerExtensions.FormatBytes(a.FileSize),
                    a.UserId,
                    Notes = String.IsNullOrEmpty(a.Notes) ? "" : a.Notes
                });

            return this.Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save admin document into database
        /// </summary>
        public virtual ActionResult Save(IEnumerable<HttpPostedFileBase> files, Guid userid)
        {
            if (files == null) return this.Content("");

            try
            {
                var user = this.Context.SetOf<Security.User>().First(u => u.Id == userid);

                using (this.Context.CreateScope(ScopeOptions.AutoCommit))
                {
                    foreach (var file in files)
                    {
                        var adminFile = new AdminDocument(file.FileName, file.InputStream, user);
                        this.Context.SetOf<AdminDocument>().Add(adminFile);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogEvent(ex, Severity.High, Application.AccurateAppend_Admin, this.Request.UserHostAddress, "AdminFile upload failing.");
                throw;
            }
            // Return an empty string to signify success
            return this.Content("");
        }

        /// <summary>
        /// Download a file
        /// </summary>
        public ActionResult Download(int fileid)
        {
            using (this.Context.CreateScope(ScopeOptions.NoTracking))
            {
                var file = this.Context.SetOf<AdminDocument>()
                        .Where(f => f.LegacyId == fileid)
                        .Select(f => new {f.Content, f.FileName})
                        .First();
                return this.File(file.Content, MimeTypeHelper.ConvertMimeType(file.FileName), file.FileName);
            }
        }

        /// <summary>
        /// Delete a file
        /// </summary>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(int fileid)
        {
            using (this.Context.CreateScope(ScopeOptions.AutoCommit))
            {
                var file = this.Context.SetOf<AdminDocument>().First(f => f.LegacyId == fileid);
                this.Context.SetOf<AdminDocument>().Remove(file);
            }

            return new JsonResult { Data = true };

        }

        /// <summary>
        /// Adds a note to given file
        /// </summary>
        /// <param name="fileid"></param>
        /// <param name="notes"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult AddNote(int fileid, string notes)
        {
            notes = notes ?? String.Empty;
            if (notes.Length > 1000)
            {
                throw new Exception($"Notes can be 5000 characters max but your note is {notes.Length} characters.");
            }

            using (this.Context.CreateScope(ScopeOptions.AutoCommit))
            {
                var file = this.Context.SetOf<AdminDocument>().First(f => f.LegacyId == fileid);
                file.Notes = notes;
            }

            return new JsonResult { Data = true };
        }

        #endregion
    }
}