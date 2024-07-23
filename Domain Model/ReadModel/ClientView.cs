using System;
using System.Data.Entity.ModelConfiguration;
using AccurateAppend.Accounting;
using AccurateAppend.Core;
using DomainModel.Enum;
using LeadSource = AccurateAppend.Accounting.LeadSource;

namespace DomainModel.ReadModel
{
    /// <summary>
    /// Readmodel for Client Views.
    /// </summary>
    public class ClientView
    {
        #region Constructor

        /// <summary>
        /// This is a readonly type.
        /// </summary>
        protected ClientView()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the identifier of the current client.
        /// </summary>
        public virtual Guid UserId { get; protected set; }

        /// <summary>
        /// Gets the identifier of the client application.
        /// </summary>
        public virtual Guid ApplicationId { get; protected set; }

        /// <summary>
        /// Gets the username of the current client.
        /// </summary>
        public virtual String UserName { get; protected set; }

        /// <summary>
        /// Gets the current status for the client.
        /// </summary>
        public virtual UserStatus Status { get; protected set; }

        /// <summary>
        /// Gets the point in time the last client generated action (submit job, logon, query API, etc) occured.
        /// </summary>
        public virtual DateTime LastActivityDate { get; protected set; }

        /// <summary>
        /// Gets the total revenue for the last calendar year.
        /// </summary>
        public virtual Decimal LifeTimeRevenue { get; protected set; }

        /// <summary>
        /// Gets the first name of the client.
        /// </summary>
        public virtual String FirstName { get; protected set; }

        /// <summary>
        /// Gets the last name of the client.
        /// </summary>
        public virtual String LastName { get; protected set; }

        /// <summary>
        /// Gets the business name of the client.
        /// </summary>
        public virtual String BusinessName { get; protected set; }

        /// <summary>
        /// Indicates whether the current client has a current subscription.
        /// </summary>
        public virtual Boolean IsSubscriber { get; protected set; }

        public String Address { get; protected set; }

        public String City { get; protected set; }

        public String State { get; protected set; }

        public String Zip { get; protected set; }

        public String Phone { get; protected set; }

        /// <summary>
        /// Contains the originating source of the client.
        /// </summary>
        public LeadSource LeadSource { get; set; }

        #region Derived Properties

        /// <summary>
        /// Provides a human readable description of the total <see cref="LifeTimeRevenue"/>. Do not query.
        /// </summary>
        public virtual String LifeTimeRevenueDescription
        {
            get
            {
                if (this.LifeTimeRevenue < 1000) return "$";
                if (this.LifeTimeRevenue < 5000) return "$$";
                if (this.LifeTimeRevenue < 10000) return "$$$";
                if (this.LifeTimeRevenue < 20000) return "$$$$";

                return "$$$$$";
            }
        }

        /// <summary>
        /// Gets the formated location of the client (City, State).
        /// </summary>
        public virtual String Location
        {
            get
            {
                var city = (this.City ?? String.Empty).Trim();
                var state = (this.State ?? String.Empty).Trim();

                if (city.Length > 0 && state.Length > 0) return $"{city}, {state}";
                return String.Empty;
            }
        }

        /// <summary>
        /// Gets the name of the client. Do not query.
        /// </summary>
        public virtual String CompositeName => PartyExtensions.BuildCompositeName(this.FirstName, this.LastName, this.BusinessName);

        /// <summary>
        /// Provides a human readable description of the <see cref="LastActivityDate"/>. Do not query.
        /// </summary>
        public virtual String LastActivityDescription => this.LastActivityDate.DescribeDifference(DateTime.Now);

        #endregion

        #endregion
    }

    internal class ClientViewConfiguration : EntityTypeConfiguration<ClientView>
    {
        public ClientViewConfiguration()
        {
            this.ToTable("ClientsView", "admin");

            // Primary Key
            this.HasKey(c => c.UserId);

            // Ignore derived properties
            this.Ignore(c => c.CompositeName);
            this.Ignore(c => c.LastActivityDescription);
            this.Ignore(c => c.LifeTimeRevenueDescription);
            this.Ignore(c => c.Location);

            this.Property(c => c.ApplicationId);
            this.Property(c => c.UserName).IsUnicode().HasMaxLength(256);
            this.Property(c => c.FirstName).IsUnicode(false).HasMaxLength(100);
            this.Property(c => c.LastActivityDate);
            this.Property(c => c.LastName).IsUnicode(false).HasMaxLength(100);
            this.Property(c => c.LifeTimeRevenue).HasPrecision(19, 4);
            this.Property(c => c.Address).IsUnicode(false).HasMaxLength(100);
            this.Property(c => c.City).IsUnicode(true).HasMaxLength(50);
            this.Property(c => c.State).IsUnicode(true).HasMaxLength(50);
            this.Property(c => c.Zip).IsUnicode(true).HasMaxLength(50);
            this.Property(c => c.Phone).IsUnicode(false).HasMaxLength(50);

            this.Property(c => c.Status);
            this.Property(c => c.UserId);
            this.Property(c => c.LeadSource);
        }
    }
}
