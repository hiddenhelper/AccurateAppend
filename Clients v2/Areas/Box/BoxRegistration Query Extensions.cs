using System.Linq;
using System.Threading;
using AccurateAppend.Core.IdentityModel;

namespace AccurateAppend.Websites.Clients.Areas.Box
{
    /// <summary>
    /// Extension methods for the <see cref="BoxRegistration"/> type.
    /// </summary>
    public static class BoxRegistrationExtensions
    {
        /// <summary>
        /// Crafts a query predicate that can acquire the <see cref="BoxRegistration"/> instances owned by the current interactive user.
        /// </summary>
        /// <note type="Warning">
        /// The predicate filter is added to the provided <paramref name="source"/> instance. 
        /// It does not guarantee that a <see cref="BoxRegistration"/> instance will be returned if the originating
        /// predicate produces no results. 
        /// </note>
        /// <param name="source">The source query to acquire the data from.</param>
        /// <returns>An <see cref="IQueryable{BoxRegistration}"/> filtered to the current user that can be further customized.</returns>
        public static IQueryable<BoxRegistration> ForInteractiveUser(this IQueryable<BoxRegistration> source)
        {
            var userId = Thread.CurrentPrincipal.Identity.GetIdentifier();

            return source.Where(r => r.UserId == userId);
        }
    }
}