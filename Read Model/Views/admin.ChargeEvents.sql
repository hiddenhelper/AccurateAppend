CREATE VIEW [admin].[ChargeEvents]
AS

SELECT
	t.CreditCardTransactionId [Id], 
	t.Amount,
	u.UserId,
	u.ApplicationId,
	u.UserName,
	t.DateAdded [EventDate],
	t.DisplayValue,
	t.[Status],
	t.FirstName + ' ' + t.LastName [FullName],
	t.OrderId,
	o.[DealId],
	t.AuthNetTransactionId [TransactionId],
	t.AuthNetTransactionType [TransactionType],
	t.AuthNetAuthCode [AuthorizationCode],
	t.AuthNetErrorMessage [Message],
	t.[Address],
	t.City,
	t.[State],
	t.ZipCode,
	t.CardExp [ExpirationDate]
FROM [billing].[CreditCardTransactions] t (nolock)
INNER JOIN [sales].[Orders] o (nolock) ON t.[OrderId] = o.[OrderId]
INNER JOIN [sales].[Deals] d (nolock) ON o.[DealId] = d.[DealId]
INNER JOIN [accounts].[UserDetail] ud (nolock) on d.[UserId] = ud.[UserId]
INNER JOIN [dbo].[aspnet_Users] u (nolock) ON ud.[UserId] = u.[UserId]
