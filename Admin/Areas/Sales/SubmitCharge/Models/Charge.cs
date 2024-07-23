using System;

namespace AccurateAppend.Websites.Admin.Areas.Sales.SubmitCharge.Models
{
    public class Charge
    {
        public Guid Client { get; set; }

        public Int32 Order { get; set; }

        public Decimal MaxCharge { get; set; }

        public Decimal OrderTotal { get; set; }
    }
}