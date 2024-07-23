CREATE VIEW [clients].[DirectClientOrder]
AS

SELECT 
	po.[OrderId],
	j.[JobId] [RequestId], 
	5 [Source], -- Client
	Coalesce(j.[Status], 5) [JobStatus] -- InQueue
FROM [sales].[ProductOrder] po (nolock)
LEFT JOIN jobs.[JobQueue] j (nolock) ON j.[InputFileName] = po.[OrderId] AND j.[Source] = 5 -- Client
WHERE po.[Source] = 5 -- Client