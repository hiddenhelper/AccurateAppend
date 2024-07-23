CREATE VIEW [admin].[DealsView]
AS

SELECT
d.[DealId],
d.[Amount],
d.[DateAdded],
d.[Description],
d.[Status],
d.[Title],
d.[ProcessingInstructions],
d.[DateComplete],
ud.[BusinessName],
ud.[FirstName],
ud.[LastName],
u.[UserId],
u.[UserName],
a.[ApplicationId],
o.[UserId] [OwnerId],
o.[UserName] [OwnerName],
COALESCE(s.[ApplicationName], '') [ApplicationName],
(SELECT [EnableAutoBill] FROM [sales].[Orders] WHERE [OrderType]=0 AND [DealId]=d.[DealId]) [EnableAutoBill]
FROM [sales].[Deals] d (nolock)
INNER JOIN [accounts].[UserDetail] ud (nolock) ON d.[UserId] = ud.[UserId]
INNER JOIN [dbo].[aspnet_Users] u (nolock) ON u.[UserId] = ud.[UserId]
INNER JOIN [dbo].[aspnet_Applications] a (nolock) ON a.[ApplicationId] = u.[ApplicationId]
LEFT JOIN [security].[Sites] s (nolock) ON s.[ApplicationId] = a.[ApplicationId]
INNER JOIN [dbo].[aspnet_Users] o (nolock) ON o.[UserId] = d.[Creator_UserId]