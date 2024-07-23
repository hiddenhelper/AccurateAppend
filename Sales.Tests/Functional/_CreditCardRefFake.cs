namespace AccurateAppend.Sales.Tests.Functional
{
    /// <summary>
    /// Fake used in testing
    /// </summary>
    internal sealed class CreditCardRefFake : CreditCardRef
    {
        internal static readonly CreditCardRef Instance = new CreditCardRefFake() {DisplayValue = "1234"};
    }
}
