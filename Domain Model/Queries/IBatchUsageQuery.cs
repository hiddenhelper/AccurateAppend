using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core;
using AccurateAppend.Core.Definitions;
using DomainModel.ReadModel;

namespace DomainModel.Queries
{
    /// <summary>
    /// A query component that can provides access to <see cref="JobUsageReport"/> queries.
    /// </summary>
    public interface IBatchUsageQuery
    {
        /// <summary>
        /// Generates a sequence of available operations to perform a report on for a job. The results
        /// of this method can be used to generate specific operation based reports such as <see cref="MatchLevelReportForJob"/>.
        /// </summary>
        /// <remarks>
        /// The set of operations is not guarenteed to be the set of operations contained in the manifest executed
        /// for the job itself. Only operations that contain an actual metric result will be returned. If all
        /// operations are needed, the caller should instead use the manifest operations directly.
        /// </remarks>
        /// <param name="jobId">The identifier of the job to acquire available operations to report on.</param>
        /// <param name="cancellation">The token to monitor for cancellation requests.</param>
        /// <returns>A sequence of <see cref="DataServiceOperation"/> memebers that have available metrics.</returns>
        Task<IEnumerable<DataServiceOperation>> AvailableOperationsForJob(Int32 jobId, CancellationToken cancellation);

        /// <summary>
        /// Generates a sequence of meterics that provide a calculation of the maximum match levels in a particular job for an
        /// <paramref name="operation"/> as a percentage of total number of validations by <see cref="MatchLevel"/> and compared
        /// as a sequence to all other jobs of the user and all other jobs on the system in total (again, by operation).
        /// For each individual metric sequence, the sum of the rate of operations will approach 1 (though due to rounding errors
        /// it may not match or even exceed 100%).
        /// </summary>
        /// <remarks>
        /// The returned set of <see cref="MatchLevel"/> values will not be guarenteed to always have a metric for the
        /// one of the sequences (Job and/or User) as there may be no matches at that level, however, no metrics will be
        /// returned that do not have at least a system value. Supplying an operation that is not contained in the manifest
        /// of the job will not cause an exception or error.
        /// </remarks>
        /// <param name="jobId">The identifier of the job to calculate available operation match level precentages to report on.</param>
        /// <param name="operation">The <see cref="DataServiceOperation"/> to calculate what precentage of matches by match level to generate for.</param>
        /// <param name="cancellation">The token to monitor for cancellation requests.</param>
        /// <returns>A sequence of <see cref="MetricComparison{T}"/> that comapre the spread of indvidual <see cref="MatchLevel"/> results by job, user, and system levels.</returns>
        Task<IEnumerable<MetricComparison<MatchLevel>>> MatchLevelReportForJob(Int32 jobId, DataServiceOperation operation, CancellationToken cancellation);

        /// <summary>
        /// Generates a sequence of meterics that provide a calculation of the quality levels in a particular job for an
        /// <paramref name="operation"/> as a percentage of total groupings by <see cref="QualityLevel"/> and compared
        /// as a sequence to all other jobs of the user and all other jobs on the system in total (again, by operation).
        /// For each individual metric sequence, the sum of the rate of operations will approach 1 (though due to rounding errors
        /// it may not match or even exceed 100%).
        /// </summary>
        /// <remarks>
        /// The returned set of <see cref="QualityLevel"/> values will not be guarenteed to always have a metric for the
        /// one of the sequences (Job and/or User) as there may be no matches at that level, however, no metrics will be
        /// returned that do not have at least a system value. Supplying an operation that is not contained in the manifest 
        /// of the job will not cause an exception or error. 
        /// </remarks>
        /// <param name="jobId">The identifier of the job to calculate available operation quality level precentages to report on.</param>
        /// <param name="operation">The <see cref="DataServiceOperation"/> to calculate what precentage of matches by quality level to generate for.</param>
        /// <param name="cancellation">The token to monitor for cancellation requests.</param>
        /// <returns>A sequence of <see cref="MetricComparison{T}"/> that comapre the spread of indvidual <see cref="QualityLevel"/> results by job, user, and system levels.</returns>
        Task<IEnumerable<MetricComparison<QualityLevel>>> QualityLevelReportForJob(Int32 jobId, DataServiceOperation operation, CancellationToken cancellation);

