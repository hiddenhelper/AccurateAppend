using System;
using Moq;
using NUnit.Framework;

namespace AccurateAppend.Sales.Tests.Unit
{
    [TestFixture()]
    public class CostTests
    {
        [Test()]
        public void IsSystemIsCaseInensitive()
        {
            Assert.That(Cost.IsSystem(Cost.CsvUploadCategory), Is.True);
            Assert.That(Cost.IsSystem(Cost.DefaultCategory), Is.True);
            Assert.That(Cost.IsSystem(Cost.ListBuilderCategory), Is.True);
            Assert.That(Cost.IsSystem(Cost.NationBuilderCategory), Is.True);

            Assert.That(Cost.IsSystem(Cost.CsvUploadCategory.ToLower()), Is.True);
            Assert.That(Cost.IsSystem(Cost.DefaultCategory.ToLower()), Is.True);
            Assert.That(Cost.IsSystem(Cost.ListBuilderCategory.ToLower()), Is.True);
            Assert.That(Cost.IsSystem(Cost.NationBuilderCategory.ToLower()), Is.True);
        }

        [Test()]
        public void CanHandleFiltersOnProduct()
        {
            var productMock1 = new Mock<Product>();
            productMock1.Setup(p => p.Id).Returns(1);
            productMock1.CallBase = true;
            var product1 = productMock1.Object;

            var productMock2 = new Mock<Product>(MockBehavior.Strict);
            productMock2.Setup(p => p.Id).Returns(2);
            var product2 = productMock2.Object;

            var productMock3 = new Mock<Product>(MockBehavior.Strict);
            productMock3.Setup(p => p.Id).Returns(1);
            var product3 = productMock3.Object;

            var c = new Cost(product1, "Doesn't Matter");
            c.Floor = 0;
            c.Ceiling = Int32.MaxValue;

            Assert.That(c.CanHandle(product2, 0), Is.False);
            Assert.That(c.CanHandle(product3, 0), Is.True);
        }

        [Test()]
        public void CanHandleFiltersOnRanges()
        {
            var productMock1 = new Mock<Product>();
            productMock1.Setup(p => p.Id).Returns(1);
            productMock1.CallBase = true;
            var product1 = productMock1.Object;

            var productMock2 = new Mock<Product>();
            productMock2.Setup(p => p.Id).Returns(1);
            var product2 = productMock2.Object;
            
            var c = new Cost(product1, "Doesn't Matter");
            c.Floor = 0;
            c.Ceiling = 2;

            Assert.That(c.CanHandle(product2, 0), Is.True);
            Assert.That(c.CanHandle(product2, 1), Is.True);
            Assert.That(c.CanHandle(product2, 3), Is.False);
        }
    }
}
