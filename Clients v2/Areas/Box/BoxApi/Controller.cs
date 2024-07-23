using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Data;
using AccurateAppend.JobManagement.Contracts;
using Box.V2;
using Box.V2.Auth;
using Box.V2.Config;
using DomainModel.ActionResults;

namespace AccurateAppend.Websites.Clients.Areas.Box.BoxApi
{
    /// <summary>
    /// Controller for supplying Box.com integration information.
    /// </summary>
    [Authorize()]
    public class BoxApiController : Controller
    {
        #region Fields

        private readonly DataContext context;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="BoxApiController" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DataContext" /> component providing data access.</param>
        public BoxApiController(DataContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            this.context = context;
        }

        #endregion

        #region Action Methods

        public virtual async Task<ActionResult> ForCurrentUser(CancellationToken cancellation)
        {
            var cutOff = DateTime.UtcNow.AddMinutes(-5);

            var data = (await this.context.SetOf<BoxRegistration>()
                    .ForInteractiveUser()
                    .Where(r => r.DateExpires > cutOff)
                    .OrderByDescending(r => r.DateRegistered)
                    .Select(r =>
                        new
                        {
                            r.Name,
                            r.Id,
                            r.UserId,
                            r.DateRegistered,
                            r.DateExpires,
                            r.PublicKey
                        })
                    .ToListAsync(cancellation))
                .Select(r =>
                    new
                    {
                        r.Name,
                        r.Id,
                        r.UserId,
                        r.DateRegistered,
                        IsActive = r.DateExpires > DateTime.UtcNow.AddMinutes(-5),
                        r.PublicKey,
                        Actions =
                            new
                            {
                                Enumerate = Url.Action("Enumerate", new {Area = "Box", regId = r.Id}),
                                Renew = Url.Action("Initiate", "AuthHandler", new {Area = "Box", regId = r.Id})
                            }
                    }).OrderByDescending(a => a.Id).FirstOrDefault();

            var result = new JsonNetResult(DateTimeKind.Utc) {Data = new {data}};
            return result;
        }

        public async Task<ActionResult> Enumerate(Int32 regId, Int64? nodeId, CancellationToken cancellation)
        {
            if (nodeId == null) nodeId = 0;

            if (!await this.context
                .SetOf<BoxRegistration>()
                .ForInteractiveUser()
                .AnyAsync(r => r.Id == regId, cancellation))
            {
                throw new SecurityException("Registration is not allowed access from this account");
            }

            var client = await this.BuildClient(regId, cancellation);
            var folderItems =
                await client.FoldersManager.GetFolderItemsAsync(nodeId.ToString(), 1000, autoPaginate: true);

            const String Folder = "folder";

            var data = folderItems.Entries.Select(item =>
                new
                {
                    item.Name,
                    NodeId = item.Id,
                    spriteCssClass = item.Type == Folder
                        ? "folder"
                        : Path.GetExtension(item.Name)?.Replace(".", String.Empty),
                    HasChildren = item.Type == Folder,
                    Actions = new
                    {
                        Enumerate = item.Type == Folder
                            ? Url.Action("Enumerate", new {Area = "Box", regId, NodeId = item.Id})
                            : null,
                        Details = item.Type != Folder
                            ? Url.Action("Details", "BoxApi", new {Area = "Box", regId, NodeId = item.Id})
                            : null
                    }
                });

            var result = new JsonNetResult {Data = data};

            return result;
        }

        public async Task<ActionResult> Details(Int32 regId, Int64 nodeId, CancellationToken cancellation)
        {
            if (!await this.context
                .SetOf<BoxRegistration>()
                .ForInteractiveUser()
                .AnyAsync(r => r.Id == regId, cancellation))
                throw new SecurityException("Registration is not allowed access from this account");

            var client = await this.BuildClient(regId, cancellation);
            var details = await client.FilesManager.GetInformationAsync(nodeId.ToString());

            var data = new
            {
                NodeId = details.Id,
                details.Name,
                Size = IntegerExtensions.FormatBytes(details.Size ?? 0),
                Extension = Path.GetExtension(details.Name),
                IsSupported = JobPipeline.IsSupported(details.Name),
                details.Description,
                ModifiedAt = details.ModifiedAt.ToSafeLocal().Coerce(),
                ModifiedBy = details.ModifiedBy.Name
            };

            var result = new JsonNetResult(DateTimeKind.Utc) {Data = data};
            return result;
        }

        #endregion

        #region Helpers

        /// <remarks>
        /// Temporary stop-gap so we can learn more about their security token management
        /// </remarks>
        private void OnSessionAuthenticated(Object sender, SessionAuthenticatedEventArgs e)
        {
            if (Debugger.IsAttached) Debugger.Break();

            var id = ((IdentifiedOAuthSession) sender).Id;

            var registration = this.context
                .SetOf<BoxRegistration>()
                .First(r => r.Id == id);

            registration.Update(e.Session);

            context.SaveChanges();
        }

        private async Task<BoxClient> BuildClient(Int32 regId, CancellationToken cancellation)
        {
            // TODO: Get this from the DB
            const String ClientId = "gz0w0uvlrj0tkxw1ra1mn6474gszs98c";
            const String Secret = "N6OobNczf3Pyzwh0Rmz23AKg3PkQEeBp";

            var redirectUrl = Url.Action("Index", "AuthHandler", new {Area = "Box"});
            redirectUrl = $"{Request.Url.Scheme}://{Request.Url.Host}:{Request.Url.Port}{redirectUrl}";

            var config = new BoxConfig(ClientId, Secret, new Uri(redirectUrl));

            var registration = await this.context.SetOf<BoxRegistration>()
                .ForInteractiveUser()
                .Where(r => r.Id == regId)
                .FirstAsync(cancellation)
                .ConfigureAwait(false);

            var session = registration.CreateSession();
            var client = new BoxClient(config, session);
            client.Auth.SessionAuthenticated += OnSessionAuthenticated;
            Trace.WriteLine($"{client.Auth.Session.AccessToken}:{client.Auth.Session.RefreshToken}");

            return client;
        }
        
        #endregion
    }
}