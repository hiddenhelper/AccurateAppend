using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Security;
using AccurateAppend.Accounting;
using AccurateAppend.ChargeProcessing.Contracts;
using AccurateAppend.Core;
using AccurateAppend.Core.IdentityModel;
using AccurateAppend.Core.Utilities;
using AccurateAppend.CustomerManagement.Contracts;
using AccurateAppend.Data;
using AccurateAppend.Security;
using AccurateAppend.Standardization;
using AccurateAppend.Websites.Clients.Areas;
using AccurateAppend.Websites.Clients.Models;
using NServiceBus;
using LeadContactMethod = AccurateAppend.Accounting.LeadContactMethod;
using LeadSource = AccurateAppend.Accounting.LeadSource;

namespace AccurateAppend.Websites.Clients.Security
{
    /// <summary>
    /// High level API for account creation.
    /// </summary>
    public class AccountSignupService
    {
        #region Fields

        private readonly IEncryptor ccEncryptor;
        private readonly ISessionContext context;
        private readonly IMembershipService ms;
        private readonly ILeadConsolidationService leadService;
        private readonly IMessageSession bus;
        private readonly IEmailStandardizer emailStandardizer;
        private readonly IEmailVerificationService verification;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountSignupService"/> class.
        /// </summary>
        public AccountSignupService(ISessionContext context, IMembershipService ms, ILeadConsolidationService leadService, IEncryptor ccEncryptor, IEmailStandardizer emailStandardizer, IEmailVerificationService verification, IMessageSession bus)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (ms == null) throw new ArgumentNullException(nameof(ms));
            if (leadService == null) throw new ArgumentNullException(nameof(leadService));
            if (ccEncryptor == null) throw new ArgumentNullException(nameof(ccEncryptor));
            if (emailStandardizer == null) throw new ArgumentNullException(nameof(emailStandardizer));
            if (verification == null) throw new ArgumentNullException(nameof(verification));
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            Contract.EndContractBlock();

            this.context = context;
            this.ms = ms;
            this.leadService = leadService;
            this.ccEncryptor = ccEncryptor;
            this.emailStandardizer = emailStandardizer;
            this.verification = verification;
            this.bus = bus;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Encapsulates the logic of account signup into a single service. This overload is used for self signup processes such as the public signup
        /// or NationBuilder accounts only. Therefore, the application is ALWAYS configured for Accurate Append.
        /// </summary>
        public virtual async Task<ClaimsPrincipal> Create(CreateAccountModelBase userModel, CancellationToken cancellation = default(CancellationToken))
        {
            await this.ValidateModel(userModel, cancellation).ConfigureAwait(false);

            var app = await this.context.SetOf<Application>()
                .ForAccurateAppend()
                .FirstAsync(cancellation);

            using (var transaction = new TransactionScope(TransactionScopeOption.RequiresNew, TransactionScopeAsyncFlowOption.Enabled))
            {
                // Check for existing user name
                var userName = new[] {userModel.Email, $"_{userModel.Email}"};

                if (await this.context.SetOf<User>().AnyAsync(u => userName.Contains(u.UserName), cancellation))
                {
                    throw new ValidationException(
                        new ValidationResult(MembershipUtilities.ErrorCodeToString(MembershipCreateStatus.DuplicateEmail),
                        new[] {MembershipUtilities.ErrorCodeToModelMember(MembershipCreateStatus.DuplicateEmail)}),
                        null,
                        null);
                }

                using (var uow = this.context.CreateScope(ScopeOptions.AutoCommit))
                {
                    #region create source lead for account

                    var lead = userModel.ToLead(app);
                    lead.ChangeOwner(WellKnownIdentifiers.SystemUserId); // self service clients are always system ownership

                    lead = await this.leadService.DeduplicateLead(lead);
                    if (lead.Id == null) this.context.SetOf<Lead>().Add(lead);

                    // I need to review just how many of these take precedence over existing lead values
                    lead.ContactMethod = LeadContactMethod.Form;
                    lead.FollowUpDate = DateTime.UtcNow.AddDays(3);

                    lead.Source = userModel.Source;
                    lead.IP = userModel.DetermineIpForCaller();

                    #endregion

                    #region create account

                    // auto-generate password
                    userModel.Password = String.IsNullOrWhiteSpace(userModel.Password) ? Login.GenerateTempPassword() : userModel.Password;

                    var createStatus = await this.ms.CreateUserAsync(userModel.Email, userModel.Password, app.Id, cancellation, true);  // user must change password the first login

                    if (createStatus != MembershipCreateStatus.Success)
                    {
                        var result = new ValidationResult(
                            MembershipUtilities.ErrorCodeToString(createStatus),
                            new[] {MembershipUtilities.ErrorCodeToModelMember(createStatus)});

                        // registration error
                        throw new ValidationException(result, null, null);
                    }

                    var logon = await this.context.SetOf<User>().SingleAsync(u => u.UserName == userModel.Email, cancellation);
                    var userid = logon.Id;

                    #endregion

                    #region user details

                    var userdetail = lead.ConvertToClient(logon);
                    userdetail.IsApproved = true;
                    userdetail.AllowDataRetention = true;

                    this.context.SetOf<Client>().Add(userdetail);

                    await uow.CommitAsync(cancellation);
                    
                    #endregion

                    #region Send Bus

                    var @event = new AccountCreatedEvent
                    {
                        Channel = this.SelectChannelForSource(userModel.Source),
                        ApplicationId = app.Id,
                        GeneratedPassword = userModel.Password,
                        UserName = userModel.Email,
                        UserId = userid,
                        DateCreated = DateTime.UtcNow
                    };

                    await this.bus.Publish(@event);

                    #endregion
                }

                transaction.Complete();
            }

            return await this.ms.GetPrincipalAsync(userModel.Email, System.Security.Claims.AuthenticationTypes.Password, cancellation);
        }