        /// <summary>
        /// Generates a sequence of meterics that provide a calculation of the CASS parsing statues in a particular job as a
        /// percentage of total groupings by <see cref="CassStatus"/> and compared as a sequence to all other jobs of the user
        /// and all other jobs on the system in total. For each individual metric sequence, the sum of the rate of statuses
        /// will approach 1 (though due to rounding errors it may not match or even exceed 100%).
        /// </summary>
        /// <remarks>
        /// The returned set of <see cref="CassStatus"/> values will not be guarenteed to always have a metric for the
        /// one of the sequences (Job and/or User) as there may be no values at that level, however, no metrics will be
        /// returned that do not have at least a system value.
        /// 
        /// Parsing occurs at the file level not the operation.
        /// </remarks>
        /// <param name="jobId">The identifier of the job to calculate CASS parsing status level precentages to report on.</param>
        /// <param name="cancellation">The token to monitor for cancellation requests.</param>
        /// <returns>A sequence of <see cref="MetricComparison{T}"/> that comapre the spread of indvidual <see cref="CassStatus"/> results by job, user, and system levels.</returns>
        Task<IEnumerable<MetricComparison<CassStatus>>> CassReportForJob(Int32 jobId, CancellationToken cancellation);
    }

    /// <summary>
    /// Provides a ratio based metric comparing file to user to system level rates for each scope.
    /// </summary>
    /// <remarks>
    /// The individual instance of this metric do not compare metrics between scopes but instead
    /// are used as part of a sequence of metrics keyed via a value. Each value per metric for a
    /// single scope woul be used in contrast to the entire sequence.
    /// </remarks>
    /// <typeparam name="TMetric">The type of metric being measured.</typeparam>
    public class MetricComparison<TMetric>
    {
        /// <summary>
        /// Gets or sets the metric value being calculated.
        /// </summary>
        public TMetric Metric { get; set; }

        /// <summary>
        /// Gets or sets the rate on the file level scope.
        /// </summary>
        public Double File { get; set; }

        /// <summary>
        /// Gets or sets the rate on the user level scope.
        /// </summary>
        public Double User { get; set; }

        /// <summary>
        /// Gets or sets the rate on the system level scope.
        /// </summary>
        public Double System { get; set; }
    }

    /// <summary>
    /// Provides a ratio based metric comparing the spread file to user to system level rates for each scope.
    /// </summary>
    public class CassStatusComparison
    {
        /// <summary>
        /// Gets the 
        /// </summary>
        public String Type { get; set; }
        public double F { get; set; }
        public double P { get; set; }
        public double S { get; set; }
        public double U { get; set; }
        public double C { get; set; }
    }

    public class BatchUsageQuery : IBatchUsageQuery
    {
        #region Fields

