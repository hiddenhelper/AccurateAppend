CREATE VIEW [admin].[NationBuilderRegistrations]
AS

SELECT
	NationBuilderRegistrationId [Id],
	u.UserName,
	ud.UserId,
	u.ApplicationId,
	SLUG,
	r.IsActive,
	r.LatestAccessToken,
	[DateRegistered],
	(SELECT Count(*) FROM [integration].[NationBuilderPush] p (nolock) WHERE p.NationBuilderRegistrationId = r.NationBuilderRegistrationId AND p.[Status] <> -2) [PushCount],
	0 [ReportCount],
	CASE
	WHEN PersonCount = 0 THEN null
	ELSE
	PersonCount
	END
	[PersonCount]
FROM [integration].[NationBuilderRegistration] r (nolock)
INNER JOIN [accounts].[UserDetail] ud (nolock) on r.UserDetailId = ud.UserDetailId
INNER JOIN [dbo].[aspnet_Users] u (nolock) on u.UserId = ud.UserId
WHERE r.IsActive = 1