        /// <summary>
        /// Encapsulates the logic of account signup into a single service. This overload is used for directed signup processes such as the classic
        /// signup form. Therefore, the model lead identifier public key are matched to the supplied <paramref name="userModel"/>.
        /// </summary>
        public virtual async Task<ClaimsPrincipal> Create(UserModel userModel, CancellationToken cancellation = default(CancellationToken))
        {
            await this.ValidateModel(userModel, cancellation).ConfigureAwait(false);

            var lead = await this.context
                .SetOf<Lead>()
                .Where(l => l.IsDeleted == null || !l.IsDeleted.Value)
                .Where(l => l.PublicKey == userModel.PublicKey)
                .Where(l => l.Status != LeadStatus.ConvertedToCustomer)
                .Include(l => l.Application)
                .FirstOrDefaultAsync(cancellation);
            if (lead == null) throw new InvalidOperationException("This link information cannot be found.");
            if (lead.Status == LeadStatus.ConvertedToCustomer) throw new InvalidOperationException("This link has already been used to create an account. If you need assistance logging in, please contact customer support.");

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                #region create account

                // auto-generate password
                userModel.Password = String.IsNullOrWhiteSpace(userModel.Password) ? Login.GenerateTempPassword() : userModel.Password;

                var email = userModel.Email;
                var createStatus = await this.ms.CreateUserAsync(email, userModel.Password, lead.Application.Id, cancellation);

                if (createStatus != MembershipCreateStatus.Success)
                {
                    var result = new ValidationResult(
                        MembershipUtilities.ErrorCodeToString(createStatus),
                        new[] { MembershipUtilities.ErrorCodeToModelMember(createStatus) });

                    // registration error
                    throw new ValidationException(result, null, null);
                }

                var uow = this.context.CreateScope();

                var logon = await this.context.SetOf<User>().SingleAsync(u => u.UserName == email, cancellation);
                var userid = logon.Id;

                #region user details

                var userdetail = lead.ConvertToClient(logon);
                Debug.Assert(lead.Status == LeadStatus.ConvertedToCustomer);
                userdetail.IsApproved = true;
                userdetail.AllowDataRetention = true;

                this.context.SetOf<Client>().Add(userdetail);

                #endregion

                #region user card

                if (!String.IsNullOrEmpty(userModel.CardNumber))
                {
                    var command = new CreatePaymentProfileCommand
                    {
                        BillingAddress = new BillingAddressPayload
                        {
                            BusinessName = userModel.BusinessName.ToTitleCase(),
                            FirstName = userModel.CardHolderFirstName.ToTitleCase(),
                            LastName = userModel.CardHolderLastName.ToTitleCase(),
                            PhoneNumber = userModel.CardHolderPhone,
                            Address = userModel.CardAddress.ToTitleCase(),
                            City = userModel.CardCity.ToTitleCase(),
                            State = userModel.CardState,
                            PostalCode = userModel.CardPostalCode,
                            Country = userModel.CardCountry
                        },

                        Card = new CreditCardPayload(this.ccEncryptor.SymetricEncrypt(userModel.CardNumber), userModel.GetExpirationDate(), userModel.CardCvv),

                        RequestId = Guid.NewGuid(),
                        UserId = userdetail.Logon.Id
                    };

                    await this.bus.Send(command);
                }

                #endregion

                #region Raise Bus Events

                var @event = new AccountCreatedEvent
                {
                    Channel = this.SelectChannelForSource(userModel.Source),
                    ApplicationId = lead.Application.Id,
                    GeneratedPassword = userModel.Password,
                    UserName = userModel.Email,
                    FirstName = userModel.FirstName,
                    LastName = userModel.LastName,
                    UserId = userid,
                    SourceLeadKey = lead.PublicKey,
                    DateCreated = DateTime.UtcNow
                };

                await this.bus.Publish(@event);

                #endregion

                await uow.CommitAsync(cancellation);

                transaction.Complete();

                #endregion
            }

