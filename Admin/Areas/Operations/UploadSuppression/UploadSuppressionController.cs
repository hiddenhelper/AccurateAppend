using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AccurateAppend.Data;
using AccurateAppend.Websites.Admin.Controllers;
using DAL.Properties;

namespace AccurateAppend.Websites.Admin.Areas.Operations.UploadSuppression
{
    [Authorize()]
    public class UploadSuppressionController : ActivityLoggingController
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadSuppressionController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        public UploadSuppressionController(ISessionContext context) : base(context)
        {
        }

        #endregion

        #region Action Methods

        [HttpGet()]
        public virtual ActionResult Index()
        {
            return this.View(Guid.NewGuid());
        }

        [HttpPost()]
        public virtual async Task<ActionResult> Index(Guid id, IEnumerable<HttpPostedFileBase> files)
        {
            await this.LogEventAsync($"User loaded suppression set {id}");

            using (var db = new DbContext(Settings.Default.OperationResultSuppressionConnectionString))
            {
                foreach (var file in files)
                {
                    var parameters = new List<Object>();
                    parameters.Add(id);

                    var sb = new StringBuilder();
                    sb.AppendLine($"if not exists (select * from dbo.SuppressionSet where id=@p0) insert into dbo.SuppressionSet values (@p0, dateadd(day, 30, getdate()))");

                    using (var reader = new DataStreams.Csv.CsvReader(file.InputStream, Encoding.Default))
                    {
                        var i = 0;
                        while (reader.ReadRecord())
                        {
                            i = i + 1;
                            sb.AppendLine($"if not exists (select * from dbo.Emails where id=@p0 and Email=@p{i}) insert into dbo.Emails values (@p0, @p{i})");
                            parameters.Add(reader[0]);
                        }
                    }

                    await db.Database.ExecuteSqlCommandAsync(sb.ToString(), parameters.ToArray());

                    return this.RedirectToAction("Index", "Dashboard", new {Area = "Operations"});
                }
            }
            return this.RedirectToAction("Index");
        }

        #endregion
    }
}