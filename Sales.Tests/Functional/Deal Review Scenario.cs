using System;
using System.Linq;
using System.Net.Mail;
using Moq;
using NUnit.Framework;

namespace AccurateAppend.Sales.Tests.Functional
{
    [TestFixture()]
    public class Deal_Review_Scenario
    {
        [Test()]
        [Description("Confirms that a review cycle will create the needed audit logs and roll back any draft emails")]
        public void Decline()
        {
            var dealReviewedEventWasRaised = false;
            var draftClearedEventWasRaised = false;

            var adminUser = Guid.Empty;
            var client = (new Mock<ClientRef>()).Object; // Doesn't matter

            var deal = new MutableDeal(client, adminUser);
            deal.Reviewed += (s, e) => { dealReviewedEventWasRaised = true; };

            var order = deal.CreateOrder();
            order.Complete += (s, e) => { Assert.Fail("Order Compled Called"); };

            var p1 = (new Mock<Product>()).Object; // Doesn't matter
            order.CreateLine(p1, 55);

            var bill = new BillContent("party@domain.com");
            bill.DraftCleared += (s, e) => { draftClearedEventWasRaised = true; };

            order.DraftBill(bill, ContractType.Invoice, adminUser);

            deal.Decline(new Audit("Declined", adminUser));
            
            Assert.That(deal.Notes.First().CreatedBy, Is.EqualTo(adminUser));
            Assert.That(deal.Notes.First().Content, Is.EqualTo("Send to review"));

            Assert.That(deal.Notes.Skip(1).First().CreatedBy, Is.EqualTo(adminUser));
            Assert.That(deal.Notes.Skip(1).First().Content, Is.EqualTo("Declined"));

            Assert.That(order.Content, Is.Null);
            Assert.That(order.Bill.ContractType, Is.Null);

            Assert.That(dealReviewedEventWasRaised);
            Assert.That(draftClearedEventWasRaised);
        }

        [Test()]
        [Description("Confirms that approving an invoice will complete the deal and send the invoice emails")]
        public void Approve_Invoice()
        {
            var dealReviewedEventWasRaised = false;
            var completeEventWasRaised = false;

            var adminUser = Guid.Empty;
            var client = (new Mock<ClientRef>()).Object; // Doesn't matter

            var deal = new MutableDeal(client, adminUser);
            deal.Reviewed += (s, e) => { dealReviewedEventWasRaised = true; };

            Assert.That(deal.CompletedDate, Is.Null); // Sanity check

            var order = deal.CreateOrder();
            order.Complete += (s, e) => { completeEventWasRaised = true; };

            var p1 = (new Mock<Product>()).Object; // Doesn't matter
            order.CreateLine(p1, 55);

            var bill = new BillContent("party@domain.com");
            bill.SendTo.Add(new MailAddress("party@domain.com"));

            bill.DraftCleared += (s, e) => { Assert.Fail("Bill DraftCleared Called"); };

            order.DraftBill(bill, ContractType.Invoice, adminUser);

            deal.Approve(new Audit("Approved", adminUser));

            Assert.That(deal.Notes.First().CreatedBy, Is.EqualTo(adminUser));
            Assert.That(deal.Notes.First().Content, Is.EqualTo("Send to review"));

            Assert.That(deal.Notes.Skip(1).First().CreatedBy, Is.EqualTo(adminUser));
            Assert.That(deal.Notes.Skip(1).First().Content, Is.EqualTo("Approved"));

            Assert.That(order.Content, Is.Not.Null);
            
            Assert.That(order.Bill.ContractType, Is.EqualTo(ContractType.Invoice));

            Assert.That(deal.Status,Is.EqualTo(DealStatus.Complete));
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Billed));
            Assert.That(deal.CompletedDate, Is.Not.Null);

            Assert.That(dealReviewedEventWasRaised);
            Assert.That(completeEventWasRaised);
        }

        [Test()]
        [Description("Confirms that approving a card bill will move to billing status")]
        public void Approve_Receipt()
        {
            var dealReviewedEventWasRaised = false;
            
            var adminUser = Guid.Empty;
            var client = (new Mock<ClientRef>()).Object; // Doesn't matter

            var deal = new MutableDeal(client, adminUser);
            deal.Reviewed += (s, e) => { dealReviewedEventWasRaised = true; };

            var order = deal.CreateOrder();
            order.Complete += (s, e) => { Assert.Fail("Bill DraftCleared Called"); };

            var p1 = (new Mock<Product>()).Object; // Doesn't matter
            order.CreateLine(p1, 55);

            var bill = new BillContent("party@domain.com");
            bill.SendTo.Add(new MailAddress("party@domain.com"));

            bill.DraftCleared += (s, e) => { Assert.Fail("Bill DraftCleared Called"); };

            order.DraftBill(bill, ContractType.Receipt, adminUser);

            deal.Approve(new Audit("Approved", adminUser));

            Assert.That(deal.Notes.First().CreatedBy, Is.EqualTo(adminUser));
            Assert.That(deal.Notes.First().Content, Is.EqualTo("Send to review"));

            Assert.That(deal.Notes.Skip(1).First().CreatedBy, Is.EqualTo(adminUser));
            Assert.That(deal.Notes.Skip(1).First().Content, Is.EqualTo("Approved"));
            
            Assert.That(order.Content, Is.Not.Null);

            Assert.That(order.Bill.ContractType, Is.EqualTo(ContractType.Receipt));

            Assert.That(deal.Status, Is.EqualTo(DealStatus.Billing));
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Open));

            Assert.That(dealReviewedEventWasRaised);
        }
    }
}
