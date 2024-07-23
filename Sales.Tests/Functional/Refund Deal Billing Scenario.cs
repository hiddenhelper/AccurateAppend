using System;
using System.Linq;
using System.Net.Mail;
using AccurateAppend.Core.Definitions;
using Moq;
using NUnit.Framework;

namespace AccurateAppend.Sales.Tests.Functional
{
    [TestFixture()]
    public class Refund_Deal_Billing_Scenario
    {
        [Test()]
        [Description("Confirms refund creation against DataServiceOperation products and Unrestricted products are initialized according to maximal rules")]
        public void CreateRefund()
        {
            #region Completed Deal

            var adminUser = Guid.Empty;
            var client = (new Mock<ClientRef>()).Object; // Doesn't matter

            var deal = new MutableDeal(client, adminUser);

            Assert.That(deal.CompletedDate, Is.Null); // Sanity check

            var order = deal.CreateOrder();

            var p1 = (new Mock<Product>()).Object; // Doesn't matter
            var line1 = order.CreateLine(p1);
            line1.Price = 1;
            line1.Quantity = 55;
            Assert.That(line1.HasRestrictedRefund(), Is.False); // Sanity check

            var p2Mock = new Mock<Product>();
            p2Mock.Setup(m => m.Key).Returns(DataServiceOperation.CASS.ToString());

            var line2 = order.CreateLine(p2Mock.Object);
            line2.Price = 2;
            line2.Quantity = 14;
            Assert.That(line2.HasRestrictedRefund(), Is.True); // Sanity check

            var bill = new BillContent("party@domain.com");
            bill.SendTo.Add(new MailAddress("someone@somewhere.com"));

            order.DraftBill(bill, ContractType.Receipt, adminUser);

            deal.Approve(new Audit("Approved", adminUser));

            var request = order.CreateRequest(CreditCardRefFake.Instance, 83m);
            var charge = request.Complete(TransactionResult.Approved, 83m, 83m);
            order.PostCharge(charge);

            Assert.That(deal.Total(), Is.EqualTo(83M)); // Sanity check
            Assert.That(deal.Status, Is.EqualTo(DealStatus.Complete)); // Sanity check
            
            #endregion

            var refund = deal.CreateRefund();

            Assert.That(refund.Lines.Count, Is.EqualTo(order.Lines.Count));

            // Product 1 unrestricted
            Assert.That(refund.Lines.First().Product, Is.EqualTo(order.Lines.First().Product));
            Assert.That(refund.Lines.First().Maximum, Is.EqualTo(null));
            Assert.That(refund.Lines.First().Quantity, Is.EqualTo(0));
            Assert.That(refund.Lines.First().Price, Is.EqualTo(order.Lines.First().Price * -1));

            // Product 2 DSO
            Assert.That(refund.Lines.Skip(1).First().Product, Is.EqualTo(order.Lines.Skip(1).First().Product));
            Assert.That(refund.Lines.Skip(1).First().Maximum, Is.EqualTo(order.Lines.Skip(1).First().Quantity));
            Assert.That(refund.Lines.Skip(1).First().Quantity, Is.EqualTo(0));
            Assert.That(refund.Lines.Skip(1).First().Price, Is.EqualTo(order.Lines.Skip(1).First().Price * -1));
        }

        [Test()]
        [Description("Confirms refund processing")]
        public void CreateRefundDateServiceOperation()
        {
            #region Completed Deal

            var adminUser = Guid.Empty;
            var client = (new Mock<ClientRef>()).Object; // Doesn't matter

            var deal = new MutableDeal(client, adminUser);

            Assert.That(deal.CompletedDate, Is.Null); // Sanity check

            var order = deal.CreateOrder();

            var p1 = (new Mock<Product>()).Object; // Doesn't matter
            var line1 = order.CreateLine(p1);
            line1.Price = 1;
            line1.Quantity = 55;
            Assert.That(line1.HasRestrictedRefund(), Is.False); // Sanity check

            var bill = new BillContent("party@domain.com");
            bill.SendTo.Add(new MailAddress("someone@somewhere.com"));

            order.DraftBill(bill, ContractType.Receipt, adminUser);

            deal.Approve(new Audit("Approved", adminUser));

            var request = order.CreateRequest(CreditCardRefFake.Instance, 55m);
            var charge = request.Complete(TransactionResult.Approved, 55m, 55m);
            order.PostCharge(charge);

            Assert.That(deal.Total(), Is.EqualTo(55m)); // Sanity check
            Assert.That(deal.Status, Is.EqualTo(DealStatus.Complete)); // Sanity check

            #endregion

            var refund = deal.CreateRefund();
            refund.Lines.First().Quantity = 17;

            Assert.That(refund.Total(), Is.LessThan(0));

            refund.DraftRefund(bill); // The refund content doesn't matter so just reusing the receipt here

            refund.PostRefund(new TransactionEvent(refund, TransactionResult.Refunded, Guid.NewGuid(), -17m));

            Assert.That(refund.Status, Is.EqualTo(OrderStatus.Refunded));
            Assert.That(refund.Total(), Is.EqualTo(-17m));
        }

        [Test()]
        [Description("Confirms that two refunds cannot be processing at the same time")]
        public void CreateRefundReturnsAlreadyOpenRefund()
        {
            #region Completed Deal

            var adminUser = Guid.Empty;
            var client = (new Mock<ClientRef>()).Object; // Doesn't matter

            var deal = new MutableDeal(client, adminUser);

            Assert.That(deal.CompletedDate, Is.Null); // Sanity check

            var order = deal.CreateOrder();

            var p1 = (new Mock<Product>()).Object; // Doesn't matter
            var line1 = order.CreateLine(p1);
            line1.Price = 1;
            line1.Quantity = 55;
            Assert.That(line1.HasRestrictedRefund(), Is.False); // Sanity check
            
            var bill = new BillContent("party@domain.com");
            bill.SendTo.Add(new MailAddress("someone@somewhere.com"));

            order.DraftBill(bill, ContractType.Receipt, adminUser);

            deal.Approve(new Audit("Approved", adminUser));

            var request = order.CreateRequest(CreditCardRefFake.Instance, 55m);
            var charge = request.Complete(TransactionResult.Approved, 55m, 55m);
            order.PostCharge(charge);

            Assert.That(deal.Total(), Is.EqualTo(55m)); // Sanity check
            Assert.That(deal.Status, Is.EqualTo(DealStatus.Complete)); // Sanity check

            #endregion

            var refund = deal.CreateRefund();
            var refund2 = deal.CreateRefund();

            Assert.That(refund, Is.SameAs(refund2));
        }
    }
}
