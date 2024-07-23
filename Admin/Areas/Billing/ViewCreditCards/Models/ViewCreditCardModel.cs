using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using AccurateAppend.ChargeProcessing;
using AccurateAppend.Core;
using AccurateAppend.Security;
using AccurateAppend.Websites.Admin.ViewModels.Common;

namespace AccurateAppend.Websites.Admin.Areas.Billing.ViewCreditCards.Models
{
    public class ViewCreditCardModel
    {
        #region Fields

        private Party<Int32> billTo;
        private AddressModel address;
        private String displayValue;
        private String securityCode;
        private String expiration;
        private Boolean isPrimary;
        private Boolean canMakePrimary;
        private String externalProfileId;
        private Boolean canUpdateBilling;

        #endregion

        #region Constructor
        
        public ViewCreditCardModel(ChargePayment account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            Contract.EndContractBlock();

            this.isPrimary = account.IsPrimary;
            Party party = account.BillTo;
            this.billTo = new Party<Int32>
            {
                BusinessName = party.BusinessName,
                Email = party.DefaultEmail,
                FirstName = party.FirstName.ToTitleCase(),
                LastName = party.LastName.ToTitleCase(),
            };

            this.billTo.Id = account.Id.Value;
            this.billTo.PhoneNumber = (new PhoneNumber {Value = account.BillTo.PhoneNumber}).ToString();

            this.displayValue = $"XXXX-XXXX-XXXX-{account.Card.DisplayValue}";
            this.securityCode = account.Card.CscValue;
            this.expiration = account.Card.Expiration.ToString("MM") + @"-" + account.Card.Expiration.Year.ToString().Right(2);

            this.address = new AddressModel
            {
                City = account.BillTo.City.ToTitleCase(),
                PostalCode = account.BillTo.Zip.ToTitleCase(),
                State = account.BillTo.State,
                Street = account.BillTo.Address.ToTitleCase(),
                Country = account.BillTo.Country
            };
            this.canMakePrimary = !account.IsPrimary && account.Card.IsValid();
            this.canUpdateBilling = account.Card.IsValid();

            this.externalProfileId = account.Profile?.ProfileId;
        }

        #endregion

        #region Properties

        public virtual Boolean IsPrimary
        {
            get => this.isPrimary;
            protected set => this.isPrimary = value;
        }

        public virtual Party<Int32> BillTo
        {
            get => this.billTo;
            protected set => this.billTo = value;
        }

        public virtual AddressModel Address
        {
            get => this.address;
            protected set => this.address = value;
        }

        public virtual String DisplayValue
        {
            get => this.displayValue;
            protected set => this.displayValue = value;
        }

        public virtual String SecurityCode
        {
            get => this.securityCode;
            protected set => this.securityCode = value;
        }

        public virtual String Expiration
        {
            get => this.expiration;
            protected set => this.expiration = value;
        }

        public virtual Boolean CanMakePrimary
        {
            get => this.canMakePrimary;
            protected set => this.canMakePrimary = value;
        }

        public virtual Boolean CanUpdateBilling
        {
            get => this.canUpdateBilling;
            protected set => this.canUpdateBilling = value;
        }

        public virtual String ExternalProfileId
        {
            get => this.externalProfileId;
            set => this.externalProfileId = value;
        }

        #endregion
    }

    public class ViewCreditCardsModel
    {
        private String userName;

        public ViewCreditCardsModel(ClientRef client)
        {
            this.HasCard = client.ChargePayments.Any();
            this.userName = client.UserName;
            this.UserId = client.UserId;

            this.Cards = client.ChargePayments.OrderByDescending(a => a.IsPrimary).ThenByDescending(a => a.CreatedDate).Select(a => new ViewCreditCardModel(a)).ToList();
        }

        public virtual String UserName
        {
            get => this.userName;
            protected set => this.userName = value;
        }

        public Boolean HasCard
        {
            get;
            protected set;
        }

        public Guid UserId { get; protected set; }

        public IList<ViewCreditCardModel> Cards { get; }
    }
}