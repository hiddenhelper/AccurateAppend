CREATE VIEW admin.JobSliceProcessingView
AS
SELECT
	s.JobId,
	s.JobSliceId,
	s.DateSubmitted,
	s.Status,
	CASE WHEN s.Status = 1 THEN
		s.DateUpdated
	ELSE
		NULL
	END [DateStarted],
	CASE WHEN s.Status = 1 THEN
		s.DateComplete
	ELSE
		NULL
	END [DateComplete],
	s.InputFileName,
	Convert(uniqueidentifier, s.InputFileName) [CorrelationId],
	s.TotalRecords,
	s.ProcessedRecords,
	CASE WHEN s.Status = 1 THEN
		s.Processor
	ELSE
		sys.SystemName
	END [Processor]
FROM [jobs].[JobSliceQueue] s (nolock)
LEFT JOIN operations.Systems sys (nolock) on s.LockId = sys.InstanceId