using System;
using AccurateAppend.Core;
using NUnit.Framework;

namespace AccurateAppend.Sales.Tests.Unit
{
    [TestFixture()]
    public class BillingPeriodTests
    {
        [Test()]
        public void IsClosed()
        {
            var period = new BillingPeriod();
            period.PaidThrough = new DateTime(2017, 1, 31);
            period.StartingOn = new DateTime(2017, 2, 1);
            period.EndingOn = new DateTime(2017, 2, 28);
            period.WaitUntilPeriodClose = true;
            period.Type = LedgerType.ForUsage;

            Assert.That(period.IsClosed(), Is.True);

            period.EndingOn = DateTime.Now.AddDays(1);

            Assert.That(period.IsClosed(), Is.False);
        }

        [Test()]
        public void ToOutstandingRangeUsesLargerOfStartValue()
        {
            var period = new BillingPeriod();
            period.StartingOn = new DateTime(2017, 2, 1);
            period.EndingOn = new DateTime(2017, 2, 28);
            period.WaitUntilPeriodClose = true;
            period.Type = LedgerType.ForUsage;

            Assert.That(period.StartingOn, Is.GreaterThan(period.PaidThrough)); // Sanity check

            var range = period.ToOutstandingRange();
            Assert.That(range.StartingOn, Is.EqualTo(period.StartingOn));

            period.PaidThrough = new DateTime(2017, 2, 10);

            range = period.ToOutstandingRange();
            Assert.That(range.StartingOn, Is.EqualTo(period.PaidThrough.AddDays(1)));
        }

        [Test()]
        public void ToOutstandingRangeCanCaluculateRemainder()
        {
            var period = new BillingPeriod();
            period.StartingOn = new DateTime(2017, 2, 1);
            period.EndingOn = new DateTime(2017, 2, 28);
            period.WaitUntilPeriodClose = true;
            period.Type = LedgerType.ForUsage;

            var range = period.ToOutstandingRange();
            Assert.That(range.StartingOn, Is.EqualTo(period.StartingOn));
            Assert.That(range.EndingOn, Is.EqualTo(period.EndingOn));

            period.PaidThrough = new DateTime(2017, 2, 10);

            range = period.ToOutstandingRange();
            Assert.That(range.StartingOn, Is.EqualTo(period.PaidThrough.AddDays(1)));
            Assert.That(range.EndingOn, Is.EqualTo(period.EndingOn));
        }

        [Test()]
        public void ToOutstandingRangeCanHandlePaidPeriod()
        {
            var period = new BillingPeriod();
            period.StartingOn = new DateTime(2017, 2, 1);
            period.EndingOn = new DateTime(2017, 2, 28);
            period.PaidThrough = new DateTime(2017, 2, 28);
            period.WaitUntilPeriodClose = true;
            period.Type = LedgerType.ForUsage;

            var range = period.ToOutstandingRange();
            Assert.That(range.StartingOn, Is.EqualTo(period.PaidThrough.AddDays(1)));
            Assert.That(range.EndingOn, Is.EqualTo(period.EndingOn));
        }

        [Test()]
        public void IsLogicalPeriod()
        {
            var period = new BillingPeriod();
            period.EndingOn = new DateTime(2017, 2, 1);
            period.StartingOn = new DateTime(2017, 2, 28);

            Assert.That(period.IsLogicalDateRange(), Is.False);
        }

        [Test()]
        public void FromDateSpanGeneratesExpectedValue()
        {
            var span = new DateSpan();
            span.EndingOn = new DateTime(2017, 2, 1);
            span.StartingOn = new DateTime(2017, 2, 28);

            var period = BillingPeriod.FromDateSpan(span, LedgerType.GeneralAdjustment);

            Assert.That(period.StartingOn, Is.EqualTo(span.StartingOn));
            Assert.That(period.StartingOn.Kind, Is.EqualTo(span.StartingOn.Kind));
            Assert.That(period.EndingOn, Is.EqualTo(span.EndingOn));
            Assert.That(period.EndingOn.Kind, Is.EqualTo(span.EndingOn.Kind));
            Assert.That(period.PaidThrough, Is.EqualTo(span.StartingOn.AddDays(-1)));
            Assert.That(period.Type, Is.EqualTo(LedgerType.GeneralAdjustment));

            // Test that time portion is maintained
            span.EndingOn = DateTime.Now;
            Assert.That(span.EndingOn.TimeOfDay, Is.Not.EqualTo(TimeSpan.Zero)); // Sanity check

            period = BillingPeriod.FromDateSpan(span, LedgerType.GeneralAdjustment);

            Assert.That(period.StartingOn, Is.EqualTo(span.StartingOn));
            Assert.That(period.StartingOn.Kind, Is.EqualTo(span.StartingOn.Kind));
            Assert.That(period.EndingOn, Is.EqualTo(span.EndingOn));
            Assert.That(period.EndingOn.Kind, Is.EqualTo(span.EndingOn.Kind));
            Assert.That(period.PaidThrough, Is.EqualTo(span.StartingOn.AddDays(-1)));
            Assert.That(period.Type, Is.EqualTo(LedgerType.GeneralAdjustment));
        }
    }
}
