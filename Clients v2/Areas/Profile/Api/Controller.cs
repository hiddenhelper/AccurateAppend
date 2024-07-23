using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.Websites.Clients.Areas.Profile.Api.Models;
using DefaultContext = AccurateAppend.Sales.DataAccess.DefaultContext;

namespace AccurateAppend.Websites.Clients.Areas.Profile.Api
{
    /// <summary>
    /// Controller to update the interactive user real-time API access.
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
        /// /// <param name="context">The <see cref="ISessionContext"/> component providing data access to this instance.</param>
        public Controller(DefaultContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Action to display the password update form.
        /// </summary>
        [HttpGet()]
        public virtual async Task<ActionResult> Index(CancellationToken cancellation)
        {
            var userId = this.User.Identity.GetIdentifier();

            var xmlEnabled = await this.context
                .Database
                .SqlQuery<ApiDetailsModel>(@"
SELECT [UserId] [Key],
CASE WHEN [XmlAccess] IS NULL
THEN CAST(0 AS BIT)
ELSE CAST(1 AS BIT) END [ApiEnabled],
CASE WHEN [BatchAccess] IS NULL
THEN CAST(0 AS BIT)
ELSE CAST(1 AS BIT) END [BatchEnabled],
[UserName] [FtpUser], [Password] [FtpPassword]
FROM
(

SELECT ud.[UserId],xml.UserId [XmlAccess],batch.UserId [BatchAccess], ftp.[UserName], ftp.[Password]
FROM [accounts].[UserDetail] ud
LEFT JOIN [security].[UserAccess] xml ON xml.[UserId] = ud.[UserId] AND xml.[RoleName] = 'XML User'
LEFT JOIN [security].[UserAccess] batch ON batch.[UserId] = ud.[UserId] AND batch.[RoleName] = 'Batch User'
LEFT JOIN [jobs].[FTPBatchUsers] ftp ON ftp.[UserId] = ud.[UserId]
WHERE ud.[UserId] = @p0

) s", userId)
                .FirstAsync(cancellation);

            return this.View(xmlEnabled);
        }

        #endregion
    }
}