#pragma warning disable SA1649 // File name must match first type name

using System;
using System.Collections.Generic;
using System.Threading;
using AccurateAppend.Core.IdentityModel;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Extension methods for the <see cref="Audit"/> class.
    /// </summary>
    public static class AuditExtensions
    {
        /// <summary>
        /// Convenience method to create and append a new <see cref="Audit"/>.
        /// </summary>
        /// <remarks>
        /// <note type="warning">Only safe to use when the application creates and establishes user security sessions.</note>
        /// </remarks>
        /// <param name="target">The collection to append the <see cref="Audit"/> to.</param>
        /// <param name="content">The content of the audit entry to create.</param>
        /// <returns>The new <see cref="Audit"/> instance.</returns>
        public static Audit Add(this ICollection<Audit> target, String content)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            var createdBy = Thread.CurrentPrincipal.Identity.GetIdentifier();

            var audit = new Audit(content, createdBy);
            target.Add(audit);

            return audit;
        }
    }
}
