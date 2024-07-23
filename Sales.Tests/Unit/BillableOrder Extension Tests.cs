using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace AccurateAppend.Sales.Tests.Unit
{
    [TestFixture()]
    [Description("Contains unit test logic specific to the " + nameof(BillableOrderExtensions) + " class")]
    public class BillableOrderExtensionTests
    {
        [Test()]
        [Description("If the order has adjustments, they shouldn't effect the subtotal")]
        public void SubTotalShouldNotUseAdjustments()
        {
            var orderList = new List<Order>();

            var mockDeal = new Mock<MutableDeal>();
            mockDeal.SetupGet(d => d.Status).Returns(DealStatus.InProcess);
            mockDeal.SetupGet(d => d.Orders).Returns(orderList);
            var deal = mockDeal.Object;

            var order = new BillableOrder(deal);

            // Create some fake items
            var mockItem1 = new Mock<ProductLine>();
            mockItem1.Setup(m => m.Total()).Returns(1m);
            order.Lines.Add(mockItem1.Object);
            
            var mockItem2 = new Mock<ProductLine>();
            mockItem2.Setup(m => m.Total()).Returns(10m);
            order.Lines.Add(mockItem2.Object);

            Assert.That(order.Total(), Is.EqualTo(11m)); // Sanity check

            // Now add a min
            order.OrderMinimum = 50m;
            Assert.That(order.Total(), Is.EqualTo(50m)); // Sanity check

            Assert.That(order.SubTotal(), Is.EqualTo(11m));
        }
    }
}
