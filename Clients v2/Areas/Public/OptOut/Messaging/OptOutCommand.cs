using System;
using AccurateAppend.Standardization;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.Public.OptOut.Messaging
{
    [Serializable()]
    public class OptOutCommand : ICommand
    {
        public String FirstName { get; set; } = String.Empty;

        public String LastName { get; set; } = String.Empty;

        public String Address { get; set; } = String.Empty;

        public String City { get; set; } = String.Empty;

        public String State { get; set; } = String.Empty;

        public String PostalCode { get; set; } = String.Empty;

        public String Phone { get; set; } = String.Empty;

        public String Email { get; set; } = String.Empty;

        public String Comments { get; set; } = String.Empty;

        public void Standardize(INameStandardizer nameStandardizer, IAddressStandardizer addresStandardizer)
        {
            var nameResult = nameStandardizer.Parse($"{this.FirstName} {this.LastName}");

            this.FirstName = nameResult.FirstName;
            this.LastName = nameResult.LastName;

            var adddressResult = addresStandardizer.Parse(this.Address, this.City, this.State, this.PostalCode);
            this.Address = adddressResult.Address;
            this.City = adddressResult.City;
            this.State = adddressResult.State;
            this.PostalCode = adddressResult.Zip;
        }
    }
}