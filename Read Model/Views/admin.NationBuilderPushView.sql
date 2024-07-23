CREATE VIEW [admin].[NationBuilderPushView]
AS

SELECT
	p.NationBuilderPushId [Id],
	SupressionId [CorrelationId],
	RequestDate,
	p.[Status],
	p.TotalRecords,
	TotalPages,
	CurrentPage,
	ErrorsEncountered,
	u.UserId,
	u.UserName,
	r.SLUG,
	p.Instructions,
	p.LockId,
	d.[Status] [DealStatus],
	Convert(datetime, l.[Value]) [WaitTill],
	j.JobId,
	p.ListName
FROM integration.NationBuilderPush p (nolock)
INNER JOIN integration.NationBuilderRegistration r (nolock)
ON p.NationBuilderRegistrationId = r.NationBuilderRegistrationId
INNER JOIN [accounts].UserDetail ud (nolock)
ON r.UserDetailId = ud.UserDetailId
INNER JOIN dbo.aspnet_Users u (nolock)
ON ud.UserId = u.UserId
LEFT JOIN jobs.JobQueue j (nolock)
ON p.JobId = j.JobId
LEFT JOIN [sales].[Deals] d (nolock)
ON p.DealId = d.DealId
LEFT JOIN integration.NationBuilderPushLookup l (nolock)
ON p.NationBuilderPushId = l.NationBuilderPushLookupId AND l.[Key] = 'WaitTill'