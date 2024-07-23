using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace AccurateAppend.Sales.Tests.Unit
{
    [TestFixture()]
    public class ProductLineExtensionTests
    {
        [Test()]
        public void FilterNonBillableOperations()
        {
            var mock1 = new Mock<ProductLine>(MockBehavior.Default);
            mock1.Setup(m => m.IsBillable()).Returns(true);
            mock1.Setup(m => m.Equals(It.IsAny<ProductLine>())).CallBase();
            
            var mock2 = new Mock<ProductLine>(MockBehavior.Default);
            mock2.Setup(m => m.IsBillable()).Returns(false);
            mock2.Setup(m => m.Equals(It.IsAny<ProductLine>())).CallBase();

            var mock3 = new Mock<ProductLine>(MockBehavior.Default);
            mock3.Setup(m => m.IsBillable()).Returns(true);
            mock3.Setup(m => m.Equals(It.IsAny<ProductLine>())).CallBase();

            var lines = new[] {mock1.Object, mock2.Object, mock3.Object};

            var results = lines.FilterNonBillableOperations().ToArray();

            Assert.That(results, Has.Length.EqualTo(2));
            Assert.That(results, Contains.Item(mock1.Object));
            Assert.That(results, Does.Not.Contain(mock2.Object));
            Assert.That(results, Contains.Item(mock3.Object));
        }

        [Test()]
        public void TotalCalculatesExpectedValue()
        {
            var mockOrder = new Mock<Order>();
            mockOrder.SetupGet(m => m.Lines).Returns(new List<ProductLine>());

            var mockProduct = new Mock<Product>();
            mockProduct.SetupGet(m => m.Description).Returns("Product Description");

            var item1 = new ProductLine(mockOrder.Object, mockProduct.Object);
            item1.Price = 0.1m;
            item1.Quantity = 40;
            Assert.That(item1.Total(), Is.GreaterThan(0));

            var item2 = new ProductLine(mockOrder.Object, mockProduct.Object);
            item2.Price = 0.12m;
            item2.Quantity = 61;
            Assert.That(item2.Total(), Is.GreaterThan(0));

            var item3 = new ProductLine(mockOrder.Object, mockProduct.Object);
            item3.Price = -0.02m;
            item3.Quantity = 18;
            Assert.That(item3.Total(), Is.LessThan(0));

            Assert.That(new[] {item1, item2, item3}.Total(), Is.EqualTo(item1.Total() + item2.Total() + item3.Total()));
        }

        [Test()]
        public void TotalShouldHandleNullsAndDuplicate()
        {
            var mockOrder = new Mock<Order>();
            mockOrder.SetupGet(m => m.Lines).Returns(new List<ProductLine>());

            var mockProduct = new Mock<Product>();
            mockProduct.SetupGet(m => m.Description).Returns("Product Description");

            var item1 = new ProductLine(mockOrder.Object, mockProduct.Object);
            item1.Price = 0.1m;
            item1.Quantity = 40;
            Assert.That(item1.Total(), Is.GreaterThan(0));

            var item2 = new ProductLine(mockOrder.Object, mockProduct.Object);
            item2.Price = 0.12m;
            item2.Quantity = 61;
            Assert.That(item2.Total(), Is.GreaterThan(0));

            var item3 = new ProductLine(mockOrder.Object, mockProduct.Object);
            item3.Price = -0.02m;
            item3.Quantity = 18;
            Assert.That(item3.Total(), Is.LessThan(0));

            var data = new[] {item1, item2, item3, null, item3};
            Assert.That(data, Has.Length.EqualTo(5)); //Sanity check

            Assert.That(data.Total(), Is.EqualTo(item1.Total() + item2.Total() + item3.Total()));
        }
    }
}
