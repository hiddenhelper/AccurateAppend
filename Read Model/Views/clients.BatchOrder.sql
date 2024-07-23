CREATE VIEW [clients].[BatchOrder]
AS

SELECT
	Convert(uniqueidentifier, j.[InputFileName]) [OrderId],
	j.[JobId] [RequestId], 
	j.[Source],
	j.[Status] [JobStatus]
FROM [jobs].[JobQueue] j (nolock)
WHERE j.[Source] IN (2, 3 ,4) -- FTP, SMTP, Admin