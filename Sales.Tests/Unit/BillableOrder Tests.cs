using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace AccurateAppend.Sales.Tests.Unit
{
    [TestFixture()]
    [Description("Contains unit test logic specific to the " + nameof(BillableOrder) + " class")]
    public class BillableOrderTests
    {
        [Test()]
        public void WriteOffOrdersShouldTotalZero()
        {
            var mock = new Mock<DealBinder>();
            mock.SetupGet(d => d.Orders).Returns(new List<Order>());
            mock.SetupGet(d => d.Status).Returns(DealStatus.InProcess);

            // Should sum single item.Total
            var mockItem1 = new Mock<ProductLine>();
            mockItem1.Setup(m => m.Total()).Returns(1);

            var mockOrder = new Mock<BillableOrder>();
            mockOrder.SetupGet(o => o.Status).Returns(OrderStatus.WriteOff);
            mockOrder.CallBase = true;

            var order = mockOrder.Object;
            Assert.That(order.Total(), Is.EqualTo(0)); // sanity check

            order.Lines.Add(mockItem1.Object);
            Assert.That(order.Total(), Is.EqualTo(0));
            Assert.That(order.Lines.Sum(i => i.Total()), Is.EqualTo(1));
        }

        [Test()]
        public void CannotAddBillableOrderWhenDealIsNotEditable()
        {
            var orderList = new List<Order>();

            var mockDeal = new Mock<MutableDeal>();
            mockDeal.SetupGet(d => d.Status).Returns(DealStatus.Complete);
            mockDeal.SetupGet(d => d.Orders).Returns(orderList);
            var deal = mockDeal.Object;

            // Sanity checks
            Assert.That(deal.Orders.Count, Is.EqualTo(0));
            Assert.That(!deal.Status.CanBeEdited());

            try
            {
                new BillableOrder(deal);
                Assert.Fail("Exception should have been thrown");
            }
            catch (InvalidOperationException)
            {
                Assert.That(deal.Orders.Count, Is.EqualTo(0));
            }
        }

        [Test()]
        public void PostChargeShouldRequireMatchingRequest()
        {
            var mockDeal = new Mock<DealBinder>(MockBehavior.Strict);
            mockDeal.SetupGet(d => d.Status).Returns(DealStatus.Billing);

            var mockOrder = new Mock<BillableOrder>();
            mockOrder.SetupGet(o => o.Status).Returns(OrderStatus.Open);
            mockOrder.Setup(o => o.Total()).Returns(101.54m);
            mockOrder.Setup(o => o.Deal).Returns(mockDeal.Object);
            mockOrder.CallBase = true;

            var order = mockOrder.Object;

            var fakeId = Guid.NewGuid();
            var mockCharge = new Mock<TransactionEvent>();
            mockCharge.Setup(m => m.Order).Returns(order);
            mockCharge.Setup(m => m.PublicKey).Returns(fakeId);

            try
            {
                order.PostCharge(mockCharge.Object);
                Assert.Fail("Exception should have been thrown");
            }
            catch (InvalidOperationException ex)
            {
                Assert.That(ex.Message,Is.EqualTo($"The posted transactions {fakeId} lacks a matching pending transaction request. Was this posted to the correct order?"));
            }
        }
    }
}
