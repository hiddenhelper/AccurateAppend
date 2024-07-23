using System;
using Moq;
using NUnit.Framework;

namespace AccurateAppend.Sales.Tests.Unit
{
    [TestFixture()]
    public class MutableDealExtensionsTests
    {
        [Test()]
        public void DealMustBeCompleteToRefund()
        {
            var mock = new Mock<MutableDeal>(MockBehavior.Strict);
            mock.Setup(m => m.Status).Returns(DealStatus.InProcess);

            var deal = mock.Object;

            try
            {
                deal.CreateRefund();
                Assert.Fail("Exception should have been thrown");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.That(ex.Message, Is.EqualTo("Deals must be Complete to issue a refund\r\nParameter name: deal\r\nActual value was InProcess."));
                Assert.That(ex.ActualValue, Is.EqualTo(DealStatus.InProcess));
                Assert.That(ex.ParamName, Is.EqualTo("deal"));
            }
        }
    }
}
