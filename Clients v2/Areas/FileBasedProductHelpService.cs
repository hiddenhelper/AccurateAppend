using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Core.Utilities;

namespace AccurateAppend.Websites.Clients.Areas
{
    /// <summary>
    /// Default implementation of the <see cref="IProductHelpService"/> interface allowing
    /// sales team supplied content to be access off the file system.
    /// </summary>
    /// <remarks>
    /// Requires an <see cref="IFileLocation"/> location to be supplied. Within that location
    /// two files will be required to be present for each <see cref="Products"/> value. The
    /// first is {Product}.html and should contain HTML content providing markup for
    /// consumption by a browser. The second should be {Product}.description.html which
    /// should contain literal human readable content.
    /// 
    /// e.g. EMAIL_VER_DELIVERABLE product would have the following files
    /// -EMAIL_VER_DELIVERABLE.html
    /// -EMAIL_VER_DELIVERABLE.description.html
    /// </remarks>
    public class FileBasedProductHelpService : IProductHelpService
    {
        #region Fields

        private readonly Lazy<IDictionary<PublicProduct, String>> helpContent;
        private readonly Lazy<IDictionary<PublicProduct, String>> descriptions;
        private readonly IFileLocation location;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FileBasedProductHelpService"/> class.
        /// </summary>
        /// <param name="location">The <see cref="IFileLocation"/> that provides access to the location to locate product files from.</param>
        public FileBasedProductHelpService(IFileLocation location)
        {
            if (location == null) throw new ArgumentNullException(nameof(location));
            Contract.EndContractBlock();

            this.location = location;
            this.helpContent = new Lazy<IDictionary<PublicProduct, String>>(this.HelpContentFactory, LazyThreadSafetyMode.ExecutionAndPublication);
            this.descriptions = new Lazy<IDictionary<PublicProduct, String>>(this.DescriptionFactory, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the set of supported products with sales help text content.
        /// </summary>
        protected virtual PublicProduct[] Products => SupportedProductHelper.Products;

        #endregion

        #region Methods

        protected virtual IDictionary<PublicProduct, String> HelpContentFactory()
        {
            Contract.Ensures(Contract.Result<IDictionary<PublicProduct, String>>() != null);

            var data = this.Products.ToDictionary(p => p, p => String.Empty);

            Parallel.ForEach(this.Products, p =>
            {
                data[p] = this.LoadContent($"{p}.html");
            });

            return data;
        }

        protected virtual IDictionary<PublicProduct, String> DescriptionFactory()
        {
            Contract.Ensures(Contract.Result<IDictionary<PublicProduct, String>>() != null);

            var data = this.Products.ToDictionary(p => p, p => String.Empty);

            Parallel.ForEach(this.Products, p =>
            {
                data[p] = this.LoadContent($"{p}.description.html");
            });

            return data;
        }

        private String LoadContent(String product)
        {
            var file = this.location.CreateInstance(product);
            if (!file.Exists()) throw new ConfigurationErrorsException($"Product description file for {product} is missing at {this.location}");

            using (var sr = new StreamReader(file.OpenStream(FileAccess.Read)))
            {
                var result = sr.ReadToEnd();
                return result;
            }
        }

        #endregion

        #region IProductHelpService Members

        /// <inheritdoc />
        public virtual String GetHelpText(PublicProduct product)
        {
            return this.helpContent.Value[product];
        }

        /// <inheritdoc />
        public virtual String GetTitle(PublicProduct product)
        {
            switch (product)
            {
                case PublicProduct.PHONE_REV_PREM:
                    return "Append U.S. postal addresses to phone where missing";
                case PublicProduct.EMAIL_VER_DELIVERABLE:
                    return "Verify existing email addresses";
                case PublicProduct.EMAIL_BASIC_NO_SUPRESSION_W_VERIFICATION:
                    return "Append verified email addresses";
                case PublicProduct.EMAIL_BASIC_REV:
                    return "Append U.S. postal addresses to email addresses where missing";
                case PublicProduct.PHONE_PREM:
                    return "Append quarterly updated land line phone numbers";
                case PublicProduct.PHONE_DA:
                    return "Append daily updated land line phone numbers";
                case PublicProduct.PHONE_BUS_DA:
                    return "Append daily updated business phone numbers";
                case PublicProduct.PHONE_MOB:
                    return "Append mobile phone numbers";
                case PublicProduct.DEMOGRAHICS:
                    return "Append demographic and lifestyle attributes";
                case PublicProduct.CASS:
                    return "Validate and standardize U.S. postal addresses";
                case PublicProduct.PHONE:
                    return "Validate existing phone & identify phone line type";
                case PublicProduct.NCOA48:
                    return "Update addresses using USPS 48 Month National Change of Address";
                case PublicProduct.SCORE_DONOR:
                    return "Score Donor";
                case PublicProduct.SCORE_GREEN:
                    return "Score Green";
                case PublicProduct.SCORE_WEALTH:
                    return "Score Wealth";
            }

            return String.Empty;
        }

        /// <inheritdoc />
        public virtual String GetDescription(PublicProduct product)
        {
            return this.descriptions.Value[product];
        }

        /// <inheritdoc />
        public virtual SupportedProductCategory GetCategory(PublicProduct product)
        {
            return product.GetCategory();
        }

        #endregion
    }
}