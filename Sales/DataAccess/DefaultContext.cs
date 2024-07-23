using System;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Core.ComponentModel;
using AccurateAppend.Data;

namespace AccurateAppend.Sales.DataAccess
{
    /// <summary>
    /// Provides the default EF context for the assembly.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Incorrectly flagged in CA as this type doesn't hold disposable references")]
    public class DefaultContext : ExtendedDbContext, ISessionContext
    {
        #region Fields

        private Boolean inActiveScope;
        private readonly Guid instanceId = Guid.NewGuid();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new context instance using conventions to create the name of the database to
        ///                 which a connection will be made.  The by-convention name is the full name (namespace + class name)
        ///                 of the derived context class.
        ///                 See the class remarks for how this is used to create a connection.
        /// 
        /// </summary>
        public DefaultContext()
        {
            System.Data.Entity.Database.SetInitializer<DefaultContext>(null);
        }
        
        /// <summary>
        /// Constructs a new context instance using the given string as the name or connection string for the
        ///                 database to which a connection will be made.
        ///                 See the class remarks for how this is used to create a connection.
        /// 
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        public DefaultContext(String nameOrConnectionString) : base(nameOrConnectionString)
        {
            System.Data.Entity.Database.SetInitializer<DefaultContext>(null);
        }
        
        /// <summary>
        /// Constructs a new context instance using the existing connection to connect to a database.
        ///                 The connection will not be disposed when the context is disposed.
        /// 
        /// </summary>
        /// <param name="existingConnection">An existing connection to use for the new context.</param>
        /// <param name="contextOwnsConnection">If set to <c>true</c> the connection is disposed when
        /// the context is disposed, otherwise the caller must dispose the connection.</param>
        public DefaultContext(DbConnection existingConnection, Boolean contextOwnsConnection) : base(existingConnection, contextOwnsConnection)
        {
            System.Data.Entity.Database.SetInitializer<DefaultContext>(null);
        }
        
        /// <summary>
        /// Constructs a new context instance around an existing ObjectContext.
        ///                 <param name="objectContext">An existing ObjectContext to wrap with the new context.</param><param name="dbContextOwnsObjectContext">If set to <c>true</c> the ObjectContext is disposed when
        ///                     the DbContext is disposed, otherwise the caller must dispose the connection.</param>
        /// </summary>
        public DefaultContext(ObjectContext objectContext, Boolean dbContextOwnsObjectContext) : base(objectContext, dbContextOwnsObjectContext)
        {
            System.Data.Entity.Database.SetInitializer<DefaultContext>(null);
        }

        /// <summary>
        /// Constructs a new context instance using the existing connection to connect to a database.
        ///                 The connection will be considered to be owned by this context and the connection lifetime will be managed by this type.
        /// </summary>
        /// <param name="connection">The <see cref="DbConnection"/> that will be used by this context.</param>
        public DefaultContext(DbConnection connection) : base(connection, true)
        {
            System.Data.Entity.Database.SetInitializer<DefaultContext>(null);
        }

        #endregion

        #region Overrides

        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but
        ///             before the model has been locked down and used to initialize the context.  The default
        ///             implementation of this method does nothing, but it can be overridden in a derived class
        ///             such that the model can be further configured before it is locked down.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// Typically, this method is called only once when the first instance of a derived context
        ///             is created.  The model for that context is then cached and is for all further instances of
        ///             the context in the app domain.  This caching can be disabled by setting the ModelCaching
        ///             property on the given ModelBuilder, but note that this can seriously degrade performance.
        ///             More control over caching is provided through use of the DbModelBuilder and DbContextFactory
        ///             classes directly.
        /// 
        /// </remarks>
        /// <param name="modelBuilder">The builder that defines the model for the context being created. </param>
        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Domain Models
            modelBuilder.Configurations.Add(new Mapping.AuditConfiguration());
            modelBuilder.Configurations.Add(new Mapping.BillableOrderConfiguration());
            modelBuilder.Configurations.Add(new Mapping.OrderRuntimeConfiguration());
            modelBuilder.Configurations.Add(new Mapping.ClientRefConfiguration());
            modelBuilder.Configurations.Add(new Mapping.PostalAddressRefConfiguration());
            modelBuilder.Configurations.Add(new Mapping.DealBinderConfiguration());
            modelBuilder.Configurations.Add(new Mapping.LedgerDealConfiguration());
            modelBuilder.Configurations.Add(new Mapping.RecurringBillingAccountConfiguration());
            modelBuilder.Configurations.Add(new Mapping.SubscriptionBillingConfiguration());
            modelBuilder.Configurations.Add(new Mapping.UsageBillingConfiguration());
            modelBuilder.Configurations.Add(new Mapping.AccrualBillingConfiguration());
            modelBuilder.Configurations.Add(new Mapping.LedgerEntryConfiguration());
            modelBuilder.Configurations.Add(new Mapping.MutableDealConfiguration());
            modelBuilder.Configurations.Add(new Mapping.OrderConfiguration());
            modelBuilder.Configurations.Add(new Mapping.ProductLineConfiguration());
            modelBuilder.Configurations.Add(new Mapping.ProductConfiguration());
            modelBuilder.Configurations.Add(new Mapping.ProductCategoryConfiguration());
            modelBuilder.Configurations.Add(new Mapping.BillingContractConfiguration());
            modelBuilder.Configurations.Add(new Mapping.BillContentConfiguration());
            modelBuilder.Configurations.Add(new Mapping.RefundOrderConfiguration());
            modelBuilder.Configurations.Add(new Mapping.PendingTransactionConfiguration());
            modelBuilder.Configurations.Add(new Mapping.TransactionEventConfiguration());
            modelBuilder.Configurations.Add(new Mapping.CostConfiguration());
            modelBuilder.Configurations.Add(new Mapping.CostPricingModelConfiguration());
            modelBuilder.Configurations.Add(new Mapping.CartConfiguration());
            modelBuilder.Configurations.Add(new Mapping.CsvCartConfiguration());
            modelBuilder.Configurations.Add(new Mapping.NationBuilderCartConfiguration());
            modelBuilder.Configurations.Add(new Mapping.ProductOrderConfiguration());
            modelBuilder.Configurations.Add(new Mapping.CreditCardRefConfiguration());

