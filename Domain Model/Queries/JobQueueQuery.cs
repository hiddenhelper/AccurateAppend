using System;
using System.Diagnostics.Contracts;
using System.Linq;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Security;
using DomainModel.ReadModel;

namespace DomainModel.Queries
{
    public class JobQueueQuery : IJobQueueQuery
    {
        #region Fields

        private readonly ISessionContext context;

        #endregion

        #region Constructor

        public JobQueueQuery(ISessionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region Methods

        protected virtual IQueryable<JobQueueView> CompletedDuring(DateTime startOn, DateTime endBy)
        {
            var query = this.context.SetOf<JobQueueView>()
                .Where(j => j.Status == JobStatus.Complete)
                .Where(j => j.DateComplete >= startOn && j.DateComplete <= endBy);

            return query;
        }

        #endregion

        #region IJobQueueQuery Members

        public virtual IQueryable<JobQueueView> CompletedDuring(Guid userId, DateTime startOn, DateTime endBy)
        {
            var query = this.CompletedDuring(startOn, endBy)
                .Where(j => j.UserId == userId);

            return query;
        }

        public virtual IQueryable<JobQueueView> CompletedDuring(String userName, DateTime startOn, DateTime endBy)
        {
            var query = this.CompletedDuring(startOn, endBy)
                .Where(j => j.UserName == userName);

            return query;
        }

        public virtual IQueryable<JobQueueSummary> CompletedSummary(Guid applicationId, DateTime startOn, DateTime endBy)
        {
            var query = this.CompletedDuring(startOn, endBy)
                .Where(j => j.ApplicationId == applicationId || j.ApplicationId == ApplicationExtensions.AdminId);

            var final = query.GroupBy(j => j.UserId).Select(g => new JobQueueSummary
            {
                FileCount = g.Count(),
                LastActivity = g.Max(j => j.DateComplete),
                MatchCount = g.Sum(j => j.MatchCount),
                RecordCount = g.Sum(j => j.RecordCount),
                UserId = g.Key,
                UserName = g.FirstOrDefault().UserName,
            });

            return final;
        }
        
        public virtual IQueryable<JobQueueView> InProgress(Guid applicationId)
        {
            var query = this.context.SetOf<JobQueueView>()
                .Where(j => j.Status != JobStatus.Complete)
                .Where(j => j.ApplicationId == applicationId || j.ApplicationId == ApplicationExtensions.AdminId);

            return query;
        }
        
        public virtual IQueryable<JobQueueView> SearchDuring(Guid applicationId, String searchTerm, DateTime startOn, DateTime endBy)
        {
            var query = this.context.SetOf<JobQueueView>()
                .Where(j => j.ApplicationId == applicationId || j.ApplicationId == ApplicationExtensions.AdminId)
                .Where(j => j.CustomerFileName.Contains(searchTerm) || j.UserName.Contains(searchTerm) || j.JobId.ToString() == searchTerm)
                .OrderByDescending(d => d.DateSubmitted)
                .Take(50);

            return query;
        }
        #endregion
    }
}