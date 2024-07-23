using AccurateAppend.Core.Definitions;
using Moq;
using NUnit.Framework;

namespace AccurateAppend.Sales.Tests.Unit
{
    [TestFixture()]
    public class RateCardTests
    {
        [Test()]
        public void CanFindApprorpiateCostFromStructure()
        {
            var mockProduct = new Mock<Product>();
            mockProduct.CallBase = true;

            var mock1 = new Mock<Cost>();
            mock1.Setup(m => m.Product).Returns(mockProduct.Object);
            mock1.SetupGet(m => m.Ceiling).Returns(1);
            mock1.SetupGet(m => m.Floor).Returns(0);
            mock1.CallBase = true;

            var mock2 = new Mock<Cost>();
            mock2.Setup(m => m.Product).Returns(mockProduct.Object);
            mock2.SetupGet(m => m.Ceiling).Returns(3);
            mock2.SetupGet(m => m.Floor).Returns(2);
            mock2.CallBase = true;

            var structure = new RateCard(new[] {mock1.Object, mock2.Object}, PricingModel.Match);

            var cost = structure.FindCost(0);
            Assert.AreSame(mock1.Object, cost);

            cost = structure.FindCost(2);
            Assert.AreSame(mock2.Object, cost);

            cost = structure.FindCost(4);
            Assert.AreNotSame(mock1.Object, cost);
            Assert.AreNotSame(mock2.Object, cost);
        }
    }
}