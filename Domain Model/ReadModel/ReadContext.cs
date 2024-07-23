using System;
using System.Data.Common;
using System.Linq;
using AccurateAppend.Core.ComponentModel;
using AccurateAppend.Data;
using DbModelBuilder = System.Data.Entity.DbModelBuilder;

namespace DomainModel.ReadModel
{
    /// <summary>
    /// A custom <see cref="ReadOnlyContext"/> for managing the system read-model.
    /// </summary>
    public class ReadContext : ReadOnlyContext, ISessionContext
    {
        #region Constructors

        /// <summary>
        /// Constructs a new context instance using conventions to create the name of the database to
        ///                 which a connection will be made.  The by-convention name is the full name (namespace + class name)
        ///                 of the derived context class.
        ///                 See the class remarks for how this is used to create a connection.
        /// 
        /// </summary>
        protected ReadContext()
        {
            System.Data.Entity.Database.SetInitializer<ReadContext>(null);

            this.Configuration.AutoDetectChangesEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = false;
        }

        /// <summary>
        /// Constructs a new context instance using the given string as the name or connection string for the
        ///                 database to which a connection will be made.
        ///                 See the class remarks for how this is used to create a connection.
        /// 
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        public ReadContext(String nameOrConnectionString) : base(nameOrConnectionString)
        {
            System.Data.Entity.Database.SetInitializer<ReadContext>(null);

            this.Configuration.AutoDetectChangesEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = false;
        }

        /// <summary>
        /// Constructs a new context instance using the existing connection to connect to a database.
        ///                 The connection will not be disposed when the context is disposed.
        /// 
        /// </summary>
        /// <param name="existingConnection">An existing connection to use for the new context.</param><param name="contextOwnsConnection">If set to <c>true</c> the connection is disposed when
        ///                 the context is disposed, otherwise the caller must dispose the connection.</param>
        public ReadContext(DbConnection existingConnection, Boolean contextOwnsConnection) : base(existingConnection, contextOwnsConnection)
        {
            System.Data.Entity.Database.SetInitializer<ReadContext>(null);

            this.Configuration.AutoDetectChangesEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = false;
        }

        #endregion

        #region ISessionContext Members

        /// <summary>
        /// Creates a new <see cref="T:AccurateAppend.Core.ComponentModel.IUnitOfWork"/> appropriate for the current session.
        /// </summary>
        /// <returns>
        /// The <see cref="T:AccurateAppend.Core.ComponentModel.IUnitOfWork"/> instance that can be used when modifying the current session.
        /// </returns>
        public IUnitOfWork CreateScope()
        {
            return this.CreateScope(ScopeOptions.NoTracking);
        }

        /// <summary>
        /// Creates a new <see cref="T:AccurateAppend.Core.ComponentModel.IUnitOfWork"/> with the supplied behavior options.
        /// </summary>
        /// <param name="options">The set of <see cref="T:AccurateAppend.Data.ScopeOptions"/> to explain the desired behavior of the scope.</param>
        /// <returns>
        /// The <see cref="T:AccurateAppend.Core.ComponentModel.IUnitOfWork"/> instance that can be used when modifying the current session.
        /// </returns>
        public virtual IUnitOfWork CreateScope(ScopeOptions options)
        {
            if (options.HasFlag(ScopeOptions.AutoCommit)) throw new NotSupportedException("The context only supports read access");

            var uow = new NullUnitOfWork(this, false);
            return uow;
        }

        /// <summary>
        /// Acquires an <see cref="T:System.Data.Entity.IDbSet`1"/> instance for the indicated entity.
        /// </summary>
        /// <typeparam name="T">The type of entity to acquire the <see cref="T:System.Data.Entity.IDbSet`1"/> for.</typeparam>
        /// <returns>
        /// An <see cref="T:System.Data.Entity.IDbSet`1"/> used to access the provider.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The type <typeparamref name="T"/> is not supported by the current session.</exception>
        public System.Data.Entity.IDbSet<T> SetOf<T>() where T : class
        {
            return base.Set<T>();
        }

        /// <summary>
        /// Acquires a command instance for the indicated API.
        /// </summary>
        /// <typeparam name="T">The type of command API to acquire the reference for.</typeparam>
        /// <returns>
        /// A component for used to implement the command API.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The type <typeparamref name="T"/> is not supported by the current session.</exception>
        public virtual T CommandFor<T>() where T : class
        {
            return null;
        }

        /// <summary>
        /// Acquires a query instance for the indicated API.
        /// </summary>
        /// <typeparam name="T">The type of query API to acquire the reference for.</typeparam>
        /// <returns>
        /// A component for used to implement the query API.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The type <typeparamref name="T"/> is not supported by the current session.</exception>
        public virtual T QueryFor<T>() where T : class
        {
            return null;
        }

        #endregion
        
        #region Overrides

        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but
        ///             before the model has been locked down and used to initialize the context.  The default
        ///             implementation of this method does nothing, but it can be overridden in a derived class
        ///             such that the model can be further configured before it is locked down.
        /// </summary>
        /// <remarks>
        /// Typically, this method is called only once when the first instance of a derived context
        ///             is created.  The model for that context is then cached and is for all further instances of
        ///             the context in the app domain.  This caching can be disabled by setting the ModelCaching
        ///             property on the given ModelBuidler, but note that this can seriously degrade performance.
        ///             More control over caching is provided through use of the DbModelBuilder and DbContextFactory
        ///             classes directly.
        /// </remarks>
        /// <param name="modelBuilder">The builder that defines the model for the context being created. </param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new ChargeEventConfiguration());
            modelBuilder.Configurations.Add(new ClientViewConfiguration());
            modelBuilder.Configurations.Add(new LeadViewConfiguration());
            modelBuilder.Configurations.Add(new NationBuilderRegistrationConfiguration());
            modelBuilder.Configurations.Add(new NationBuilderPushViewConfiguration());
            modelBuilder.Configurations.Add(new JobQueueViewConfiguration());
            modelBuilder.Configurations.Add(new DeployedSystemConfiguration());
            modelBuilder.Configurations.Add(new FailedBusMessageConfiguration());
            modelBuilder.Configurations.Add(new ContactViewConfiguration());
        }
        
        #endregion

        #region Queries

        public virtual IQueryable<ChargeEvent> Charges => this.SetOf<ChargeEvent>().AsNoTracking();

        public virtual IQueryable<ClientView> Clients => this.SetOf<ClientView>().AsNoTracking();

        public virtual IQueryable<LeadView> Leads => this.SetOf<LeadView>().AsNoTracking();

        public virtual IQueryable<NationBuilderRegistration> Registrations => this.SetOf<NationBuilderRegistration>().AsNoTracking();

        public virtual IQueryable<NationBuilderPushView> Pushes => this.SetOf<NationBuilderPushView>().AsNoTracking();

        public virtual IQueryable<JobQueueView> Jobs => this.SetOf<JobQueueView>().AsNoTracking();

        #endregion
    }
}