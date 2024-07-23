using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Sales.DataAccess;
using NUnit.Framework;

namespace AccurateAppend.Sales.Tests.Integration
{
    [Ignore("Integration tests are run on demand")]
    [TestFixture()]
    public class RateCards
    {
        public String ConnectionString
        {
            get
            {
                var builder = new SqlConnectionStringBuilder
                {
                    MultipleActiveResultSets = true,
                    DataSource = "localhost",
                    InitialCatalog = "AccurateAppendDB",
                    IntegratedSecurity = true
                };

                return builder.ToString();
            }
        }

        [Test()]
        public void CustomRateCardsDynamicallyDeterminePricingModel()
        {
            using (var context = new DefaultContext(this.ConnectionString))
            {
                var client = context.SetOf<ClientRef>().First(c => c.UserId == new Guid("2f4c125c-d87b-4a64-b345-29d2c8d84715"));
                var costService = new CustomerCostService(client, context);

                var rateCard = costService.CreateRateCard(DataServiceOperation.NCOA48).Result;
                Assert.That(rateCard.PricingModel, Is.EqualTo(PricingModel.Record));

                rateCard = costService.CreateRateCard(DataServiceOperation.EMAIL_BASIC_NO_SUPRESSION_W_VERIFICATION).Result;
                Assert.That(rateCard.PricingModel, Is.EqualTo(PricingModel.Match));
            }
        }

        [Test()]
        public async Task CalculatingUsageDuringKnownPeriodCreatesExpectedBill()
        {
            using (var context = new DefaultContext(this.ConnectionString))
            {
                var period = new BillingPeriod { StartingOn = new DateTime(2018, 12, 1), EndingOn = new DateTime(2018, 12, 31), Type = LedgerType.ForUsage };
                var calculator = new DefaultClientUsageCalculator(context);
                var operations = new Guid("E0301EC9-FC89-4A21-9342-E6BC0347BE91");
                var contracts = context.SetOf<RecurringBillingAccount>().Where(c => c.ForClient.UserId == operations).ToArray();
                var contract = contracts.FirstOrDefault(c => c.IsValidForDate(DateTime.Today));
                var costService = new CustomerCostService(contract.ForClient, context);

                var bill = await contract.CreateUsageBill(operations, period, calculator, costService, CancellationToken.None);
                var order = bill.WithDeal.Orders.First();

                Assert.That(order.Lines.Any(o => o.Product.Key == DataServiceOperation.EMAIL_BASIC_NO_SUPRESSION_W_VERIFICATION.ToString()));
                Assert.That(order.Lines.Any(o => o.Product.Key == DataServiceOperation.DEMOGRAHICS.ToString()));
                Assert.That(order.Lines.Any(o => o.Product.Key == DataServiceOperation.CASS.ToString()));
                Assert.That(order.Lines.Any(o => o.Product.Key == DataServiceOperation.DOB.ToString()));
                Assert.That(order.Lines.Any(o => o.Product.Key == DataServiceOperation.EMAIL.ToString()));
                Assert.That(order.Lines.Any(o => o.Product.Key == DataServiceOperation.EMAIL_BASIC_REV.ToString()));
                Assert.That(order.Lines.Any(o => o.Product.Key == DataServiceOperation.EMAIL_VER_SUPRESSION.ToString()));
                Assert.That(order.Lines.Any(o => o.Product.Key == DataServiceOperation.EMAIL_VERIFICATION.ToString()));
                Assert.That(order.Lines.Any(o => o.Product.Key == DataServiceOperation.NAME.ToString()));
                Assert.That(order.Lines.Any(o => o.Product.Key == DataServiceOperation.PHONE.ToString()));
                Assert.That(order.Lines.Any(o => o.Product.Key == DataServiceOperation.PHONE_BUS_PREM.ToString()));
                Assert.That(order.Lines.Any(o => o.Product.Key == DataServiceOperation.PHONE_DA.ToString()));
                Assert.That(order.Lines.Any(o => o.Product.Key == DataServiceOperation.PHONE_MOB.ToString()));
                Assert.That(order.Lines.Any(o => o.Product.Key == DataServiceOperation.PHONE_REV_CCO.ToString()));
                Assert.That(order.Lines.Any(o => o.Product.Key == DataServiceOperation.PHONE_REV_DA.ToString()));
                Assert.That(order.Lines.Any(o => o.Product.Key == DataServiceOperation.PHONE_REV_STD.ToString()));
                Assert.That(order.Lines.Any(o => o.Product.Key == DataServiceOperation.PHONE_STD.ToString()));

                Assert.That(order.Lines.First(o => o.Product.Key == DataServiceOperation.EMAIL_BASIC_NO_SUPRESSION_W_VERIFICATION.ToString()).Quantity, Is.EqualTo(607));
                Assert.That(order.Lines.First(o => o.Product.Key == DataServiceOperation.DEMOGRAHICS.ToString()).Quantity, Is.EqualTo(1338));
            }
        }
    }
}
