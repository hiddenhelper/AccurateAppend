using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Messages;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Websites.Admin.Areas.Clients.ContractWizard.Models;
using AccurateAppend.Websites.Admin.ViewModels.Common;
using DomainModel.ActionResults;
using DomainModel.Messages;
using NServiceBus;

namespace AccurateAppend.Websites.Admin.Areas.Clients.ContractWizard
{
    /// <summary>
    /// Provides a step by step approach to establishing service contracts.
    /// </summary>
    [Authorize()]
    public class ContractWizardController : Controller
    {
        #region Fields

        private readonly DefaultContext context;
        private readonly IMessageSession bus;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ContractWizardController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> to use for this controller instance.</param>
        /// <param name="bus">The <see cref="IMessageSession"/> used to publish messages.</param>
        public ContractWizardController(DefaultContext context, IMessageSession bus)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            Contract.EndContractBlock();

            this.context = context;
            this.bus = bus;
        }

        #endregion

        #region Action Methods

        public virtual async Task<ActionResult> ResumeAll()
        {
            
            var keys = await context.Database
                //.SqlQuery<Guid>("SELECT PublicKey FROM sales.Subscriptions WHERE Type=1 AND (EndDate is null or EndDate >'11-30-18')") //sub
                .SqlQuery<Int32>("SELECT SubscriptionId FROM sales.Subscriptions WHERE Type=1 AND (EndDate is null or EndDate >'6-30-19')") //overage
                                                                                                                                            //.SqlQuery<Guid>("SELECT PublicKey FROM sales.Subscriptions WHERE Type=3 AND (EndDate is null or EndDate >'1-31-19')") //usage
                .ToArrayAsync();

            foreach (var key in keys)
            {
                //var command = new ResumeUsageCommand();
                //command.PublicKey = key;
                //var command = new ResumeSubscriptionCommand();
                //command.PublicKey = key;
                var command = new ResumeOverageCommand();
                command.SubscriptionId = key;

                await this.bus.SendLocal(command);
            }
            return new LiteralResult() { Data = keys.Length };
        }

        /// <summary>
        /// Starts the wizard, presenting a view to select the type of setup.
        /// </summary>
        /// <param name="userId">The identifier of the client to create the account for.</param>
        /// <param name="cancellation">Used to signal the request for cancellation of an asynchronous process.</param>
        public virtual async Task<ActionResult> Start(Guid userId, CancellationToken cancellation)
        {
            var model = new Party<Guid>();

            using (this.context.CreateScope(ScopeOptions.NoTracking))
            {
                var client = await this.context.SetOf<ClientRef>()
                        .Where(c => c.UserId == userId)
                        .SingleOrDefaultAsync(cancellation);
                if (client == null)
                {
                    this.TempData["message"] = $"The user '{userId}' does not exist ";
                    return this.View("~/Views/Shared/Error.aspx");
                }

                model.Id = client.UserId;
                model.BusinessName = client.BusinessName;
                model.Email = client.UserName;
                model.FirstName = client.FirstName;
                model.LastName = client.LastName;
                model.PhoneNumber = client.PrimaryPhone;

                return this.View(model);
            }
        }

        [HttpPost()]
        public virtual async Task<ActionResult> Details(Guid userId, ContractType type, CancellationToken cancellation)
        {
            using (this.context.CreateScope(ScopeOptions.NoTracking))
            {
                var client = await this.context
                    .SetOf<ClientRef>()
                    .Where(c => c.UserId == userId)
                    .SingleOrDefaultAsync(cancellation);
                if (client == null)
                {
                    this.TempData["message"] = $"The user '{userId}' does not exist";
                    return this.View("~/Views/Shared/Error.aspx");
                }

                var contracts = await this.context
                    .SetOf<RecurringBillingAccount>()
                    .Where(a => a.ForClient.UserId == userId)
                    .ToArrayAsync(cancellation);

                DateTime firstDate;
                if (contracts.Length == 0)
                {
                    firstDate = DateTime.Today;
                }
                else if (contracts.Any(c => c.EndDate == null))
                {
                    this.TempData["message"] = $"The user '{client.UserName}' has a current active contract already.";
                    return this.View("~/Views/Shared/Error.aspx");
                }
                else
                {
                    firstDate = contracts.Max(c => c.EndDate.Value).AddDays(1);
                }

                switch (type)
                {
                    case ContractType.Accrual:
                        var accrual = new CreateAccruingModel();
                        accrual.UserId = userId;
                        accrual.FirstAvailableDate = firstDate;
                        accrual.EffectiveDate = Core.DateTimeExtensions.Max(accrual.FirstAvailableDate, DateTime.Today.ToBillingZone().Date);
                        return this.View("AccuralDetail", accrual);

                    case ContractType.Subscription:
                        var subscription = new CreateSubscriptionModel();
                        subscription.UserId = userId;
                        subscription.Cycle = DateGrain.Month;
                        subscription.FirstAvailableDate = firstDate;
                        subscription.EffectiveDate = Core.DateTimeExtensions.Max(subscription.FirstAvailableDate, DateTime.Today.ToBillingZone().Date);
                        return this.View("SubscriptionDetail", subscription);

                    case ContractType.FixedRate:
                        var fixedRate = new CreateFixedRateModel();
                        fixedRate.UserId = userId;
                        fixedRate.Cycle = DateGrain.Month;
                        fixedRate.FirstAvailableDate = firstDate;
                        fixedRate.EffectiveDate = Core.DateTimeExtensions.Max(fixedRate.FirstAvailableDate, DateTime.Today.ToBillingZone().Date);
                        return this.View("FixedRateDetail", fixedRate);

                    case ContractType.PaidTest:
                        var trial = new CreatePaidTestModel();
                        trial.UserId = userId;
                        trial.FirstAvailableDate = firstDate;
                        trial.EffectiveDate = Core.DateTimeExtensions.Max(trial.FirstAvailableDate, DateTime.Today.ToBillingZone().Date);
                        trial.EndDate = DateTime.Today.ToBillingZone().Date.AddDays(90);
                        return this.View("PaidTestDetail", trial);

                    case ContractType.Usage:
                        var usage = new CreateUsageModel();
                        usage.UserId = userId;
                        usage.Cycle = DateGrain.Month;
                        usage.FirstAvailableDate = firstDate;
                        usage.EffectiveDate = Core.DateTimeExtensions.Max(usage.FirstAvailableDate, DateTime.Today.ToBillingZone().Date);
                        return this.View("UsageDetail", usage);

                    default:
                        throw new NotSupportedException($"{type} is not supported");
                }
            }
        }

