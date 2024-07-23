using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using AccurateAppend.Accounting;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Messaging;
using DomainModel.Messages;
using EventLogger;
using NServiceBus;
using Application = AccurateAppend.Core.Definitions.Application;
using AccurateAppend.Security.FTPAdministration;
using System.IO;
using AccurateAppend.JobProcessing;
using AccurateAppend.Security;

namespace AccurateAppend.Websites.Admin.Messages
{
    /// <summary>
    /// Handler for the <see cref="ConfigureFtpAccountCommand"/> bus message.
    /// </summary>
    /// <remarks>
    /// Responds to a message by deleting the previous client state historgram
    /// and then recreated base on the current <see cref="ServiceAccount"/> with
    /// an externally supplied <see cref="ISessionContext"/> data component.
    /// </remarks>
    public class ConfigureFtpAccountCommandHandler : IHandleMessages<ConfigureFtpAccountCommand>
    {
        #region Fields

        private readonly AccurateAppend.JobProcessing.DataAccess.DefaultContext dataContext;
        private readonly IFtpHost server;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ResetClientHistorgramCommandHandler"/> class.
        /// </summary>
        /// <param name="dataContext">The required <see cref="AccurateAppend.JobProcessing.DataAccess.DefaultContext"/> component.</param>
        /// <param name="server">The <see cref="IFtpHost"/> server component providing access to the FTP system.</param>
        public ConfigureFtpAccountCommandHandler(AccurateAppend.JobProcessing.DataAccess.DefaultContext dataContext, IFtpHost server)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            if (server == null) throw new ArgumentNullException(nameof(server));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
            this.dataContext.Database.CommandTimeout = Convert.ToInt32(TimeSpan.FromMinutes(5).TotalSeconds);
            this.server = server;
        }

        #endregion

        #region IHandleMessages<ResetClientHistorgramCommand> Members

        /// <inheritdoc />
        public virtual async Task Handle(ConfigureFtpAccountCommand message, IMessageHandlerContext context)
        {
            Trace.TraceInformation($"Handling ConfigureFtpAccount {message.UserName}");

            var id = context.DefaultCorrelation();

            var username = CleanPath(message.UserName); // must clean ahead of time for name equality checks to be meaningful.

            try
            {
                var ftpAcct = await this.dataContext
                    .FtpAccounts
                    .Where(a => a.Logon.Id == message.UserId)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                var logon = await this.dataContext
                    .SetOf<User>()
                    .SingleAsync(u => u.Id == message.UserId);

                if (ftpAcct != null) // user already has Ftp Batch Account
                {
                    // make sure FtpBatchAccount username matches indicated username. Do no work otherwise.
                    if (!String.Equals(ftpAcct.UserName, username, StringComparison.OrdinalIgnoreCase)
                    ) // Username specified on FTP setup screen is incorrect. Do no work. No exception.
                    {
                        // should this be an exception instead of just a log?
                        Logger.LogEvent($"FtpBatchAccount name mismatch for {message.UserId}", Severity.Low, Application.AccurateAppend_Admin, description: $"{message.UserId} already has FtpBatchAccount {username}, cannot use {ftpAcct.UserName}");
                        return;
                    }

                    // update titan
                    server.ResetPassword(username, message.Password);
                    server.Switch(username, message.Enabled);

                    // update database
                    ftpAcct.ChangePassword(message.Password);
                }
                else // no ftp account, create from scratch.
                {
                    // check db table to see if desired username is in use by a different customer
                    if (this.dataContext.FtpAccounts.Any(a => a.UserName == username))
                    {
                        // should this be an exception instead of just a log? Exception would let it sit in queue for us to fix.

                        // yeah I had thought the same but there's nothing to fix and rerun the message after I thought about it more- we'd simply have to delete the message from the error queue.
                        // probably a better idea to perhaps send an email at the end to the request initiator explaining the final report on the action taken? use the MessageContext extensions to get the initiator username
                        Logger.LogEvent($"Ftp user {username} not available", Severity.Medium, Application.AccurateAppend_Admin, description: $"Cannot assign Ftp name {username} (to {message.UserId}), already in use.");
                        return;
                    }

                    // delete preexisting from previous setup attempt.  (cannot be from another user because of above check).
                    if (server.UserExists(username)) server.DeleteUser(username);

                    server.CreateUser(username, message.Password, message.Enabled);

                    ftpAcct = new FtpBatchAccount(logon, username, message.Password);

                    this.dataContext.FtpAccounts.Add(ftpAcct);
                }

                // Set the status to the indicated value, if the user isn't locked out
                if (message.Enabled && !logon.IsLockedOut)
                {
                    ftpAcct.Enable();
                }
                else
                {
                    ftpAcct.Disable();
                }

                await this.dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                if (Debugger.IsAttached) Debugger.Break();

                throw;
            }
        }

        #endregion

        #region helpers

        /// <summary>
        /// Returns string cleansed of any invalid characters
        /// </summary>
        /// <param name="path">String to cleanse</param>
        /// <returns>cleansed string</returns>
        private String CleanPath(String path)
        {
            return new String(path.Trim().Where(c => !Path.GetInvalidPathChars().Contains(c)).ToArray());
        }

        #endregion
    }
}

