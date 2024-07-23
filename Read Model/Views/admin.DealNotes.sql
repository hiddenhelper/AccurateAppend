CREATE VIEW [admin].[DealNotes]
AS
SELECT
	d.[DealId],
	n.[NoteId],
	n.[DateAdded],
	n.[Body],
	u.[UserId],
	u.[UserName]
FROM [sales].[DealNotes] d (nolock)
INNER JOIN [operations].[Notes] n (nolock) ON d.[NoteId] = n.[NoteId]
INNER JOIN [dbo].[aspnet_Users] u (nolock) ON n.[AddedBy] = u.[UserId]