        public virtual async Task<ActionResult> CreateSubscription(CreateSubscriptionModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("SubscriptionDetail", model);
            }

            var command = new CreateSubscriptionCommand();
            command.EffectiveDate = model.EffectiveDate;
            command.EndDate = model.EndDate;
            command.Prepayment = model.Prepayment;
            command.PublicKey = Guid.NewGuid();
            command.UserId = model.UserId;
            command.Cycle = model.Cycle;
            command.HasCustomBilling = model.HasCustomBilling;
            command.MaxBalance = model.MaxBalance;

            await this.bus.SendLocal(command);
            await Task.Delay(TimeSpan.FromSeconds(4));
            return this.RedirectToAction("Index", "ViewAccounts", new { Area = "Clients", model.UserId });
        }

        public virtual async Task<ActionResult> CreateFixedRate(CreateFixedRateModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("FixedRateDetail", model);
            }

            var command = new CreateSubscriptionCommand();
            command.EffectiveDate = model.EffectiveDate;
            command.EndDate = model.EndDate;
            command.Prepayment = model.Prepayment;
            command.PublicKey = Guid.NewGuid();
            command.UserId = model.UserId;
            command.Cycle = model.Cycle;
            command.HasCustomBilling = model.HasCustomBilling;
            command.IsFixedRate = true;

            await this.bus.SendLocal(command);
            await Task.Delay(TimeSpan.FromSeconds(4));
            return this.RedirectToAction("Index", "ViewAccounts", new { Area = "Clients", model.UserId });
        }

        public virtual async Task<ActionResult> CreateUsage(CreateUsageModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("UsageDetail", model);
            }

            var command = new CreateUsageOnlyCommand();
            command.EffectiveDate = model.EffectiveDate;
            command.EndDate = model.EndDate;
            command.PublicKey = Guid.NewGuid();
            command.UserId = model.UserId;
            command.Cycle = model.Cycle;
            command.HasCustomBilling = model.HasCustomBilling;
            command.MaxBalance = model.MaxBalance;

            await this.bus.SendLocal(command);
            await Task.Delay(TimeSpan.FromSeconds(4));
            return this.RedirectToAction("Index", "ViewAccounts", new { Area = "Clients", model.UserId });
        }

        public virtual async Task<ActionResult> CreateAccrual(CreateAccruingModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("AccuralDetail", model);
            }

            var command = new CreateAccrualCommand();
            command.EffectiveDate = model.EffectiveDate;
            command.EndDate = model.EndDate;
            command.PublicKey = Guid.NewGuid();
            command.UserId = model.UserId;
            command.MaxBalance = model.MaxBalance;

            await this.bus.SendLocal(command);
            await Task.Delay(TimeSpan.FromSeconds(4));
            return this.RedirectToAction("Index", "ViewAccounts", new { Area = "Clients", model.UserId });
        }

        public virtual async Task<ActionResult> CreateTest(CreatePaidTestModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("PaidTestDetail", model);
            }

            var command = new CreateAccrualCommand();
            command.EffectiveDate = model.EffectiveDate;
            command.EndDate = model.EndDate;
            command.PublicKey = Guid.NewGuid();
            command.UserId = model.UserId;
            command.MaxBalance = model.MaxBalance;

            await this.bus.SendLocal(command);
            await Task.Delay(TimeSpan.FromSeconds(4));
            return this.RedirectToAction("Index", "ViewAccounts", new { Area = "Clients", model.UserId });
        }

        #endregion

        #region Nested Types

        public enum ContractType
        {
            Subscription,
            FixedRate,
            Usage,
            Accrual,
            PaidTest
        }

        #endregion
    }
}