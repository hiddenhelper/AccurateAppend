using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace AccurateAppend.Sales.Tests.Unit
{
    [TestFixture()]
    public class UsageContractTests
    {
        #region Helper

        protected static void SetId(RecurringBillingAccount account, Int32 id = 1)
        {
            const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            var property = typeof(RecurringBillingAccount).GetProperty(nameof(RecurringBillingAccount.Id), Flags);
            property.SetMethod.Invoke(account, new Object[] { id });
        }

        #endregion

        [Test()]
        public void DatesShouldBeConvertedToStartOfDay()
        {
            var client = new Mock<ClientRef>(); // Does not matter for test

            var start = new DateTime(2016, 08, 01, 12, 0, 0);
            var end = start.AddDays(2);

            Assert.That(start, Is.GreaterThan(start.Date)); //Sanity check
            Assert.That(end, Is.GreaterThan(end.Date)); //Sanity check

            var contract = new UsageBilling(client.Object, Guid.NewGuid(), start, end);

            Assert.That(contract.EffectiveDate, Is.EqualTo(contract.EffectiveDate.Date));
            Assert.That(contract.EndDate, Is.EqualTo(contract.EndDate.Value.Date));
        }

        [Test()]
        public void IsValidForDateIsValidForIndicatedDate()
        {
            var client = new Mock<ClientRef>(); // Does not matter for test
            
            var contract = new UsageBilling(client.Object, Guid.NewGuid(), DateTime.Now.AddDays(-1));
            Assert.That(contract.IsValidForDate(DateTime.Now), Is.True);
            Assert.That(contract.IsValidForDate(DateTime.Now.AddDays(2)), Is.True);
            Assert.That(contract.IsValidForDate(DateTime.Now.AddDays(-2)), Is.False);

            contract = new UsageBilling(client.Object, Guid.NewGuid(), DateTime.Now.AddDays(-1), DateTime.Now.AddDays(1));
            Assert.That(contract.IsValidForDate(DateTime.Now), Is.True);
            Assert.That(contract.IsValidForDate(DateTime.Now.AddDays(2)), Is.False);
            Assert.That(contract.IsValidForDate(DateTime.Now.AddDays(-2)), Is.False);
        }

        [Test()]
        [Description("Confirms an unbilled unlimited contract get the expected calculated billing for full period on usage")]
        public void BillingForNewUnlimitedContractShouldCreatePeriods()
        {
            var mockCalculator = new Mock<IClientUsageCalculator>(MockBehavior.Strict);
            mockCalculator.Setup(m => m.NewestLedgerDate(It.IsAny<Int32>(), LedgerType.ForUsage, CancellationToken.None)).Returns(Task.FromResult(new DateTime?()));

            var client = new Mock<ClientRef>(MockBehavior.Strict);
            
            var contract = new UsageBilling(client.Object, Guid.NewGuid(), new DateTime(2017, 1, 1));
            SetId(contract);

            var periods = contract.DetermineUnpaidBillingPeriods(mockCalculator.Object, CancellationToken.None).Result.ToArray();

            Assert.That(periods.Length, Is.EqualTo(1));

            var usePeriod = periods.Single(p => p.Type == LedgerType.ForUsage);
            Assert.That(usePeriod.StartingOn, Is.EqualTo(new DateTime(2017, 1, 1)));
            Assert.That(usePeriod.EndingOn, Is.EqualTo(new DateTime(2017, 1, 31)));
            Assert.That(usePeriod.WaitUntilPeriodClose, Is.True);

            mockCalculator.Verify();
        }

        [Test()]
        [Description("Confirms an unbilled but limited contract get the expected calculated billing for full period on usage")]
        public void BillingForNewLimitedContractShouldCreatePeriods()
        {
            var mockCalculator = new Mock<IClientUsageCalculator>(MockBehavior.Strict);
            mockCalculator.Setup(m => m.NewestLedgerDate(It.IsAny<Int32>(), LedgerType.ForUsage, CancellationToken.None)).Returns(Task.FromResult(new DateTime?()));

            var client = new Mock<ClientRef>(MockBehavior.Strict);
            
            var contract = new UsageBilling(client.Object, Guid.NewGuid(), new DateTime(2017, 1, 1));

            contract.ApplyLimit(100);
            SetId(contract);

            var periods = contract.DetermineUnpaidBillingPeriods(mockCalculator.Object, CancellationToken.None).Result.ToArray();

            Assert.That(periods.Length, Is.EqualTo(1));

            var usePeriod = periods.Single(p => p.Type == LedgerType.ForUsage);
            Assert.That(usePeriod.StartingOn, Is.EqualTo(new DateTime(2017, 1, 1)));
            Assert.That(usePeriod.EndingOn, Is.EqualTo(new DateTime(2017, 1, 31)));
            Assert.That(usePeriod.WaitUntilPeriodClose, Is.False);

            mockCalculator.Verify();
        }

        [Test()]
        [Description("Confirms an unbilled but limited contract with a partial usage payment in period get the residual period")]
        public void UsageShouldBillForRemainingPeriod()
        {
            var mockCalculator = new Mock<IClientUsageCalculator>(MockBehavior.Strict);
            mockCalculator.Setup(m => m.NewestLedgerDate(It.IsAny<Int32>(), LedgerType.ForUsage, CancellationToken.None)).Returns(Task.FromResult(new DateTime?(new DateTime(2017, 1, 15))));

            var client = new Mock<ClientRef>(MockBehavior.Strict);

            var contract = new UsageBilling(client.Object, Guid.NewGuid(), new DateTime(2017, 1, 1));
            SetId(contract);
            contract.ApplyLimit(100);

            var periods = contract.DetermineUnpaidBillingPeriods(mockCalculator.Object, CancellationToken.None).Result.ToArray();

            Assert.That(periods.Length, Is.EqualTo(1));

            var usePeriod = periods.Single(p => p.Type == LedgerType.ForUsage);
            Assert.That(usePeriod.StartingOn, Is.EqualTo(new DateTime(2017, 1, 1))); // The period will always be the actual full period
            Assert.That(usePeriod.EndingOn, Is.EqualTo(new DateTime(2017, 1, 31)));
            Assert.That(usePeriod.PaidThrough, Is.EqualTo(new DateTime(2017, 1, 15))); // But the current through marker will be in place

            mockCalculator.Verify();
        }
    }
}