        private readonly ReadContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BatchUsageQuery"/> class.
        /// </summary>
        public BatchUsageQuery(ReadContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region IBatchUsageQuery Members

        /// <inheritdoc />
        public virtual Task<IEnumerable<DataServiceOperation>> AvailableOperationsForJob(Int32 jobId, CancellationToken cancellation)
        {
            #region sql statement

            const String Sql = @"
                DECLARE @jobid int = @p0
                DECLARE @temp table([OperationId] int)

                INSERT INTO @temp
                SELECT [OperationId]
                FROM [jobs].[JobSliceMaxValidationLevelFact] (nolock)
                WHERE [JobId] = @jobid
                UNION
                SELECT [OperationId]
                FROM [jobs].[JobSliceMatchLevelFact] (nolock)
                WHERE [JobId] = @jobid
                UNION
                SELECT [OperationId]
                FROM [jobs].[JobSliceQualityLevelFact] (nolock)
                WHERE [JobId] = @jobid

                SELECT DISTINCT [OperationId] FROM @temp";
            #endregion

            var result = this.context.Database
                .SqlQuery<DataServiceOperation>(Sql, jobId)
                .ToListAsync(cancellation)
                .ContinueWith(t => (IEnumerable<DataServiceOperation>) t.Result, TaskContinuationOptions.ExecuteSynchronously);

            return result;
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<MetricComparison<MatchLevel>>> MatchLevelReportForJob(Int32 jobId, DataServiceOperation operation, CancellationToken cancellation)
        {
            #region sql statement
            const String Sql = @"
                DECLARE @jobId int = @p0
                DECLARE @userId uniqueidentifier
                DECLARE @operationId int = @p1

                SELECT @userId = [UserId] FROM [jobs].[JobQueue] (nolock) WHERE [JobId] = @JobId

                DECLARE @file table( MatchLevel int, MatchLevelRate float )
                DECLARE @user table ( MatchLevel int, MatchLevelRate float )
                DECLARE @system table( MatchLevel int, MatchLevelRate float )
                DECLARE @dim table(MatchLevel int)

                INSERT @file
                exec [jobs].[GetMaxValidationLevelRate] @jobId, null, @operationId
                INSERT @user
                exec [jobs].[GetMaxValidationLevelRate] null, @userId, @operationId
                INSERT @system
                exec [jobs].[GetMaxValidationLevelRate] null, null, @operationId

                INSERT INTO @dim 
                SELECT [MatchLevel]
                FROM @file
                UNION 
                SELECT [MatchLevel]
                FROM @user 
                UNION
                SELECT [MatchLevel]
                FROM @system;

                SELECT d.[MatchLevel] [Metric]
                , isnull(f.[MatchLevelRate],0) [File]
                , isnull(u.[MatchLevelRate],0) [User]
                , isnull(s.[MatchLevelRate],0) [System] 
                FROM @dim d 
                LEFT JOIN @file f ON d.[MatchLevel] = f.[MatchLevel]
                LEFT JOIN @user u ON d.[MatchLevel] = u.[MatchLevel]
                LEFT JOIN @system s ON d.[MatchLevel] = s.[MatchLevel]
                WHERE d.[MatchLevel] != 0;";
            #endregion

            var result = this.context.Database
                .SqlQuery<MetricComparison<MatchLevel>>(Sql, jobId, (Int32) operation)
                .ToListAsync(cancellation)
                .ContinueWith(t => (IEnumerable<MetricComparison<MatchLevel>>) t.Result, TaskContinuationOptions.ExecuteSynchronously);

            return result;
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<MetricComparison<MatchLevel>>> MaxValidationLevelReportForJob(Int32 jobId, DataServiceOperation operation, CancellationToken cancellation)
        {
            #region sql statement
            const String Sql = @"
                DECLARE @jobId int = @p0
                DECLARE @userId uniqueidentifier
                DECLARE @operationId int = @p1

                SELECT @userId = [UserId] FROM [jobs].[JobQueue] WHERE [JobId] = @JobId

                DECLARE @file table( MatchLevel int, MatchLevelRate float )
                DECLARE @user table ( MatchLevel int, MatchLevelRate float )
                DECLARE @system  table( MatchLevel int, MatchLevelRate float )
                DECLARE @dim table(MatchLevel int)

                INSERT  @file
                exec [jobs].[GetMaxValidationLevelRate] @jobId, null, @operationId
                INSERT @user
                exec [jobs].[GetMaxValidationLevelRate] null, @userId, @operationId
                INSERT @system
                exec [jobs].[GetMaxValidationLevelRate] null, null, @operationId

                INSERT INTO @dim 
                SELECT MatchLevel
                FROM @file
                UNION 
                SELECT [MatchLevel]
                FROM @user 
                UNION
                SELECT [MatchLevel]
                FROM @system;

                SELECT d.[MatchLevel] [Metric]
                , isnull(f.[MatchLevelRate],0) [File]
                , isnull(u.[MatchLevelRate],0) [User]
                , isnull(s.[MatchLevelRate],0) [System] 
                FROM @dim d 
                LEFT JOIN @file f on d.[MatchLevel] = f.[MatchLevel]
                LEFT JOIN @user u on d.[MatchLevel] = u.[MatchLevel]
                LEFT JOIN @system s on d.[MatchLevel] = s.[MatchLevel]
                WHERE d.[MatchLevel] != 0;";
            #endregion

            var result = this.context.Database
                .SqlQuery<MetricComparison<MatchLevel>>(Sql, jobId, (Int32)operation)
                .ToListAsync(cancellation)
                .ContinueWith(t => (IEnumerable<MetricComparison<MatchLevel>>)t.Result, TaskContinuationOptions.ExecuteSynchronously);

            return result;
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<MetricComparison<QualityLevel>>> QualityLevelReportForJob(Int32 jobId, DataServiceOperation operation, CancellationToken cancellation)
        {
            #region sql statement

            const String Sql = @"
                DECLARE @jobId int = @p0
                DECLARE @userId uniqueidentifier
                DECLARE @operationId int = @p1

                SELECT @userId = [UserId] FROM [jobs].[JobQueue] WHERE JobId = @JobId

                DECLARE @file table( QualityLevel char(1), QualityLevelRate float )
                DECLARE @user table ( QualityLevel char(1), QualityLevelRate float )
                DECLARE @system  table( QualityLevel char(1), QualityLevelRate float )
                DECLARE @dim table(QualityLevel char(1))

                INSERT @file
                exec [jobs].[GetQualityLevelRate] @jobId, null, @operationId
                INSERT @user
                exec [jobs].[GetQualityLevelRate] null, @userId, @operationId
                INSERT @system
                exec [jobs].[GetQualityLevelRate] null, null, @operationId

                INSERT INTO @dim
                SELECT [QualityLevel] FROM @file
                UNION
                SELECT [QualityLevel] FROM @user
                UNION
                SELECT [QualityLevel] FROM @system

                SELECT d.[QualityLevel] [Metric]
                , isnull(f.[QualityLevelRate],0) [File]
                , isnull(u.[QualityLevelRate],0) [User]
                , isnull(s.[QualityLevelRate],0) [System] 
                FROM @dim d 
                LEFT JOIN @file f ON d.[QualityLevel] = f.[QualityLevel]
                LEFT JOIN @user u ON d.[QualityLevel] = u.[QualityLevel]
                LEFT JOIN @system s ON d.[QualityLevel] = s.[QualityLevel];";

            #endregion

            var result = this.context.Database
                .SqlQuery<MetricComparison<String>>(Sql, jobId, (Int32)operation)
                .ToListAsync(cancellation)
                .ContinueWith(t => t.Result.Select(m =>
                    new MetricComparison<QualityLevel>()
                    {
                        Metric = EnumExtensions.Parse<QualityLevel>(m.Metric),
                        File = m.File,
                        User = m.User,
                        System = m.System
                    }
                ), TaskContinuationOptions.ExecuteSynchronously);

            return result;
        }

        /// <inheritdoc />
        public virtual Task<IEnumerable<MetricComparison<CassStatus>>> CassReportForJob(Int32 jobId, CancellationToken cancellation)
        {
            #region sql statement

            const String Sql = @"
                DECLARE @jobId int = @p0
                DECLARE @userId uniqueidentifier

                SELECT @userId = [UserId] FROM [jobs].[JobQueue] WHERE [JobId] = @JobId

                DECLARE @file table( [CassStatus] char(1), CassStatusRate float )
                DECLARE @user table ( [CassStatus] char(1), CassStatusRate float )
                DECLARE @system  table( [CassStatus] char(1), CassStatusRate float )
                DECLARE @dim table( [CassStatus] char(1))

                INSERT @file
                exec [jobs].[GetCassStatusRate] @jobId, null
                INSERT @user
                exec [jobs].[GetCassStatusRate] null, @userId
                INSERT @system
                exec [jobs].[GetCassStatusRate] null, null

                INSERT INTO @dim
                SELECT [CassStatus] FROM @file
                UNION
                SELECT [CassStatus] FROM @user
                UNION
                SELECT [CassStatus] FROM @system

                SELECT d.[CassStatus] [Metric]
                , isnull(f.[CassStatusRate],0) [File]
                , isnull(u.[CassStatusRate],0) [User]
                , isnull(s.[CassStatusRate],0) [System] 
                FROM @dim d 
                LEFT JOIN @file f ON d.[CassStatus] = f.[CassStatus]
                LEFT JOIN @user u ON d.[CassStatus] = u.[CassStatus]
                LEFT JOIN @system s ON d.[CassStatus] = s.[CassStatus];";

            #endregion

            var result = this.context.Database
                .SqlQuery<MetricComparison<String>>(Sql, jobId)
                .ToListAsync(cancellation)
                .ContinueWith(t => t.Result.Select(m =>
                    new MetricComparison<CassStatus>()
                    {
                        Metric = EnumExtensions.Parse<CassStatus>(m.Metric),
                        File = m.File,
                        User = m.User,
                        System = m.System
                    }
                ), TaskContinuationOptions.ExecuteSynchronously);

            return result;
        }

        #endregion
    }
}
