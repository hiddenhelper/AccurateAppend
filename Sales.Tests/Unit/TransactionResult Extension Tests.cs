using System;
using System.Linq;
using AccurateAppend.Core;
using AccurateAppend.Core.Definitions;
using NUnit.Framework;

namespace AccurateAppend.Sales.Tests.Unit
{
    [TestFixture()]
    public class TransactionResult_Extension_Tests
    {
        [Test()]
        public void IsCaptured()
        {
            Assert.IsTrue(TransactionResult.Approved.IsCaptured());
            Assert.IsTrue(TransactionResult.Voided.IsCaptured());
        }

        [Test()]
        public void IsNotCaptured()
        {
            var values = Enum.GetNames(typeof(TransactionResult))
                .Select(EnumExtensions.Parse<TransactionResult>)
                .Where(e => e != TransactionResult.Approved && e != TransactionResult.Voided);

            foreach (var value in values)
            {
                Assert.IsFalse(value.IsCaptured());
            }
        }
    }
}
