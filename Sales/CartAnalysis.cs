using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AccurateAppend.Core;
using AccurateAppend.Core.Definitions;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Contains extension methods for the <see cref="Cart"/> type for interacting with the CSV input file analysis xml.
    /// </summary>
    public static class CartAnalysis
    {
        /// <summary>
        /// Access the sequence of child 'Operation' elements found in the <paramref name="analysis"/> xml.
        /// </summary>
        /// <param name="analysis">The 'Analysis' root element.</param>
        /// <returns>The sequence of 'Operation' elements.</returns>
        private static IEnumerable<XElement> Operations(this XElement analysis)
        {
            return analysis?.Elements("Operation") ?? Enumerable.Empty<XElement>();
        }

        /// <summary>
        /// Accesses the 'name' attribute on the supplied <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The element to acquire the parsed 'name' value from.</param>
        /// <returns>The value parsed as a <see cref="PublicProduct"/> value.</returns>
        private static PublicProduct Product(this XElement element)
        {
            return EnumExtensions.Parse<PublicProduct>(element.Attribute("name")?.Value);
        }

        /// <summary>
        /// Sets the 'name' attribute to the indicated <paramref name="product"/> value.
        /// </summary>
        /// <param name="element">The element to set the 'name' value for.</param>
        /// <param name="product">The <see cref="PublicProduct"/> value to set.</param>
        private static void Product(this XElement element, PublicProduct product)
        {
            element.SetAttributeValue("name", product);
        }

        /// <summary>
        /// Accesses the value on the supplied <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The element to acquire the parsed value from.</param>
        /// <returns>The value parsed as an <see cref="Int32"/> value.</returns>
        private static Int32 RecordCount(this XElement element)
        {
            return Convert.ToInt32(element.Value);
        }

        /// <summary>
        /// Sets the value to the indicated <paramref name="count"/> value.
        /// </summary>
        /// <param name="element">The element to set the value for.</param>
        /// <param name="count">The count to set the value to.</param>
        private static void RecordCount(this XElement element, Int32 count)
        {
            element.Value = count.ToString();
        }
        
        /// <summary>
        /// Iterates the analysis xml content extracting values as a projected tuple.
        /// </summary>
        /// <param name="xml">The root 'Analysis' element.</param>
        /// <returns>The sequence of parsed and extracted values from the analysis xml.</returns>
        private static IEnumerable<Tuple<PublicProduct, Int32>> AnalyzedProducts(this XElement xml)
        {
            return from operation in xml.Operations()
                let operationName = Product(operation)
                let records = operation.RecordCount()
                select Tuple.Create(operationName, records);
        }

        /// <summary>
        /// Iterates the analysis xml content extracting values as a projected tuple.
        /// </summary>
        /// <param name="cart">The <see cref="Cart"/> to access the analysis for.</param>
        /// <returns>The sequence of parsed and extracted values from the analyzed <see cref="Cart"/>.</returns>
        public static IEnumerable<Tuple<PublicProduct, Int32>> AnalyzedProducts(this Cart cart)
        {
            if (cart == null) throw new ArgumentNullException(nameof(cart));

            var csvCart = cart as CsvCart;
            if (csvCart == null) yield break;

            foreach (var operation in csvCart.Analysis.AnalyzedProducts())
            {
                yield return operation;
            }
        }

        /// <summary>
        /// Sets the analysis xml content for the <paramref name="cart"/>.
        /// </summary>
        /// <param name="cart">The <see cref="CsvCart"/> to set the analysis for.</param>
        /// <param name="product">The <see cref="PublicProduct"/> to set the value for.</param>
        /// <param name="suitableRecords">The number of suitable records that appear to have valid minimum inputs for. Setting this value to null will remove the product.</param>
        public static void AnalyzedProduct(this CsvCart cart, PublicProduct product, Int32? suitableRecords = null)
        {
            if (cart == null) throw new ArgumentNullException(nameof(cart));

            var xml = cart.Analysis ?? new XElement("Analysis");
            var element = xml.Operations().FirstOrDefault(e => e.Product() == product);

            if (element == null && suitableRecords == null) return;

            if (element == null)
            {
                element = new XElement("Operation");
                element.Product(product);
                element.RecordCount(suitableRecords.Value);

                xml.Add(element);
            }
            else if (suitableRecords == null)
            {
                element.Remove();
            }
            else
            {
                element.RecordCount(suitableRecords.Value);
            }

            cart.Analysis = xml;
        }
    }
}