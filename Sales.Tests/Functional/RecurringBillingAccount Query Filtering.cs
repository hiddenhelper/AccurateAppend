using System;
using System.Linq;
using AccurateAppend.Sales.DataAccess;
using Moq;
using NUnit.Framework;

namespace AccurateAppend.Sales.Tests.Functional
{
    [TestFixture()]
    public class RecurringBillingAccount_Query_Filtering
    {
        [Test()]
        [Description("Performs investigation of the queryable returned by the extension method")]
        public void CanFilterForUserOnDate()
        {
            var userId = new Guid("b65ef3de-d65e-4d02-bd56-fc7a2024aceb");

            var userMock = new Mock<ClientRef>(MockBehavior.Strict);
            userMock.Setup(m => m.UserId).Returns(userId);

            var accountMock1 = new Mock<RecurringBillingAccount>(MockBehavior.Strict);
            accountMock1.Setup(m => m.ForClient).Returns(userMock.Object);
            accountMock1.Setup(m => m.EffectiveDate).Returns(DateTime.Now.AddDays(-10));
            accountMock1.Setup(m => m.EndDate).Returns(DateTime.Now.AddDays(-2));
            var accountMock2 = new Mock<RecurringBillingAccount>(MockBehavior.Strict);
            accountMock2.Setup(m => m.ForClient).Returns(userMock.Object);
            accountMock2.Setup(m => m.EffectiveDate).Returns(DateTime.Now.AddDays(-1));
            accountMock2.Setup(m => m.EndDate).Returns(new DateTime?());

            var data = new[] {accountMock1.Object, accountMock2.Object}.AsQueryable();

            // Wrong User
            var results = data.ValidForDate(Guid.Empty, DateTime.Now).ToList();
            Assert.That(results, Is.Empty);

            // Just today
            results = data.ValidForDate(userId, DateTime.Now).ToList();
            Assert.That(results, Is.Not.Empty);
            Assert.That(results, Has.Count.EqualTo(1));
            Assert.That(results.First(), Is.SameAs(data.Last()));

            // Land Before Time
            results = data.ValidForDate(userId, DateTime.MinValue).ToList();
            Assert.That(results, Is.Empty);
        }
    }
}
