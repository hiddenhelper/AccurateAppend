CREATE VIEW [admin].[ContactsView]
AS

SELECT
	c.[ClientContactId],
	ud.[UserId],
	u.[ApplicationId],
	c.[ContactName],
	c.[EmailAddress]
FROM [accounts].[ClientContacts] c (nolock)
INNER JOIN [accounts].[UserDetail] ud (nolock) ON ud.[UserDetailId] = c.[UserDetailId]
INNER JOIN [dbo].[aspnet_Users] u (nolock) ON u.[UserId] = ud.[UserId]
