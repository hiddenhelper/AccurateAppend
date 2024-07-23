using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using AccurateAppend.Sales;
using AccurateAppend.Core;

namespace AccurateAppend.Websites.Admin.Areas.Clients.ViewAccounts.Models
{
    public class ViewAccountsModel
    {
        public Guid UserId { get; set; }

        public String UserName { get; set; }

        public ICollection<AccountViewModel> Accounts { get; } = new Collection<AccountViewModel>();

        public AccountViewModel CurrentAccount
        {
            get { return this.Accounts.FirstOrDefault(a => a.IsCurrent); }
        }

        public void LoadAccount(RecurringBillingAccount account)
        {
            AccountViewModel model;
            if (account is SubscriptionBilling)
            {
                model = new AccountViewModel((SubscriptionBilling)account);
            }
            else if (account is UsageBilling)
            {
                model = new AccountViewModel((UsageBilling)account);
            }
            else if (account is AccrualBilling)
            {
                model = new AccountViewModel((AccrualBilling)account);
            }
            else
            {
                return;
            }

            this.Accounts.Add(model);
        }
    }

    public class AccountViewModel
    {
        protected AccountViewModel(RecurringBillingAccount account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            Contract.EndContractBlock();

            this.Id = account.Id.Value;
            this.EffectiveDate = account.EffectiveDate;
            this.EndDate = account.EndDate;
            this.SpecialProcessing = account.SpecialProcessing;
        }

        public AccountViewModel(SubscriptionBilling account) : this((RecurringBillingAccount) account)
        {
            this.Amount = account.PrepaymentAmount;
            this.IsFixedRate = account.FixedRate;
            this.Type = AccountType.Subscription;
            this.Recurrance = account.Recurrence;
            this.Limit = account.MaxOverageLimit;
        }

        public AccountViewModel(AccrualBilling account) : this((RecurringBillingAccount)account)
        {
            this.Amount = account.MaxAccrualAmount;
            this.Type = AccountType.Accural;
            this.Recurrance = DateGrain.Day;
        }

        public AccountViewModel(UsageBilling account) : this((RecurringBillingAccount)account)
        {
            this.Type = AccountType.UsageOnly;
            this.Recurrance = account.Recurrence;
            this.Limit = account.MaxBalance;
        }

        public DateGrain Recurrance
        {
            get; protected set;
        }

        public DateTime EffectiveDate { get; protected set; }

        public DateTime? EndDate { get; protected set; }

        public Boolean IsFixedRate { get; protected set; }

        public Boolean IsCurrent
        {
            get
            {
                if (this.EffectiveDate > DateTime.Now) return false;
                if (this.EndDate == null) return true;
                return this.EndDate.Value >= DateTime.Now;
            }
        }

        public AccountType Type { get; protected set; }

        public Decimal? Amount { get; protected set; }

        public Decimal? Limit { get; protected set; }

        public Int32 Id { get; protected set; }

        public Boolean SpecialProcessing { get; set; }
    }

    [Serializable()]
    public enum AccountType
    {
        Subscription,
        [Description("Usage Only")]
        UsageOnly,
        Accural
    }
}