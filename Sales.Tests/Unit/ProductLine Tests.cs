using System;
using System.Collections.Generic;
using System.Linq;
using AccurateAppend.Core.Definitions;
using Moq;
using NUnit.Framework;

namespace AccurateAppend.Sales.Tests.Unit
{
    [TestFixture()]
    public class ProductLineTests
    {
        [Test()]
        public void TotalCalculatesExpectedValue()
        {
            var mockOrder = new Mock<Order>();
            mockOrder.SetupGet(m => m.Lines).Returns(new List<ProductLine>());

            var mockProduct = new Mock<Product>();
            mockProduct.SetupGet(m => m.Description).Returns("Product Description");

            var item = new ProductLine(mockOrder.Object, mockProduct.Object);
            item.Price = 0;
            item.Quantity = 0;
            Assert.That(item.Total(), Is.EqualTo(0));

            item.Quantity = 1;
            Assert.That(item.Total(), Is.EqualTo(0));

            item.Price = 1;
            Assert.That(item.Total(), Is.EqualTo(1));

            item.Quantity = 10;
            Assert.That(item.Total(), Is.EqualTo(10));

            item.Price = -1;
            Assert.That(item.Total(), Is.EqualTo(-10));
        }

        [Test()]
        public void TotalSkipsNonBillableItems()
        {
            var mockOrder = new Mock<Order>();
            mockOrder.SetupGet(m => m.Lines).Returns(new List<ProductLine>());

            var mockProduct = new Mock<Product>();
            mockProduct.SetupGet(m => m.Key).Returns(DataServiceOperation.SET_PREF_BASED_ON_VERIFICATION.ToString());

            var item = new ProductLine(mockOrder.Object, mockProduct.Object);

            Assert.That(item.IsBillable, Is.False); // Sanity check

            item.Price = 50;
            item.Quantity = 1;

            Assert.That(item.Total(), Is.EqualTo(0));
        }

        [Test()]
        public void ItemUsesDescriptionValue()
        {
            const String Description = "Product Description";

            var mockOrder = new Mock<Order>();
            mockOrder.SetupGet(m => m.Lines).Returns(new List<ProductLine>());

            var mockProduct = new Mock<Product>(MockBehavior.Strict);
            mockProduct.SetupGet(m => m.Description).Returns(Description);

            var item = new ProductLine(mockOrder.Object, mockProduct.Object);

            Assert.That(item.Description, Is.EqualTo(Description));
        }

        [Test()]
        public void ItemAddsSelfToOrderCollection()
        {
            var itemList = new List<ProductLine>();
            const String Description = "Product Description";

            var mockOrder = new Mock<Order>(MockBehavior.Strict);
            mockOrder.SetupGet(m => m.Lines).Returns(itemList);

            var mockProduct = new Mock<Product>();
            mockProduct.SetupGet(m => m.Description).Returns(Description);

            var item = new ProductLine(mockOrder.Object, mockProduct.Object);

            Assert.That(mockOrder.Object.Lines, Contains.Item(item));
        }

        [Test()]
        public void ItemsCanEvaluateBillable()
        {
            var operationstoexclude = new List<String>();
            operationstoexclude.AddRange(DataServiceOperationExtensions.PreferenceOperations.Select(o => o.ToString()));

            var data = new Stack<String>(operationstoexclude);
            data.Push("BILLABLE");

            var mockOrder = new Mock<Order>();
            mockOrder.SetupGet(m => m.Lines).Returns(new List<ProductLine>());

            var mockProduct = new Mock<Product>();
            mockProduct.SetupGet(m => m.Key).Returns(data.Pop);

            var item = new ProductLine(mockOrder.Object, mockProduct.Object);

            Assert.IsTrue(item.IsBillable());

            for (var i = 0; i < operationstoexclude.Count; i++)
            {
                Assert.IsFalse(item.IsBillable());
            }
        }

        [Test()]
        public void ItemsCanEvaluateRestrictedRefunds()
        {
            var appendproducts = new List<String>();
            appendproducts.AddRange(Enum.GetNames(typeof(DataServiceOperation)));

            var data = new Stack<String>(appendproducts);
            data.Push("UNRESTRICTED");

            var mockOrder = new Mock<Order>();
            mockOrder.SetupGet(m => m.Lines).Returns(new List<ProductLine>());

            var mockProduct = new Mock<Product>();
            mockProduct.SetupGet(m => m.Key).Returns(data.Pop);

            var item = new ProductLine(mockOrder.Object, mockProduct.Object);

            Assert.IsFalse(item.HasRestrictedRefund());

            for (var i = 0; i < appendproducts.Count; i++)
            {
                Assert.IsTrue(item.HasRestrictedRefund());
            }
        }
    }
}