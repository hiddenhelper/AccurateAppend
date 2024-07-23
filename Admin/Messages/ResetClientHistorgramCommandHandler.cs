using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using AccurateAppend.Accounting;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Messaging;
using DomainModel.Messages;
using EventLogger;
using NServiceBus;
using Application = AccurateAppend.Core.Definitions.Application;

namespace AccurateAppend.Websites.Admin.Messages
{
    /// <summary>
    /// Handler for the <see cref="ResetClientHistorgramCommand"/> bus message.
    /// </summary>
    /// <remarks>
    /// Responds to a message by deleting the previous client state historgram
    /// and then recreated base on the current <see cref="ServiceAccount"/> with
    /// an externally supplied <see cref="ISessionContext"/> data component.
    /// </remarks>
    public class ResetClientHistorgramCommandHandler : IHandleMessages<ResetClientHistorgramCommand>
    {
        #region Fields

        private readonly AccurateAppend.JobProcessing.DataAccess.DefaultContext dataContext;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ResetClientHistorgramCommandHandler"/> class.
        /// </summary>
        /// <param name="dataContext">The required <see cref="AccurateAppend.JobProcessing.DataAccess.DefaultContext"/> component.</param>
        public ResetClientHistorgramCommandHandler(AccurateAppend.JobProcessing.DataAccess.DefaultContext dataContext)
        {
            if (dataContext == null) throw new ArgumentNullException(nameof(dataContext));
            Contract.EndContractBlock();

            this.dataContext = dataContext;
            this.dataContext.Database.CommandTimeout = Convert.ToInt32(TimeSpan.FromMinutes(5).TotalSeconds);
        }

        #endregion

        #region IHandleMessages<ResetClientHistorgramCommand> Members

        /// <inheritdoc />
        public virtual async Task Handle(ResetClientHistorgramCommand message, IMessageHandlerContext context)
        {
            var id = context.DefaultCorrelation();

            var userId = message.UserId;
            Logger.LogEvent($"resetting histogram for {userId}", Severity.None, Application.AccurateAppend_Admin);

            using (new Correlation(id))
            {
                try
                {
                    await this.dataContext.Database.ExecuteSqlCommandAsync(@"exec [accounts].[ResetClientHistogram] @p0", userId);
                }
                catch (Exception ex)
                {
                    if (Debugger.IsAttached) Debugger.Break();
                    Logger.LogEvent(ex, Severity.High, Application.AccurateAppend_Admin, description: "Failure on Client Histogram generation");

                    throw;
                }
            }
        }

        #endregion
    }
}