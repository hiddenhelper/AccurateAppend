using System;
using System.Diagnostics.Contracts;
using System.Linq;
using AccurateAppend.Data;
using DomainModel.ReadModel;

namespace DomainModel.Queries
{
    /// <summary>
    /// Standard default implementation of <see cref="IClientsViewQuery"/>.
    /// </summary>
    public class ClientsViewQuery : IClientsViewQuery
    {
        #region Fields

        private readonly ISessionContext context;

        #endregion

        #region Constructor

        public ClientsViewQuery(ISessionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region IClientsViewQuery Members

        /// <summary>
        /// Crafts a queryable for <see cref="ClientView"/> entities that have been active during the indicated time frame.
        /// </summary>
        public virtual IQueryable<ClientView> ActiveDuring(DateTime startOn, DateTime endBy)
        {
            var clients = this.context.SetOf<ClientView>()
                    .Where(c => c.LastActivityDate >= startOn && c.LastActivityDate < endBy);

            return clients.AsNoTracking();
        }

        /// <inheritdoc />
        /// <remarks>
        /// Performs a fuzzy search of the following:
        /// -Clients
        /// --UserName/Email
        /// --First, Last, Business names
        /// --Address
        /// --Phone
        ///
        /// -Contacts
        /// --Name
        /// --Email
        ///
        /// -Nations
        /// --Slug
        /// </remarks>
        public virtual IQueryable<ClientView> Search(String term)
        {
            var clientQuery = this.context
                .SetOf<ClientView>()
                .Where(c => c.FirstName.Contains(term)
                            || c.LastName.Contains(term)
                            || c.BusinessName.Contains(term)
                            || c.Address.Contains(term)
                            || c.Phone.Contains(term)
                            || c.UserName.Contains(term))
                .Select(c => c.UserId);

            var contactQuery = this.context
                .SetOf<ContactView>()
                .Where(c => c.Name.Contains(term) || c.Email.Contains(term))
                .Select(c => c.UserId);

            var registrationQuery = this.context
                .SetOf<NationBuilderRegistration>()
                .Where(r => r.Slug.Contains(term))
                .Select(r => r.UserId);

            var allUsersQuery = clientQuery.Concat(registrationQuery).Concat(contactQuery);

            var users = this.context.SetOf<ClientView>().Join(allUsersQuery, u => u.UserId, id => id, (u, id) => u).Distinct();

            return users.AsNoTracking();
        }

        #endregion
    }
}
