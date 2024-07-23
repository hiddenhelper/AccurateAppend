using System;
using System.Net.Mail;
using AccurateAppend.Core.Definitions;
using Moq;
using NUnit.Framework;

namespace AccurateAppend.Sales.Tests.Functional
{
    [TestFixture()]
    public class Deal_Charge_Billing_Scenario
    {
        [Test()]
        public void ChargeInFull()
        {
            var adminUser = Guid.Empty;
            var client = (new Mock<ClientRef>()).Object; // Doesn't matter

            var deal = new MutableDeal(client, adminUser);

            Assert.That(deal.CompletedDate, Is.Null); // Sanity check

            var order = deal.CreateOrder();

            var p1 = (new Mock<Product>()).Object; // Doesn't matter
            var line = order.CreateLine(p1);
            line.Price = 1;
            line.Quantity = 55;

            var bill = new BillContent("party@domain.com");
            bill.SendTo.Add(new MailAddress("someone@somewhere.com"));

            order.DraftBill(bill, ContractType.Receipt, adminUser);

            deal.Approve(new Audit("Approved", adminUser));

            Assert.That(deal.Total(), Is.EqualTo(55M)); // Sanity check
            Assert.That(order.OutstandingTotal(), Is.EqualTo(55M)); // Sanity check
            Assert.That(deal.Status, Is.EqualTo(DealStatus.Billing)); // Sanity check

            var request = order.CreateRequest(CreditCardRefFake.Instance, 55m);
            Assert.That(order.PendingTransactions, Is.Not.Empty);
            Assert.That(order.Transactions, Is.Empty);
            Assert.That(order.OutstandingTotal(), Is.EqualTo(0m)); // Sanity check

            var charge = request.Complete(TransactionResult.Approved, 55, 55);
            order.PostCharge(charge);

            Assert.That(order.PendingTransactions, Is.Empty);
            Assert.That(order.Transactions, Is.Not.Empty);
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Billed));
            Assert.That(deal.Status, Is.EqualTo(DealStatus.Complete));
            Assert.That(deal.CompletedDate, Is.Not.Null);
            Assert.That(order.OutstandingTotal(), Is.EqualTo(0m));
        }

        [Test()]
        public void PartialBilling()
        {
            var adminUser = Guid.Empty;
            var client = (new Mock<ClientRef>()).Object; // Doesn't matter

            var deal = new MutableDeal(client, adminUser);

            Assert.That(deal.CompletedDate, Is.Null); // Sanity check

            var order = deal.CreateOrder();

            var p1 = (new Mock<Product>()).Object; // Doesn't matter
            var line = order.CreateLine(p1);
            line.Price = 1;
            line.Quantity = 55;

            var bill = new BillContent("party@domain.com");
            bill.SendTo.Add(new MailAddress("someone@somewhere.com"));

            order.DraftBill(bill, ContractType.Receipt, adminUser);

            deal.Approve(new Audit("Approved", adminUser));

            Assert.That(deal.Total(), Is.EqualTo(55M)); // Sanity check
            Assert.That(deal.Status, Is.EqualTo(DealStatus.Billing)); // Sanity check

            // post $5 charge
            var request = order.CreateRequest(CreditCardRefFake.Instance, 5);
            Assert.That(order.OutstandingTotal(), Is.EqualTo(50m));
            Assert.That(order.PendingTransactions, Is.Not.Empty);
            Assert.That(order.Transactions, Is.Empty);

            var charge = request.Complete(TransactionResult.Approved, 5, 5);
            order.PostCharge(charge);

            // Still billing-not done
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Open));
            Assert.That(deal.Status, Is.EqualTo(DealStatus.Billing));
            Assert.That(deal.CompletedDate, Is.Null);
            Assert.That(order.OutstandingTotal(), Is.EqualTo(50m));
            Assert.That(order.PendingTransactions, Is.Empty);
            Assert.That(order.Transactions, Is.Not.Empty);

            // post $50 charge
            request = order.CreateRequest(CreditCardRefFake.Instance, 50m);
            Assert.That(order.PendingTransactions, Is.Not.Empty);
            Assert.That(order.OutstandingTotal(), Is.EqualTo(0m));

            charge = request.Complete(TransactionResult.Approved, 50, 50);
            order.PostCharge(charge);

            // Now should be complete
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Billed));
            Assert.That(deal.Status, Is.EqualTo(DealStatus.Complete));
            Assert.That(deal.CompletedDate, Is.Not.Null);
            Assert.That(order.OutstandingTotal(), Is.EqualTo(0m));
            Assert.That(order.PendingTransactions, Is.Empty);
            Assert.That(order.Transactions, Has.Count.EqualTo(2));
        }

        [Test()]
        public void DeclinedBilling()
        {
            var adminUser = Guid.Empty;
            var client = (new Mock<ClientRef>()).Object; // Doesn't matter

            var deal = new MutableDeal(client, adminUser);

            Assert.That(deal.CompletedDate, Is.Null); // Sanity check

            var order = deal.CreateOrder();

            var p1 = (new Mock<Product>()).Object; // Doesn't matter
            var line = order.CreateLine(p1);
            line.Price = 1;
            line.Quantity = 55;

            var bill = new BillContent("party@domain.com");
            bill.SendTo.Add(new MailAddress("someone@somewhere.com"));

            order.DraftBill(bill, ContractType.Receipt, adminUser);

            deal.Approve(new Audit("Approved", adminUser));

            Assert.That(deal.Total(), Is.EqualTo(55M)); // Sanity check
            Assert.That(deal.Status, Is.EqualTo(DealStatus.Billing)); // Sanity check

            // post $5 charge
            var request = order.CreateRequest(CreditCardRefFake.Instance, 5);
            Assert.That(order.OutstandingTotal(), Is.EqualTo(50m));
            Assert.That(order.PendingTransactions, Is.Not.Empty);
            Assert.That(order.Transactions, Is.Empty);

            var charge = request.Complete(TransactionResult.Approved, 5, 5);
            order.PostCharge(charge);

            // Still billing-not done
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Open));
            Assert.That(deal.Status, Is.EqualTo(DealStatus.Billing));
            Assert.That(deal.CompletedDate, Is.Null);
            Assert.That(order.OutstandingTotal(), Is.EqualTo(50m));
            Assert.That(order.PendingTransactions, Is.Empty);
            Assert.That(order.Transactions, Is.Not.Empty);

            // post $50 charge
            request = order.CreateRequest(CreditCardRefFake.Instance, 50m);
            Assert.That(order.PendingTransactions, Is.Not.Empty);
            Assert.That(order.OutstandingTotal(), Is.EqualTo(0m));

            charge = request.Complete(TransactionResult.Declined, 50, null);
            order.PostCharge(charge);

            // Should be not complete
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Open));
            Assert.That(deal.Status, Is.EqualTo(DealStatus.Billing));
            Assert.That(deal.CompletedDate, Is.Null);
            Assert.That(order.OutstandingTotal(), Is.EqualTo(50m));
            Assert.That(order.PendingTransactions, Is.Empty);
            Assert.That(order.Transactions, Has.Count.EqualTo(2));
        }

        [Test()]
        public void AttemptToOvercharge()
        {
            var adminUser = Guid.Empty;
            var client = (new Mock<ClientRef>()).Object; // Doesn't matter

            var deal = new MutableDeal(client, adminUser);

            Assert.That(deal.CompletedDate, Is.Null); // Sanity check

            var order = deal.CreateOrder();

            var p1 = (new Mock<Product>()).Object; // Doesn't matter
            var line = order.CreateLine(p1);
            line.Price = 1;
            line.Quantity = 55;

            var bill = new BillContent("party@domain.com");
            bill.SendTo.Add(new MailAddress("someone@somewhere.com"));

            order.DraftBill(bill, ContractType.Receipt, adminUser);

            deal.Approve(new Audit("Approved", adminUser));

            Assert.That(deal.Total(), Is.EqualTo(55M)); // Sanity check
            Assert.That(deal.Status, Is.EqualTo(DealStatus.Billing)); // Sanity check

            // post $5 charge
            var request = order.CreateRequest(CreditCardRefFake.Instance, 5);
            Assert.That(order.OutstandingTotal(), Is.EqualTo(50m));
            Assert.That(order.PendingTransactions, Is.Not.Empty);
            Assert.That(order.Transactions, Is.Empty);

            var charge = request.Complete(TransactionResult.Approved, 5, 5);
            order.PostCharge(charge);

            // Still billing-not done
            Assert.That(order.Status, Is.EqualTo(OrderStatus.Open));
            Assert.That(deal.Status, Is.EqualTo(DealStatus.Billing));
            Assert.That(deal.CompletedDate, Is.Null);
            Assert.That(order.OutstandingTotal(), Is.EqualTo(50m));
            Assert.That(order.PendingTransactions, Is.Empty);
            Assert.That(order.Transactions, Is.Not.Empty);

            try
            {
                // post $55 overcharge
                order.CreateRequest(CreditCardRefFake.Instance, 55m);
                Assert.Fail("Exception should have been thrown");
            }
            catch (InvalidOperationException ex)
            {
                Assert.That(ex.Message,Is.EqualTo("Total charge of $55.00 exceeds remaining order total of $50.00"));
            }
        }
    }
}
