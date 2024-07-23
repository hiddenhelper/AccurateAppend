using System;
using AccurateAppend.Core;
using Moq;
using NUnit.Framework;

namespace AccurateAppend.Sales.Tests.Unit
{
    [TestFixture()]
    public class LedgerEntryTests
    {
        [Test()]
        [Description("The entry will call check the effective dates are all valid for the service account that was provided")]
        public void LedgerConfirmsThatPeriodIsValidForAccount()
        {
            var accountMock = new Mock<RecurringBillingAccount>(MockBehavior.Strict);
            accountMock.Setup(m => m.IsValidForPeriod(It.IsAny<DateSpan>())).Returns(false);
            accountMock.Setup(m => m.EffectiveDate).Returns(DateTime.MinValue); // Need to set this up for exception message
            accountMock.Setup(m => m.EndDate).Returns((DateTime?)null); // Need to set this up for exception message

            var dealMock = new Mock<LedgerDeal>();

            var period = new DateSpan(DateTime.Now, DateTime.Now); // Doesn't matter
            try
            {
                LedgerEntry.ForUsage(accountMock.Object, dealMock.Object, period);
                Assert.Fail("Exception should have been thrown");
                //Note: we only need to test one factory as they all call into the same constructor
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.That(ex.ParamName, Is.EqualTo("period"));
            }
        }

        [Test()]
        [Description("Confirms that the datespan is in billing time zone and that the time portion is gone")]
        public void FactoryShouldConvertPeriodIntoBillingDate()
        {
            var accountMock = new Mock<RecurringBillingAccount>(MockBehavior.Strict);
            accountMock.Setup(m => m.IsValidForPeriod(It.IsAny<DateSpan>())).Returns(true); // We don't care to test this check, just that it gets called

            var dealMock = new Mock<LedgerDeal>();

            var start = DateTime.SpecifyKind(new DateTime(2017, 1, 1, 1, 0, 0), DateTimeKind.Utc);
            var end = start.AddMonths(1).AddDays(-1); //Last day of previous month

            var period = new DateSpan(start, end);

            Assert.That(start.TimeOfDay, Is.GreaterThan(TimeSpan.Zero)); // sanity check

            var entry = LedgerEntry.ForUsage(accountMock.Object, dealMock.Object, period);

            Assert.That(entry.PeriodEnd.TimeOfDay, Is.EqualTo(TimeSpan.Zero)); // No time portion
            var ee = TimeZoneInfo.ConvertTime(end, DateTimeExtensions.BillingZone()).Date;
            Assert.That(entry.PeriodEnd, Is.EqualTo(ee));
        }
    }
}
