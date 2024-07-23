CREATE VIEW [admin].[RunningJobs]
AS
SELECT
	j.JobId,
	u.UserId,
	j.DateSubmitted,
	j.DateUpdated,
	j.DateComplete,
	j.[Priority],
	j.InputFileName,
	j.InputFileSize,
	j.TotalRecords,
	j.ProcessedRecords,
	j.MatchRecords,
	j.SystemErrors,
	j.[Status],
	j.MillisecondsPerRecord,
	j.Product,
	j.ColumnMap,
	j.CustomerFileName,
	j.Source,
	u.UserName,
	(SELECT Count(*) FROM [jobs].[JobSliceQueue] (nolock) WHERE JobId = j.JobId) [TotalSlices],
	(SELECT Coalesce(Sum(TotalRecords), 0) FROM jobs.JobSliceQueue (nolock) WHERE JobId = j.JobId AND [Status] = 1 AND DateComplete >= DateAdd(minute, -3, GETDATE())) [RecentProcessedRecords],
	(SELECT Coalesce(Min(DateComplete), j.DateSubmitted) FROM jobs.JobSliceQueue (nolock) WHERE JobId = j.JobId AND [Status] = 1) [FirstCompleteSlice]
FROM [jobs].[JobQueue] j (nolock)
INNER JOIN dbo.aspnet_Users u (nolock) ON u.UserId=j.UserId