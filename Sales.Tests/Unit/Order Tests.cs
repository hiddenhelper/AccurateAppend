using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace AccurateAppend.Sales.Tests.Unit
{
    [TestFixture()]
    [Description("Unit tests for rules that effect all Order types , (unless overriden), even if the test is using a specific concrete type")]
    public class OrderTests
    {
        [Test()]
        public void TotalCalculatesExpectedValue()
        {
            var dealMock = new Mock<DealBinder>();
            dealMock.SetupGet(d => d.Orders).Returns(new List<Order>());
            dealMock.SetupGet(d => d.Status).Returns(DealStatus.InProcess);
            
            var orderMock = new Mock<Order>(dealMock.Object);
            orderMock.CallBase = true;
            var order = orderMock.Object;

            // Should start at 0
            Assert.That(order.Total(), Is.EqualTo(0));

            // Should sum single item.Total
            var mockItem1 = new Mock<ProductLine>();
            mockItem1.Setup(m => m.Total()).Returns(1);
            order.Lines.Add(mockItem1.Object);
            Assert.That(order.Total(), Is.EqualTo(1));

            // Should sum all item.Total
            var mockItem2 = new Mock<ProductLine>();
            mockItem2.Setup(m => m.Total()).Returns(10);
            order.Lines.Add(mockItem2.Object);
            Assert.That(order.Total(), Is.EqualTo(11));
        }

        [Test()]
        public void AdjustmentEffectsTotal()
        {
            var dealMock = new Mock<DealBinder>();
            dealMock.SetupGet(d => d.Orders).Returns(new List<Order>());
            dealMock.SetupGet(d => d.Status).Returns(DealStatus.InProcess);

            var orderMock = new Mock<Order>(dealMock.Object);
            orderMock.CallBase = true;
            var order = orderMock.Object;

            // Should start at 0
            Assert.That(order.Total(), Is.EqualTo(0));
            Assert.That(order.OrderMinimum, Is.EqualTo(0));

            // Should sum single item.Total
            var mockItem1 = new Mock<ProductLine>();
            mockItem1.Setup(m => m.Total()).Returns(10);
            order.Lines.Add(mockItem1.Object);
            Assert.That(order.Total(), Is.EqualTo(10));

            // should return total
            order.OrderMinimum = 5;
            Assert.That(order.Total(), Is.EqualTo(10));

            // should adjust to minimum
            order.OrderMinimum = 50;
            Assert.That(order.Total(), Is.EqualTo(50));

            // should adjust to 0
            order.OrderMinimum = -50;
            Assert.That(order.Total(), Is.EqualTo(0));
        }

        [Test()]
        public void OrderAddsSelfToDealCollection()
        {
            var orderList = new List<Order>();

            var dealMock = new Mock<DealBinder>();
            dealMock.SetupGet(d => d.Orders).Returns(orderList);
            dealMock.SetupGet(d => d.Status).Returns(DealStatus.InProcess);

            var orderMock = new Mock<Order>(dealMock.Object);
            orderMock.CallBase = true;
            var order = orderMock.Object;

            Assert.That(dealMock.Object.Orders, Contains.Item(order));
        }

        [Test()]
        public void CanceledOrderShouldGetNewKey()
        {
            var mockDeal = new Mock<DealBinder>();
            mockDeal.SetupGet(d => d.Orders).Returns(new List<Order>());
            mockDeal.SetupGet(d => d.Status).Returns(DealStatus.InProcess);
            mockDeal.SetupGet(d => d.Id).Returns(1);
            mockDeal.CallBase = true;
            var deal = mockDeal.Object;
            
            var mockOrder = new Mock<Order>(deal);
            mockOrder.CallBase = true;
            var order = mockOrder.Object;

            var origionalValue = order.PublicKey;
            deal.Cancel(new Audit("fake", Guid.NewGuid()));

            var newValue = order.PublicKey;

            Assert.That(origionalValue, Is.Not.EqualTo(newValue));
        }
    }
}
