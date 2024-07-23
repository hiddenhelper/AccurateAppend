using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.JobProcessing.DataAccess;
using AccurateAppend.Security.Mapping;
using Integration.NationBuilder.Data;
using Integration.NationBuilder.Data.Mapping;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Admin specific context that ties the entire system together.
    /// </summary>
    public class UnifiedDataContext : DefaultContext
    {
        #region Constructors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedDataContext"/> class. 
        /// </summary>
        /// <param name="nameOrConnectionString">The connection string or connection string settings key containing the connection string.</param>
        public UnifiedDataContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            Database.SetInitializer<UnifiedDataContext>(null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnifiedDataContext"/> class. 
        /// </summary>
        /// <remarks>
        /// The connection will be considered to be owned by this context and the connection lifetime will be managed by this type.
        /// </remarks>
        /// <param name="connection">The <see cref="T:System.Data.Common.DbConnection"/> that will be used by this context.</param>
        public UnifiedDataContext(DbConnection connection) : base(connection)
        {
            Database.SetInitializer<UnifiedDataContext>(null);
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AccessConfiguration());
            modelBuilder.Configurations.Add(new RightConfiguration());
            modelBuilder.Configurations.Add(new SingleUseLogonConfiguration());
            modelBuilder.Configurations.Add(new UserAlertConfiguration());

            modelBuilder.Configurations.Add(new PushLookupConfiguration());
            modelBuilder.Configurations.Add(new JobRequestConfiguration());
            modelBuilder.Configurations.Add(new PushRequestConfiguration());
            modelBuilder.Configurations.Add(new ReportRequestConfiguration());
            modelBuilder.Configurations.Add(new ListReportConfiguration());
            modelBuilder.Configurations.Add(new RegistrationConfiguration());
            modelBuilder.Configurations.Add(new ClientRefConfiguration());
            modelBuilder.Configurations.Add(new DealRefConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        /// <inheritdoc />
        /// <remarks>
        /// Extends the behavior to scan for loaded <see cref="PushLookup"/> instances
        /// without a connected <see cref="PushRequest"/>. Any instanced matching this
        /// condition will be removed from the context prior to actually commiting the
        /// changes.
        /// </remarks>
        protected override void OnSaving()
        {
            base.OnSaving();

            var lookups = this.SetOf<PushLookup>();
            var orphanLookups = lookups.Local.Where(o => o.For == null).ToArray();
            orphanLookups.ForEach(o => lookups.Remove(o));
        }

        #endregion
    }
}