            // Analytic Models
            modelBuilder.Configurations.Add(new Mapping.DailyUsageRollupConfiguration());
            modelBuilder.Configurations.Add(new Mapping.BatchUsageRollupConfiguration());
            modelBuilder.Configurations.Add(new Mapping.ApiUsageRollupConfiguration());
            modelBuilder.Configurations.Add(new Mapping.ApiCallsUsageRollupConfiguration());
        }

        /// <inheritdoc />
        /// <remarks>
        /// If <paramref name="entity"/> implements <see cref="ISupportInitialize"/> the <see cref="ISupportInitialize.EndInit"/>
        /// method will be called. This base version is safe not to call (provided calls to <see cref="ISupportInitialize"/> are not
        /// needed or otherwise handled).
        /// 
        /// <see cref="BillContent.DraftCleared"/> events will be observed causing the event source entity to be removed from the context automatically.
        /// </remarks>
        /// <param name="entity">The materialized object.</param>
        protected override void OnObjectMaterialized(Object entity)
        {
            base.OnObjectMaterialized(entity);

            var bill = entity as BillContent;
            if (bill == null) return;

            bill.DraftCleared += (sender, args) => this.Set<BillContent>().Remove((BillContent)sender);
        }

        /// <inheritdoc />
        /// <remarks>
        /// Will scan for any loaded entities (<see cref="PendingTransactions"/>, <see cref="Orders"/>, and <see cref="OrderLines"/>)
        /// that have a null parent entity. This is the convention that stipulates this entity has been abandoned and should be
        /// treated as should be deleted as part of this operation (if not already).
        /// </remarks>
        protected override void OnSaving()
        {
            {
                var orphans = this.PendingTransactions.Local.Where(t => t.Order == null).ToArray();
                orphans.ForEach(t => this.PendingTransactions.Remove(t));
            }

            {
                var orphans = this.Orders.Local.Where(o => o.Deal == null).ToArray();
                orphans.ForEach(o => this.Orders.Remove(o));
            }

            {
                var orphans = this.OrderLines.Local.Where(l => l.Order == null).ToArray();
                orphans.ForEach(l => this.OrderLines.Remove(l));
            }
        }

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override String ToString()
        {
            return $"Instance {this.instanceId.ToString()}";
        }

        #endregion

        #region Entity Properties

        /// <summary>
        /// Gets a new queryable expression over the source data for the <see cref="Order"/> entity.
        /// </summary>
        /// <value>A new queryable expression over the source data for the <see cref="Order"/> entity.</value>
        protected virtual System.Data.Entity.IDbSet<Order> Orders => this.Set<Order>();

        /// <summary>
        /// Gets a new queryable expression over the source data for the <see cref="PendingTransaction"/> entity.
        /// </summary>
        /// <value>A new queryable expression over the source data for the <see cref="PendingTransaction"/> entity.</value>
        protected virtual System.Data.Entity.IDbSet<PendingTransaction> PendingTransactions => this.Set<PendingTransaction>();

        /// <summary>
        /// Gets a new queryable expression over the source data for the <see cref="ProductLine"/> entity.
        /// </summary>
        /// <value>A new queryable expression over the source data for the <see cref="ProductLine"/> entity.</value>
        protected virtual System.Data.Entity.IDbSet<ProductLine> OrderLines => this.Set<ProductLine>();
        
