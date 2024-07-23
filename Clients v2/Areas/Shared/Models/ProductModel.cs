using System;
using System.Diagnostics;
using AccurateAppend.Core.Definitions;

namespace AccurateAppend.Websites.Clients.Areas.Shared.Models
{
    [DebuggerDisplay("{" + nameof(ProductKey) + "}")]
    public class ProductModel : IEquatable<ProductModel>
    {
        public ProductModel(PublicProduct product)
        {
            this.ProductKey = product;
        }

        protected ProductModel()
        {
        }

        public PublicProduct ProductKey { get; set; }
        public bool PerformOverwrites { get; set; }
        public String Title { get; set; }
        public Decimal Cost { get; set; }
        public Int32 Count { get; set; }
        public Decimal EstMatches { get; set; }
        public Decimal Total => this.EstMatches * this.Cost;

        #region Overrides

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override Boolean Equals(Object obj)
        {
            return this.Equals((ProductModel) obj);
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override Int32 GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return this.ProductKey.GetHashCode(); // this IS actually constant value in use
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }

        #endregion

        #region IEquatable<ProductModel> Members

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
        /// <param name="other">An object to compare with this object.</param>
        public virtual Boolean  Equals(ProductModel other)
        {
            if (other == null) return false;

            return this.ProductKey == other.ProductKey;
        }

        #endregion
    }
}