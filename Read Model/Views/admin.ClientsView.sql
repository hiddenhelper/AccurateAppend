CREATE VIEW [admin].[ClientsView]
AS

SELECT 
	ud.UserId,
	Max(u.ApplicationId) [ApplicationId],
	Max(ud.UserDetailId) [UserDetailId],
	Max(u.UserName) [UserName], 
	Max(ud.FirstName) [FirstName], 
	Max(ud.LastName) [LastName], 
	Coalesce(Sum(d.Amount), 0) [LifeTimeRevenue], 
	Max(u.LastActivityDate) [LastActivityDate],
	Max(ud.BusinessName) [BusinessName],
	Max(ud.[Address]) [Address],
	Max(ud.City) [City],
	Max(ud.[State]) [State],
	Max(ud.Zip) [Zip],
	Max(ud.Phone) [Phone],
	Coalesce(Max(l.[LeadSource]), 7) [LeadSource], --Unknown
	[IsSubscriber] =
	CASE
		WHEN Count(s.SubscriptionId) > 0 THEN Convert(bit,1)
		ELSE Convert(bit,0)
	END,
	[Status] =
	CASE
		WHEN DATEDIFF(day, Min(u.LastActivityDate), GetDate()) < 60 THEN 1
		ELSE 2
	END
FROM [accounts].[UserDetail] ud (nolock)
INNER JOIN [dbo].[aspnet_Users] u (nolock) on u.[UserId]=ud.[UserId]
LEFT JOIN [accounts].[Leads] l (nolock) on l.[LeadId]=ud.[SourceLeadId]
LEFT JOIN [sales].[Subscriptions] s (nolock) on s.[UserId]=u.[UserId] AND s.[StartDate]<=GETDATE() AND (s.[EndDate] IS NULL OR s.[EndDate] > GetDate())
LEFT JOIN [sales].[Deals] d (nolock) on d.[UserId]=ud.[UserId] AND d.[Status]=5 --Complete

GROUP BY ud.[UserId]