using System;
using AccurateAppend.Core;
using Moq;
using NUnit.Framework;

namespace AccurateAppend.Sales.Tests.Unit
{
    [TestFixture()]
    public class CreditCardRef_Tests
    {
        [Test()]
        public void IsValidShouldBeFalseForPreviousMonths()
        {
            var date = DateTime.Today.AddMonths(-24);
            do
            {
                var mock = new Mock<CreditCardRef>();
                mock.Setup(m => m.CardExpiration).Returns($"{date.Month:D2}{date.Year}");
                mock.CallBase = true;
                var subject = mock.Object;

                Console.WriteLine(date);
                Assert.That(subject.IsValid(), Is.False);

                date = date.AddMonths(1);
            } while (date < DateTime.Today);
        }

        [Test()]
        public void IsValidShouldBeTrueForCurrentAndFutureMonths()
        {
            var date = DateTime.Today;
            do
            {
                var mock = new Mock<CreditCardRef>();
                mock.Setup(m => m.CardExpiration).Returns($"{date.Month:D2}{date.Year}");
                mock.CallBase = true;
                var subject = mock.Object;

                Assert.That(subject.IsValid(), Is.True);

                date = date.AddMonths(1);
            } while (date <= DateTime.Today.AddMonths(12));
        }

        [Test()]
        public void CurrentMonthIsValid()
        {
            var date = DateTime.Today.ToFirstOfMonth().Date;
            var mock = new Mock<CreditCardRef>();
            mock.Setup(m => m.CardExpiration).Returns($"{date.Month:D2}{date.Year}");
            mock.CallBase = true;
            var subject = mock.Object;

            Assert.That(subject.IsValid(), Is.True);
        }
    }
}
