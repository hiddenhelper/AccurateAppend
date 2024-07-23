CREATE VIEW [admin].[JobReportView]
AS

SELECT
	j.JobId,
	j.DateSubmitted,
	j.DateComplete,
	j.CustomerFileName,
	j.Report,
	u.UserName,
	u.UserId,
	j.TotalRecords
FROM [jobs].[JobQueue] j (nolock)
INNER JOIN [dbo].[aspnet_Users] u (nolock)
ON j.[UserId] = u.[UserId]
INNER JOIN [accounts].[UserDetail] c (nolock)
ON j.[UserId] = c.[UserId]
WHERE j.[Status] = 1 -- Complete

