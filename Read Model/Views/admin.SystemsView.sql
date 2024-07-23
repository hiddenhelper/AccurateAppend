CREATE VIEW [admin].[SystemsView]
AS
	SELECT 
	[InstanceId],
	[SystemName],
	[ExecutionHost],
	[RunBy] [UserId],
	[RegisteredOn] [Heartbeat],
	[Version],
	[UserName]
	FROM [operations].[Systems] (nolock) s
	INNER JOIN [dbo].[aspnet_Users] (nolock) u ON s.[RunBy] = u.[UserId]
