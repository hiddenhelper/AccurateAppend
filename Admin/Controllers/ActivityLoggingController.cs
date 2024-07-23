using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Data;
using AccurateAppend.Security;
using AccurateAppend.Websites.Admin.Messages.Admin;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Controllers
{
    /// <summary>
    /// Provides a controller with convenence methods for logging to the activity log.
    /// </summary>
    public abstract class ActivityLoggingController : ContextBoundController
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityLoggingController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="ISessionContext"/> to use for this controller instance.</param>
        protected ActivityLoggingController(ISessionContext context) : base(context)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Performs an imperative call to the activity logging API.
        /// </summary>
        /// <param name="description">The description of the event to record.</param>
        protected virtual async Task LogEventAsync(String description)
        {
            var user = await this.Context.CurrentUserAsync().ConfigureAwait(false);

            await this.LogEventAsync(description, user);
        }

        /// <summary>
        /// Performs an imperative call to the activity logging API.
        /// </summary>
        /// <param name="description">The description of the event to record.</param>
        /// <param name="args">An array of <see cref="Object"/> containing the ordinal format parameters, if any.</param>
        protected virtual Task LogEventAsync(String description, params Object[] args)
        {
            if (description == null) throw new ArgumentNullException(nameof(description));
            if (args == null) throw new ArgumentNullException(nameof(args));

            Contract.EndContractBlock();

            return this.LogEventAsync(String.Format(CultureInfo.InvariantCulture, description, args));
        }

        /// <summary>
        /// Performs an imperative call to the activity logging API for the given user.
        /// </summary>
        /// <param name="description">The description of the event to record.</param>
        /// <param name="performedBy">The user the logged event will be for.</param>
        protected virtual Task LogEventAsync(String description, User performedBy)
        {
            var message = new LogUserActionCommand
            {
                Description = description,
                EventDate = DateTime.UtcNow,
                UserId = this.User.Identity.GetIdentifier()
            };

            return MvcApplication.Container.Resolve<IMessageSession>().Send(message);
        }

        #endregion
    }

    /// <summary>
    /// Provides a controller with convenence methods for logging to the activity log.
    /// </summary>
    public abstract class ActivityLoggingController2 : Controller
    {
        #region Methods

        /// <summary>
        /// Performs an imperative call to the activity logging API.
        /// </summary>
        /// <param name="description">The description of the event to record.</param>
        protected virtual void OnEvent(String description)
        {
            var userId = Thread.CurrentPrincipal.Identity.GetIdentifier();
            var ip = this.Request?.UserHostAddress;

            var args = new UserActvityEventArgs();
            args.UserId = userId;
            args.ActivityDescription = description;
            args.Ip = ip == null ? IPAddress.None : IPAddress.Parse(ip);

            this.UserActivity?.Invoke(this, args);
        }

        #endregion

        #region Events

        /// <summary>
        /// The event raised when a logged user event occurs.
        /// </summary>
        public event EventHandler<UserActvityEventArgs> UserActivity;

        #endregion
    }

    public class UserActvityEventArgs : EventArgs
    {
        public Guid UserId { get; set; }

        public String ActivityDescription { get; set; }

        public IPAddress Ip { get; set; }
    }
}