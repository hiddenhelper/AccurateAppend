CREATE VIEW clients.Orders
AS

--batch jobs
SELECT Convert(uniqueidentifier, j.[InputFileName]) [OrderId], j.[JobId] [RequestId], 0 [Type], u.[UserId], [DateSubmitted], j.[Status], [CustomerFileName] [Name], [TotalRecords], [CustomerFileName], [InputFileName] [SystemFileName],
CASE
	WHEN j.[Status] = 1 THEN 3 -- Complete/Available
	ELSE 1 -- Processing
END [OrderStatus],
Convert(uniqueidentifier,o.[PublicKey]) [BillId]
FROM [jobs].[JobQueue] j (nolock)
INNER JOIN [dbo].[aspnet_Users] u (nolock) ON j.[UserId] = u.[UserId]
LEFT JOIN [sales].[Orders] o (nolock) ON o.[PublicKey] = j.[PublicKey]
WHERE j.[Source] IN (2, 3 ,4) -- FTP, SMTP, Admin

UNION

--NB jobs
SELECT po.[OrderId], p.[NationBuilderPushId] [RequestId], 1 [Type], ud.UserId, p.RequestDate [DateSubmitted], p.[Status], po.[ListName] [Name], p.TotalRecords, po.[ListName] [CustomerFileName], Convert(varchar(250), po.[OrderId]) [SystemFileName], po.[Status] [OrderStatus], Convert(uniqueidentifier,o.[PublicKey])
FROM [sales].[ProductOrder] po (nolock)
LEFT JOIN [sales].[Orders] o (nolock) ON o.[PublicKey] = po.[OrderId]
INNER JOIN [integration].[NationBuilderPush] p (nolock) ON p.[SupressionId] = po.[OrderId]
INNER JOIN [integration].[NationBuilderRegistration] r (nolock) ON p.[NationBuilderRegistrationId] = r.[NationBuilderRegistrationId]
INNER JOIN [accounts].[UserDetail] ud (nolock) ON r.[UserDetailId] = ud.[UserDetailId]
INNER JOIN [dbo].[aspnet_Users] u (nolock) ON ud.[UserId] = u.[UserId]


UNION

-- Client jobs
SELECT po.[OrderId], j.[JobId] [RequestId], 2 [Type], u.[UserId], po.[DateSubmitted], j.[Status], po.[ListName] [Name], po.TotalRecords, po.[ListName] [CustomerFileName], Convert(varchar(250), po.[OrderId]) [SystemFileName], po.[Status] [OrderStatus], Convert(uniqueidentifier,o.[PublicKey])
FROM [sales].[ProductOrder] po (nolock)
LEFT JOIN [sales].[Orders] o (nolock) ON o.[PublicKey] = po.[OrderId]
INNER JOIN [dbo].[aspnet_Users] u (nolock) ON u.[UserId] = po.[UserId]
LEFT JOIN [jobs].[JobQueue] j (nolock) ON j.[InputFileName] = po.[OrderId] AND j.[Source] = 5 -- Client
WHERE po.[Source] = 5 -- Client