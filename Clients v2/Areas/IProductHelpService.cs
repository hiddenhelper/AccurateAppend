using System;
using System.Diagnostics.Contracts;
using AccurateAppend.Core.Definitions;

namespace AccurateAppend.Websites.Clients.Areas
{
    /// <summary>
    /// Represents a service that can provide the appropriate sales help text about NationBuilder products.
    /// </summary>
    /// <remarks>
    /// Used to keep nasty logic away from list appending order code.
    /// </remarks>
    [ContractClass(typeof (IProductHelpServiceContracts))]
    public interface IProductHelpService
    {
        /// <summary>
        /// Accesses the standard sales help text for the indicated <paramref name="product"/>.
        /// </summary>
        /// <param name="product">The <see cref="PublicProduct"/> to acquire the help text for.</param>
        /// <returns>The sales team produced help text marketing the product.</returns>
        String GetHelpText(PublicProduct product);

        /// <summary>
        /// Accesses the product title.
        /// </summary>
        /// <param name="product">The <see cref="PublicProduct"/> to acquire the title text for.</param>
        /// <returns>The product title.</returns>
        String GetTitle(PublicProduct product);

        /// <summary>
        /// Accesses the product description.
        /// </summary>
        /// <param name="product">The <see cref="PublicProduct"/> to acquire the description text for.</param>
        /// <returns>The product description.</returns>
        String GetDescription(PublicProduct product);

        /// <summary>
        /// Accesses the category for the title.
        /// </summary>
        /// <param name="product">The <see cref="PublicProduct"/> to acquire the title text for.</param>
        /// <returns>The product title.</returns>
        SupportedProductCategory GetCategory(PublicProduct product);
    }

    [ContractClassFor(typeof(IProductHelpService))]
    // ReSharper disable InconsistentNaming
    internal abstract class IProductHelpServiceContracts : IProductHelpService
    // ReSharper restore InconsistentNaming
    {
        String IProductHelpService.GetHelpText(PublicProduct product)
        {
            Contract.Ensures(!String.IsNullOrWhiteSpace(Contract.Result<String>()));

            return default(String);
        }

        String IProductHelpService.GetTitle(PublicProduct product)
        {
            Contract.Ensures(!String.IsNullOrWhiteSpace(Contract.Result<String>()));

            return default(String);
        }

        String IProductHelpService.GetDescription(PublicProduct product)
        {
            Contract.Ensures(!String.IsNullOrWhiteSpace(Contract.Result<String>()));

            return default(String);
        }

        SupportedProductCategory IProductHelpService.GetCategory(PublicProduct product)
        {
            return default(SupportedProductCategory);
        }
    }
}