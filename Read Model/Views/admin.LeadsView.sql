CREATE VIEW [admin].[LeadsView]
AS

SELECT 
	LeadId,
	FirstName,
	LastName,
	BusinessName,
	Email,
	Phone,
	NoteCount,
	CASE
		WHEN DATEDIFF(MINUTE, DateAdded, NoteDate) > 0 THEN NoteDate
		ELSE DateAdded
	END [LastUpdate],
	FollowUpDate,
	DateAdded,
	[Status],
	ApplicationId,
	ApplicationName,
	Qualified,
	LeadSource,
	ContactMethod,
	Website,
	[Address],
	City,
	[State],
	Zip,
	DoNotMarketTo,
	OwnerId,
	OwnerUserName
FROM (
	SELECT DISTINCT
		l.LeadId,
		l.FirstName,
		l.LastName,
		l.BusinessName,
		l.Email,
		l.Phone,
		(SELECT Count(*) FROM [accounts].[LeadNotes] (nolock) WHERE [LeadId] = l.[LeadId]) [NoteCount],
		(SELECT Max(n.DateAdded) FROM [operations].[Notes] n (nolock) WHERE n.[NoteId] IN (SELECT [NoteId] FROM [accounts].[LeadNotes] WHERE [LeadId] = l.[LeadId])) [NoteDate],
		l.FollowUpDate,
		l.DateAdded,
		l.[Status],
		l.ApplicationId,
		s.ApplicationName,
		l.Qualified,
		l.LeadSource,
		l.ContactMethod,
		l.Website,
		l.[Address],
		l.City,
		l.[State],
		l.Zip,
		Coalesce(l.DoNotMarketTo, CONVERT(bit, 0)) [DoNotMarketTo],
		OwnerId,
		u.UserName [OwnerUserName]
	FROM [accounts].[Leads] (nolock) l
	LEFT JOIN [security].[Sites] s (nolock) ON l.[ApplicationId] = s.[ApplicationId]
	INNER JOIN [dbo].[aspnet_Users] u (nolock) ON u.[UserId] = l.[OwnerId]
	WHERE l.Deleted = 0
	) q

