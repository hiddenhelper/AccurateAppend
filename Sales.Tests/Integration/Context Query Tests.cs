using System;
using System.Data.SqlClient;
using System.Linq;
using AccurateAppend.Sales.DataAccess;
using NUnit.Framework;

namespace AccurateAppend.Sales.Tests.Integration
{
    [TestFixture()]
    [Ignore("Only used to spike code")]
    public class ContextQueryTests
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
        public void QueryRefs()
        {
            using (var db = new DefaultContext(this.ConnectionString))
            {
                var query = db.SetOf<SubscriptionBilling>().Take(10).ToArray();
                var query2 = db.SetOf<BillContent>().Take(10).ToArray();
                var query3 = db.SetOf<DealBinder>().Take(10).ToArray();
            }
        }
    }
}