        #endregion

        #region ISessionContext Members

        /// <summary>
        /// Allows subclasses to provide custom logic to create the desired <see cref="IUnitOfWork"/> required.
        /// </summary>
        /// <remarks>
        /// This base implementation creates the default <see cref="AutoCommitUnitOfWork"/> if <paramref name="options"/> contains <see cref="ScopeOptions">AutoCommit</see>.
        /// If <paramref name="options"/> contains <see cref="ScopeOptions">ReadOnly</see> then a <see cref="NullUnitOfWork"/> will be returned.
        /// The base version is safe to not call provided subclasses take action to ensure nested scopes are not created.
        /// </remarks>
        /// <returns>The unit of work.</returns>
        /// <exception cref="InvalidOperationException">
        /// <para><paramref name="options"/> combines AutoCommit and NoTracking options</para>
        /// <para>-OR-</para>
        /// <para>There is already an active instance of an open <see cref="IUnitOfWork"/></para>
        /// </exception>
        protected virtual IUnitOfWork OnCreatingScope(ScopeOptions options)
        {
            Contract.Ensures(Contract.Result<IUnitOfWork>() != null);

            var shouldAutoCommit = (options.HasFlag(ScopeOptions.AutoCommit));
            //var shouldDispose = (options.HasFlag(ScopeOptions.CloseContext));
            var shouldReadOnly = (options.HasFlag(ScopeOptions.NoTracking));

            if (shouldAutoCommit && shouldReadOnly) throw new InvalidOperationException("Cannot combine AutoCommit and NoTracking options with a scope");
            if (this.inActiveScope) throw new InvalidOperationException("An active unit of work is already active. Nesting multiple unit of work instances is not supported.");

            IUnitOfWork uow;
            if (shouldAutoCommit)
            {
                uow = new AutoCommitUnitOfWork(this, false);
            }
            else if (shouldReadOnly)
            {
                uow = new NullUnitOfWork(this, false);
            }
            else
            {
                uow = new AutoCommitUnitOfWork(this, false);
            }

            ((DisposeableObject)uow).Disposed += (sender, args) => this.inActiveScope = false;

            this.inActiveScope = true;
            return uow;
        }

        /// <summary>
        /// Creates a new <see cref="IUnitOfWork"/> appropriate for the current session with the default <see cref="ScopeOptions"/>.
        /// </summary>
        /// <returns>The <see cref="IUnitOfWork"/> instance that can be used when modifying the current session.</returns>
        public IUnitOfWork CreateScope()
        {
            return this.CreateScope(ScopeOptions.Default);
        }

        /// <summary>
        /// Creates a new <see cref="IUnitOfWork"/> appropriate for the current session with the supplied <paramref name="options"/>.
        /// </summary>
        /// <returns>The <see cref="IUnitOfWork"/> instance that can be used when modifying the current session.</returns>
        /// <exception cref="InvalidOperationException">
        /// <para><paramref name="options"/> combines AutoCommit and NoTracking options</para>
        /// <para>-OR-</para>
        /// <para>There is already an active instance of an open <see cref="IUnitOfWork"/></para>
        /// </exception>
        public IUnitOfWork CreateScope(ScopeOptions options)
        {
            return this.OnCreatingScope(options);
        }

        /// <summary>
        /// Acquires an <see cref="System.Data.Entity.IDbSet{T}"/> instance for the indicated entity.
        /// </summary>
        /// <typeparam name="T">The type of entity to acquire the <see cref="System.Data.Entity.IDbSet{T}"/> for.</typeparam>
        /// <returns>An <see cref="System.Data.Entity.IDbSet{T}"/> used to access the provider.</returns>
        /// <exception cref="InvalidOperationException">The type <typeparamref name="T"/> is not supported by the current session.</exception>
        public System.Data.Entity.IDbSet<T> SetOf<T>() where T : class
        {
            return this.Set<T>();
        }

        /// <summary>
        /// Acquires a command instance for the indicated API.
        /// </summary>
        /// <typeparam name="T">The type of command API to acquire the reference for.</typeparam>
        /// <returns>A component for used to implement the command API.</returns>
        /// <exception cref="InvalidOperationException">The type <typeparamref name="T"/> is not supported by the current session.</exception>
        [Obsolete("We're trying to kill this API-please notify if found in the wild", true)]
        public virtual T CommandFor<T>() where T : class
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Acquires a query instance for the indicated API.
        /// </summary>
        /// <typeparam name="T">The type of query API to acquire the reference for.</typeparam>
        /// <returns>A component for used to implement the query API.</returns>
        /// <exception cref="InvalidOperationException">The type <typeparamref name="T"/> is not supported by the current session.</exception>
        [Obsolete("We're trying to kill this API-please notify if found in the wild", true)]
        public T QueryFor<T>() where T : class
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}