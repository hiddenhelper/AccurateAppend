using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace AccurateAppend.Sales.Tests.Unit
{
    [TestFixture()]
    [Description("Unit tests for rules that effect all DealBinder types , (unless overriden), even if the test is using a specific concrete type")]
    public class DealTests
    {
        [Test()]
        public void CannotAddSameNoteTwice()
        {
            const String Content = "MY NOTE CONTENT";
            
            var note = new Audit(Content, Guid.NewGuid());

            var mockClient = new Mock<ClientRef>();
            // Owner and Creator do not matter for this test
            var mockDeal = new Mock<DealBinder>(mockClient.Object, Guid.NewGuid());
            mockDeal.CallBase = true;
            var deal = mockDeal.Object;

            var count = deal.Notes.Count;
            deal.Notes.Add(note);
            Assert.That(deal.Notes.Count, Is.EqualTo(count + 1));

            count = deal.Notes.Count;
            deal.Notes.Add(note);
            Assert.That(deal.Notes, Has.Count.EqualTo(count)); // should still be the same
        }

        [Test()]
        public void TotalCalculatesExpectedValue()
        {
            var mockClient = new Mock<ClientRef>();
            // Owner and Creator do not matter for this test
            var mockDeal = new Mock<DealBinder>(mockClient.Object, Guid.NewGuid());
            mockDeal.CallBase = true;
            var deal = mockDeal.Object;

            // Should start at 0
            Assert.That(deal.Total(), Is.EqualTo(0));

            // Create mock Orders
            var mockOrder1 = new Mock<BillableOrder>();
            mockOrder1.SetupGet(o => o.Status).Returns(OrderStatus.Billed);
            mockOrder1.Setup(o => o.Total()).Returns(1);
            mockOrder1.SetupGet(o => o.Deal).Returns(deal);

            var mockOrder2 = new Mock<BillableOrder>();
            mockOrder1.SetupGet(o => o.Status).Returns(OrderStatus.Open);
            mockOrder2.Setup(o => o.Total()).Returns(10);
            mockOrder2.SetupGet(o => o.Deal).Returns(deal);

            deal.Orders.Add(mockOrder1.Object);
            deal.Orders.Add(mockOrder2.Object);

            // Should sum all Order.Total
            Assert.That(deal.Total(), Is.EqualTo(11));

            //// Refunds should be in total
            //var mockOrder3 = new Mock<RefundOrder>();
            //mockOrder3.Setup(o => o.Total()).Returns(-5);
            //mockOrder3.SetupGet(o => o.Deal).Returns(deal);

            //deal.Orders.Add(mockOrder3.Object);

            //// Should sum all Order.Total
            //Assert.That(deal.Total(), Is.EqualTo(6));
        }

        [Test()]
        public void CanAddBillableOrderWhenDealIsEditable()
        {
            var orderList = new List<Order>();

            var mockDeal = new Mock<DealBinder>();
            mockDeal.SetupGet(d => d.Status).Returns(DealStatus.InProcess);
            mockDeal.SetupGet(d => d.Orders).Returns(orderList);
            mockDeal.CallBase = true;
            var deal = mockDeal.Object;

            // Sanity checks
            Assert.That(deal.Orders.Count, Is.EqualTo(0));
            Assert.That(deal.Status.CanBeEdited());

            var order = new BillableOrder(deal);

            Assert.That(deal.Orders, Has.Count.EqualTo(1));
            Assert.That(deal.Orders, Contains.Item(order));

            // Now add a 2nd one
            order = new BillableOrder(deal);

            Assert.That(deal.Orders, Has.Count.EqualTo(2));
            Assert.That(deal.Orders, Contains.Item(order));
        }

        [Test()]
        public void CannotAddBillableOrderWhenDealIsNotEditable()
        {
            var orderList = new List<Order>();

            var mockDeal = new Mock<DealBinder>();
            mockDeal.SetupGet(d => d.Status).Returns(DealStatus.Complete);
            mockDeal.SetupGet(d => d.Orders).Returns(orderList);
            mockDeal.CallBase = true;
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
        public void SendToReviewShouldUpdateAmount()
        {
            // Owner does not matter for this test
            var mockClient = new Mock<ClientRef>();

            // Product does not matter for this test
            var p = new Mock<Product>();

            var mockDeal = new Mock<DealBinder>(mockClient.Object, Guid.NewGuid());
            mockDeal.CallBase = true;
            var deal = mockDeal.Object;

            var order = new BillableOrder(deal);
            var line = order.CreateLine(p.Object);
            line.Price = 55M;
            line.Quantity = 1;

            Assert.That(deal.Total(), Is.EqualTo(55m));
            Assert.That(deal.Amount, Is.EqualTo(0));

            deal.SubmitForReview(new Audit("Ignore", Guid.NewGuid()));

            Assert.That(deal.Total(), Is.EqualTo(55m));
            Assert.That(deal.Amount, Is.EqualTo(55m));
        }

        [Test()]
        public void CancelShouldZeroAmount()
        {
            // Owner does not matter for this test
            var mockClient = new Mock<ClientRef>();

            // Product does not matter for this test
            var p = new Mock<Product>();

            var mockDeal = new Mock<DealBinder>(mockClient.Object, Guid.NewGuid());
            mockDeal.CallBase = true;
            var deal = mockDeal.Object;

            var order = new BillableOrder(deal);
            var line = order.CreateLine(p.Object);
            line.Price = 55M;
            line.Quantity = 1;

            deal.Amount = 55M;
            deal.Cancel(new Audit("Ignore", Guid.NewGuid()));

            Assert.That(deal.Status,Is.EqualTo(DealStatus.Canceled));
            Assert.That(deal.Amount, Is.EqualTo(0M));
        }

        [Test()]
        [Description("The deal should default to the owner of the client unless overriden")]
        public void DealShouldHaveSameOwnerAsClient()
        {
            // Owner matters for this test
            var userId = Guid.NewGuid();
            var mockClient = new Mock<ClientRef>();
            mockClient.Setup(m => m.OwnerId).Returns(userId);
            var client = mockClient.Object;

            var deal = new MutableDeal(client);
            Assert.That(deal.OwnerId, Is.EqualTo(userId));

            var newUserId = Guid.NewGuid();
            deal = new MutableDeal(client, newUserId);
            Assert.That(deal.OwnerId, Is.EqualTo(newUserId));
            Assert.That(deal.OwnerId, Is.Not.EqualTo(userId));
        }
    }
}