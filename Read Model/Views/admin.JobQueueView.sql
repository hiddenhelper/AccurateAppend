CREATE VIEW [admin].[JobQueueView]
AS

SELECT
	j.JobId,
	j.TotalRecords [RecordCount],
	j.MatchRecords [MatchCount],
	j.ProcessedRecords [ProcessedCount],
	j.DateComplete,
	j.DateUpdated,
	j.DateSubmitted,
	j.CustomerFileName,
	j.InputFileName,
	a.ApplicationId,
	u.UserId,
	u.UserName,
	j.[Status],
	j.[Source],
	j.[Product],
	j.[Priority],
	[jobs].[CalculateProcessingRate](j.JobId, j.ProcessedRecords, j.TotalRecords) [ProcessingRate],
	CONVERT(bit, (1 ^ IsNumeric([ProcessingCost]))) [IsPaused]
FROM [jobs].[JobQueue] (nolock) j
INNER JOIN [dbo].[aspnet_Users] (nolock) u ON u.UserId = j.UserId
INNER JOIN [dbo].[aspnet_Applications] (nolock) a ON a.ApplicationId = u.ApplicationId