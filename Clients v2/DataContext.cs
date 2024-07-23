using System;
using System.Data.Common;
using System.Data.Entity;
using System.Diagnostics;
using AccurateAppend.Security.Mapping;
using AccurateAppend.Websites.Clients.Areas.Box;
using Integration.NationBuilder.Data.Mapping;

namespace AccurateAppend.Websites.Clients
{
    [DebuggerDisplay("{" + nameof(instance) + "}:{Database.Connection.ConnectionString}")]
    public class DataContext : JobProcessing.DataAccess.DefaultContext
    {
        #region Fields

        private readonly Guid instance = Guid.NewGuid();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AccurateAppend.Accounting.DataAccess.DefaultContext"/> class. 
        /// </summary>
        /// <param name="nameOrConnectionString">The connection string or connection string settings key containing the connection string.</param>
        public DataContext(String nameOrConnectionString) : base(nameOrConnectionString)
        {
            Database.SetInitializer<DataContext>(null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:AccurateAppend.Accounting.DataAccess.DefaultContext"/> class. 
        /// </summary>
        /// <remarks>
        /// The connection will be considered to be owned by this context and the connection lifetime will be managed by this type.
        /// </remarks>
        /// <param name="connection">The <see cref="T:System.Data.Common.DbConnection"/> that will be used by this context.</param>
        public DataContext(DbConnection connection) : base(connection)
        {
            Database.SetInitializer<DataContext>(null);
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // AccurateAppend.Security models
            modelBuilder.Configurations.Add(new SingleUseLogonConfiguration());
            modelBuilder.Configurations.Add(new UserAlertConfiguration());
            modelBuilder.Configurations.Add(new PasswordResetRequestConfiguration());

            // Integration.NationBuilder models
            modelBuilder.Configurations.Add(new RegistrationConfiguration());
            modelBuilder.Configurations.Add(new RegistrationMarketingConfiguration());
            modelBuilder.Configurations.Add(new PushRequestConfiguration());
            modelBuilder.Configurations.Add(new JobRequestConfiguration());
            modelBuilder.Configurations.Add(new ReportRequestConfiguration());
            modelBuilder.Configurations.Add(new ListReportConfiguration());
            modelBuilder.Configurations.Add(new ClientRefConfiguration());
            modelBuilder.Configurations.Add(new DealRefConfiguration());

            // Integration.Box models
            modelBuilder.Configurations.Add(new BoxRegistrationConfiguration());
        }

        #endregion
    }
}