using NUnit.Framework;

namespace AccurateAppend.Sales.Tests.Unit
{
    [TestFixture()]
    public class MonetaryExtensionTests
    {
        [Test()]
        public void CanConvertFractionalPennies()
        {
            Assert.That(MonetaryExtensions.RoundFractionalPennies(123.00m), Is.EqualTo(123.00m));
            Assert.That(MonetaryExtensions.RoundFractionalPennies(123.45m), Is.EqualTo(123.45m));
            Assert.That(MonetaryExtensions.RoundFractionalPennies(123.455m), Is.EqualTo(123.46m));
        }
    }
}
