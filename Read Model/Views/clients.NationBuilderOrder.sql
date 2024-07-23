CREATE VIEW [clients].[NationBuilderOrder]
AS

SELECT po.[OrderId], p.[NationBuilderPushId] [RequestId], r.[SLUG], p.[Status] [PushStatus],

CASE WHEN p.[Status]=2 THEN (SELECT Count(*) FROM [jobs].[JobSliceQueue] s (nolock) WHERE s.[JobId] = p.[JobId] AND s.[Status] =1) ELSE p.[CurrentPage] END [CurrentPage],
CASE WHEN p.[Status]=2 THEN (SELECT Count(*) FROM [jobs].[JobSliceQueue] s (nolock) WHERE s.[JobId] = p.[JobId]) ELSE p.[TotalPages] END [TotalPages]

FROM [sales].[ProductOrder] po (nolock)
INNER JOIN [integration].[NationBuilderPush] p (nolock) ON p.[SupressionId] = po.[OrderId]
INNER JOIN integration.NationBuilderRegistration r (nolock) on r.[NationBuilderRegistrationId] = p.[NationBuilderRegistrationId] and r.[IsActive] = 1
WHERE po.[Source] = 1 -- NB