            return await this.ms.GetPrincipalAsync(userModel.Email, System.Security.Claims.AuthenticationTypes.Password, cancellation);
        }

        /// <summary>
        /// Uses a <see cref="LeadSource"/> value to determine the appropriate <see cref="AccountSourceChannel"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to Sales.
        /// </remarks>
        /// <param name="source">The <see cref="LeadSource"/> to convert to the appropriate <see cref="AccountSourceChannel"/> value.</param>
        protected virtual AccountSourceChannel SelectChannelForSource(LeadSource source)
        {
            switch (source)
            {
                case LeadSource.Direct:
                    return AccountSourceChannel.SelfService;
                case LeadSource.NationBuilder:
                    return AccountSourceChannel.NationBuilder;
                case LeadSource.LeadPhilanthropy:
                    return AccountSourceChannel.LeadPhilanthropy;
                default:
                    Trace.TraceInformation($"LeadSource={source}, assuming sales directed signup");
                    return AccountSourceChannel.Sales;
            }
        }

        private async Task<Boolean> CheckEmailDeliverable(String email, CancellationToken cancellation)
        {
            const ResultCode Verified = ResultCode.E10;
            const ResultCode Disposable = ResultCode.E3;
            const ResultCode UnableToVerify = ResultCode.E7;

            var response = await this.emailStandardizer.VerifyAsync(email, this.verification, false, cancellation).ConfigureAwait(false);
            var emailIsDeliverable = response.Results.Any(a => a.Code == Verified || a.Code == Disposable || a.Code == UnableToVerify);

            return emailIsDeliverable;
        }

        /// <summary>
        /// Performs validation routines on the supplied <paramref name="userModel"/>.
        /// </summary>
        /// <param name="userModel">The input model that will be validated.</param>
        /// <param name="cancellation">The <see cref="CancellationToken"/> used to cancel an asynchronous operation.</param>
        /// <exception cref="ValidationException">Thrown when the provided <paramref name="userModel"/> fails required validations.</exception>
        protected virtual async Task ValidateModel(CreateAccountModelBase userModel, CancellationToken cancellation)
        {
            var isDeliverable = await this.CheckEmailDeliverable(userModel.Email, cancellation).ConfigureAwait(false);
            if (!isDeliverable)
            {
                var result = new ValidationResult(
                    "Please enter a valid deliverable email address.",
                    new[] { nameof(CreateAccountModelBase.Email) });

                // validation error
                throw new ValidationException(result, null, null);
            }
        }

        #endregion
    }
}
