using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace AccurateAppend.Sales.Tests.Unit
{
    [TestFixture()]
    public class RefundOrderTests
    {
        [Test()]
        public void DealMustBeCompleteToRefund()
        {
            var mockDeal = new Mock<DealBinder>();
            mockDeal.Setup(m => m.Status).Returns(DealStatus.Billing);
            mockDeal.CallBase = true;

            var deal = mockDeal.Object;

            try
            {
                new RefundOrder(deal);
                Assert.Fail("Exception should have been thrown");
            }
            catch (InvalidOperationException)
            {
            }
        }

        [Test()]
        public void DealMayOnlyHaveOneOpenRefundOrder()
        {
            var orders = new List<Order>();

            var mockRefund = new Mock<RefundOrder>();
            mockRefund.Setup(m => m.Status).Returns(OrderStatus.Open);
            orders.Add(mockRefund.Object);

            var mockDeal = new Mock<DealBinder>();
            mockDeal.Setup(m => m.Status).Returns(DealStatus.Complete);
            mockDeal.Setup(m => m.Orders).Returns(() => orders);
            mockDeal.CallBase = true;

            var deal = mockDeal.Object;

            try
            {
                new RefundOrder(deal);
                Assert.Fail("Exception should have been thrown");
            }
            catch (InvalidOperationException)
            {
            }
        }

        [Test(Description = "When we add a refund, all lines with a total > 0 should be added to the refund and have limits based on billed quantity if restricted")]
        public void RefundOrderShouldBuildFromBillableOrder()
        {
            var lines = new List<ProductLine>();
            var orders = new List<Order>();

            var mockDeal = new Mock<DealBinder>();
            mockDeal.Setup(m => m.Status).Returns(DealStatus.Complete);
            mockDeal.Setup(m => m.Orders).Returns(orders);
            mockDeal.CallBase = true;
            var deal = mockDeal.Object;

            var orderMock = new Mock<BillableOrder>();
            orderMock.Setup(m => m.Status).Returns(OrderStatus.Billed);
            orderMock.Setup(m => m.Lines).Returns(lines);
            orderMock.CallBase = true;

            orders.Add(orderMock.Object);

            var mockp1 = new Mock<Product>();
            mockp1.Setup(m => m.Key).Returns("FAKE");
            var mockL1 = new Mock<ProductLine>();
            mockL1.Setup(m => m.Product).Returns(mockp1.Object);
            mockL1.Setup(m => m.Quantity).Returns(15);
            mockL1.Setup(m => m.Price).Returns(.21M);
            mockL1.Setup(m => m.HasRestrictedRefund()).Returns(true);
            mockL1.CallBase = true;
            lines.Add(mockL1.Object);

            var mockp2 = new Mock<Product>();
            mockp2.Setup(m => m.Key).Returns("OTHERFAKE");
            var mockL2 = new Mock<ProductLine>();
            mockL2.Setup(m => m.Product).Returns(mockp2.Object);
            mockL2.Setup(m => m.Quantity).Returns(5000);
            mockL2.Setup(m => m.Price).Returns(.01M);
            mockL2.Setup(m => m.HasRestrictedRefund()).Returns(false);
            mockL2.CallBase = true;
            lines.Add(mockL2.Object);

            var mockp3 = new Mock<Product>();
            mockp3.Setup(m => m.Key).Returns("OTHEROTHERFAKE");
            var mockL3 = new Mock<ProductLine>();
            mockL3.Setup(m => m.Product).Returns(mockp3.Object);
            mockL3.Setup(m => m.Quantity).Returns(5000);
            mockL3.Setup(m => m.Price).Returns(.01M);
            mockL3.Setup(m => m.Total()).Returns(-1);
            mockL3.CallBase = true;
            lines.Add(mockL3.Object);

            Assert.That(lines.Count,Is.EqualTo(3)); // Sanity check

            var refund = new RefundOrder(deal);

            // Only >0 lines added
            Assert.That(refund.Lines, Has.Count.EqualTo(2));

            // Confirms we have a line for each expected product
            Assert.That(refund.Lines.Any(l => l.Product.Key == "FAKE"));
            Assert.That(refund.Lines.Any(l => l.Product.Key == "OTHERFAKE"));

            // Refund lines should be based on the origional
            var fakeProductLine = refund.Lines.First(l => l.Product.Key == "FAKE");
            Assert.That(fakeProductLine.Price, Is.EqualTo(mockL1.Object.Price*-1)); // Price is inverted
            Assert.That(fakeProductLine.Quantity, Is.EqualTo(0));
            Assert.That(fakeProductLine.Maximum, Is.EqualTo(mockL1.Object.Quantity));

            var otherFakeProductLine = refund.Lines.First(l => l.Product.Key == "OTHERFAKE");
            Assert.That(otherFakeProductLine.Price, Is.EqualTo(mockL2.Object.Price*-1)); // Price is inverted
            Assert.That(otherFakeProductLine.Quantity, Is.EqualTo(0));
            Assert.That(otherFakeProductLine.Maximum, Is.Null);
        }

        [Test(Description = "When we add a refund, all lines should be aggregated by price and product")]
        public void RefundOrderShouldHandleMutliplesOfSameProduct()
        {
            var lines = new List<ProductLine>();
            var orders = new List<Order>();

            var mockDeal = new Mock<DealBinder>();
            mockDeal.Setup(m => m.Status).Returns(DealStatus.Complete);
            mockDeal.Setup(m => m.Orders).Returns(orders);
            mockDeal.CallBase = true;
            var deal = mockDeal.Object;

            var orderMock = new Mock<BillableOrder>();
            orderMock.Setup(m => m.Status).Returns(OrderStatus.Billed);
            orderMock.Setup(m => m.Lines).Returns(lines);
            orderMock.CallBase = true;

            orders.Add(orderMock.Object);

            var mockp1 = new Mock<Product>();
            mockp1.Setup(m => m.Key).Returns("FAKE");
            var mockL1 = new Mock<ProductLine>();
            mockL1.Setup(m => m.Product).Returns(mockp1.Object);
            mockL1.Setup(m => m.Quantity).Returns(15);
            mockL1.Setup(m => m.Price).Returns(.21M);
            mockL1.Setup(m => m.HasRestrictedRefund()).Returns(true);
            mockL1.CallBase = true;
            lines.Add(mockL1.Object);

            var mockL2 = new Mock<ProductLine>();
            mockL2.Setup(m => m.Product).Returns(mockp1.Object);
            mockL2.Setup(m => m.Quantity).Returns(5000);
            mockL2.Setup(m => m.Price).Returns(.01M);
            mockL2.Setup(m => m.HasRestrictedRefund()).Returns(false);
            mockL2.CallBase = true;
            lines.Add(mockL2.Object);

            var mockp2 = new Mock<Product>();
            mockp2.Setup(m => m.Key).Returns("OTHERFAKE");

            var mockL3 = new Mock<ProductLine>();
            mockL3.Setup(m => m.Product).Returns(mockp2.Object);
            mockL3.Setup(m => m.Quantity).Returns(5000);
            mockL3.Setup(m => m.Price).Returns(.01M);
            mockL3.CallBase = true;
            lines.Add(mockL3.Object);

            var mockL4 = new Mock<ProductLine>();
            mockL4.Setup(m => m.Product).Returns(mockp2.Object);
            mockL4.Setup(m => m.Quantity).Returns(1000);
            mockL4.Setup(m => m.Price).Returns(.01M);
            mockL4.CallBase = true;
            lines.Add(mockL4.Object);

            Assert.That(lines.Count, Is.EqualTo(4)); // Sanity check

            var refund = new RefundOrder(deal);

            // Only 3 lines added
            Assert.That(refund.Lines, Has.Count.EqualTo(3));

            // Confirms we have a line for each expected product
            Assert.That(refund.Lines.Any(l => l.Product.Key == "FAKE"));
            Assert.That(refund.Lines.Any(l => l.Product.Key == "OTHERFAKE"));

            // Refund lines should be based on the origional
            var fakeProductLine = refund.Lines.First(l => l.Product.Key == "FAKE");
            Assert.That(fakeProductLine.Price, Is.EqualTo(mockL1.Object.Price * -1)); // Price is inverted
            Assert.That(fakeProductLine.Quantity, Is.EqualTo(0));
            Assert.That(fakeProductLine.Maximum, Is.EqualTo(mockL1.Object.Quantity));

            var otherFakeProductLine = refund.Lines.First(l => l.Product.Key == "OTHERFAKE");
            Assert.That(otherFakeProductLine.Price, Is.EqualTo(mockL2.Object.Price * -1)); // Price is inverted
            Assert.That(otherFakeProductLine.Quantity, Is.EqualTo(0));
            Assert.That(otherFakeProductLine.Maximum, Is.Null);
        }

        [Test(Description = "When we add a refund, we should account for existing refunds")]
        public void RefundOrderShouldBeAwareOfPreviousRefundsOnRestrictedItems()
        {
            var orders = new List<Order>();

            var mockDeal = new Mock<DealBinder>();
            mockDeal.Setup(m => m.Status).Returns(DealStatus.Complete);
            mockDeal.Setup(m => m.Orders).Returns(orders);
            mockDeal.CallBase = true;
            var deal = mockDeal.Object;

            var mockp1 = new Mock<Product>();
            mockp1.Setup(m => m.Key).Returns("FAKE");

            var mockp2 = new Mock<Product>();
            mockp2.Setup(m => m.Key).Returns("OTHERFAKE");

            #region Order

            var orderLines = new List<ProductLine>();
            
            var orderMock = new Mock<BillableOrder>();
            orderMock.Setup(m => m.Status).Returns(OrderStatus.Billed);
            orderMock.Setup(m => m.Lines).Returns(orderLines);
            orderMock.CallBase = true;

            orders.Add(orderMock.Object);
            
            var mockL1 = new Mock<ProductLine>();
            mockL1.Setup(m => m.Product).Returns(mockp1.Object);
            mockL1.Setup(m => m.Quantity).Returns(15);
            mockL1.Setup(m => m.Price).Returns(.21M);
            mockL1.Setup(m => m.HasRestrictedRefund()).Returns(true);
            mockL1.CallBase = true;
            orderLines.Add(mockL1.Object);

            var mockL2 = new Mock<ProductLine>();
            mockL2.Setup(m => m.Product).Returns(mockp2.Object);
            mockL2.Setup(m => m.Quantity).Returns(5000);
            mockL2.Setup(m => m.Price).Returns(.01M);
            mockL2.Setup(m => m.HasRestrictedRefund()).Returns(false);
            mockL2.CallBase = true;
            orderLines.Add(mockL2.Object);
            
            Assert.That(orderLines.Count, Is.EqualTo(2)); // Sanity check

            #endregion

            #region Previous Refund

            var refundLines = new List<ProductLine>();

            var refundMock = new Mock<RefundOrder>();
            refundMock.Setup(m => m.Status).Returns(OrderStatus.Refunded);
            refundMock.Setup(m => m.Lines).Returns(refundLines);
            refundMock.CallBase = true;

            orders.Add(refundMock.Object);

            var mockL3 = new Mock<ProductLine>();
            mockL3.Setup(m => m.Product).Returns(mockp1.Object);
            mockL3.Setup(m => m.Quantity).Returns(5);
            mockL3.Setup(m => m.Price).Returns(-.21M);
            mockL3.Setup(m => m.HasRestrictedRefund()).Returns(true);
            mockL3.CallBase = true;
            refundLines.Add(mockL3.Object);

            var mockL4 = new Mock<ProductLine>();
            mockL4.Setup(m => m.Product).Returns(mockp2.Object);
            mockL4.Setup(m => m.Quantity).Returns(1000);
            mockL4.Setup(m => m.Price).Returns(.01M);
            mockL4.Setup(m => m.HasRestrictedRefund()).Returns(false);
            mockL4.CallBase = true;
            refundLines.Add(mockL4.Object);

            Assert.That(refundLines.Count, Is.EqualTo(2)); // Sanity check

            #endregion

            var refund = new RefundOrder(deal);

            // 2 lines added
            Assert.That(refund.Lines, Has.Count.EqualTo(2));

            // Confirms we have a line for each expected product
            Assert.That(refund.Lines.Any(l => l.Product.Key == "FAKE"));
            Assert.That(refund.Lines.Any(l => l.Product.Key == "OTHERFAKE"));

            // Refund lines should be based on the origional
            var fakeProductLine = refund.Lines.First(l => l.Product.Key == "FAKE");
            Assert.That(fakeProductLine.Price, Is.EqualTo(mockL1.Object.Price * -1)); // Price is inverted
            Assert.That(fakeProductLine.Quantity, Is.EqualTo(0));
            Assert.That(fakeProductLine.Maximum, Is.EqualTo(mockL1.Object.Quantity - mockL3.Object.Quantity)); // Value is remaining items

            var otherFakeProductLine = refund.Lines.First(l => l.Product.Key == "OTHERFAKE");
            Assert.That(otherFakeProductLine.Price, Is.EqualTo(mockL2.Object.Price * -1)); // Price is inverted
            Assert.That(otherFakeProductLine.Quantity, Is.EqualTo(0));
            Assert.That(otherFakeProductLine.Maximum, Is.Null); // No restriction
        }

        [Test()]
        public void AdjustmentEffectsTotal()
        {
            var mock = new Mock<DealBinder>();
            mock.SetupGet(d => d.Orders).Returns(new List<Order>());
            mock.SetupGet(d => d.Status).Returns(DealStatus.Complete);

            // Should start at 0
            var order = new RefundOrder(mock.Object);
            Assert.That(order.Total(), Is.EqualTo(0));
            Assert.That(order.OrderMinimum, Is.EqualTo(0));

            // Should sum single item.Total
            var mockItem1 = new Mock<ProductLine>();
            mockItem1.Setup(m => m.Total()).Returns(-10);
            mockItem1.Setup(m => m.Maximum).Returns(1);
            order.Lines.Add(mockItem1.Object);
            Assert.That(order.Total(), Is.EqualTo(-10));

            // should return total
            order.OrderMinimum = 5;
            Assert.That(order.Total(), Is.EqualTo(-5));

            // had credit greater than bill applied
            order.OrderMinimum = 50;
            Assert.That(order.Total(), Is.EqualTo(-0));

            // should return total
            order.OrderMinimum = -5;
            Assert.That(order.Total(), Is.EqualTo(-10));

            // should adjust to minimum
            order.OrderMinimum = -50;
            Assert.That(order.Total(), Is.EqualTo(-50));
        }
    }
}
