using System;
using AccurateAppend.Core.Definitions;

namespace AccurateAppend.Websites.Clients.Areas
{
    /// <summary>
    /// Holds common constants and values for the <see cref="PublicProduct"/> type that are enabled for use in the application.
    /// </summary>
    public static class SupportedProductHelper
    {
        /// <summary>
        /// Holds the set of <see cref="PublicProduct"/> values in preferred order that are allowed in the application.
        /// </summary>
        public static PublicProduct[] Products { get; } = {
            PublicProduct.EMAIL_VER_DELIVERABLE,
            PublicProduct.EMAIL_BASIC_NO_SUPRESSION_W_VERIFICATION,
            PublicProduct.EMAIL_BASIC_REV,
            PublicProduct.PHONE_DA,
            PublicProduct.PHONE_PREM,
            PublicProduct.PHONE_MOB,
            PublicProduct.DEMOGRAHICS,
            PublicProduct.PHONE_REV_PREM,
#if DEBUG
            //PublicProduct.UNIFIED_REV_ALL,
            PublicProduct.PHONE_BUS_DA,
#endif
            PublicProduct.PHONE,
            PublicProduct.CASS,
            PublicProduct.NCOA48
        };

        /// <summary>
        /// Determines the <see cref="SupportedProductCategory"/> for the supplied <paramref name="product"/>.
        /// Only values found in the <see cref="Products"/> collection are supported.
        /// </summary>
        /// <param name="product">The <see cref="PublicProduct"/> to get a category for.</param>
        /// <returns>The <see cref="SupportedProductCategory"/> for the supplied <paramref name="product"/>.</returns>
        /// <exception cref="NotSupportedException"><paramref name="product"/> is not found in the list of allowed <see cref="Products"/> for the application.</exception>
        public static SupportedProductCategory GetCategory(this PublicProduct product)
        {
            switch (product)
            {
                case PublicProduct.PHONE_DA:
                case PublicProduct.PHONE_PREM:
                case PublicProduct.PHONE_MOB:
                case PublicProduct.PHONE_BUS_DA:
                case PublicProduct.PHONE_REV_PREM:
                    return SupportedProductCategory.Phone;
                case PublicProduct.EMAIL_BASIC_NO_SUPRESSION_W_VERIFICATION:
                case PublicProduct.EMAIL_VER_DELIVERABLE:
                case PublicProduct.EMAIL_BASIC_REV:
                    return SupportedProductCategory.Email;
                case PublicProduct.SCORE_DONOR:
                case PublicProduct.SCORE_GREEN:
                case PublicProduct.SCORE_WEALTH:
                case PublicProduct.DEMOGRAHICS:
                case PublicProduct.NCOA48:
                case PublicProduct.CASS:
                case PublicProduct.UNIFIED_REV_ALL:
                case PublicProduct.PHONE:
                    return SupportedProductCategory.Other;
                default:
                    throw new NotSupportedException($"Product {product} does not have a supported category.");
            }
        }
    }
}