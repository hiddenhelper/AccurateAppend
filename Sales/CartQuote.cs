using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AccurateAppend.Core;
using AccurateAppend.Core.Definitions;

namespace AccurateAppend.Sales
{
    /// <summary>
    /// Contains extension methods for the <see cref="Cart"/> type for interacting with the Order quote.
    /// </summary>
    public static class CartQuote
    {
        /// <summary>
        /// Access the sequence of child 'Operation' elements found in the <paramref name="quote"/> xml.
        /// </summary>
        /// <param name="quote">The 'Quote' root element.</param>
        /// <returns>The sequence of 'Operation' elements.</returns>
        private static IEnumerable<XElement> Operations(this XElement quote)
        {
            return quote.Elements("Operation");
        }

        /// <summary>
        /// Accesses the 'name' attribute on the supplied <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The element to acquire the parsed 'name' value from.</param>
        /// <returns>The value parsed as a <see cref="DataServiceOperation"/> value.</returns>
        private static DataServiceOperation Product(this XElement element)
        {
            return EnumExtensions.Parse<DataServiceOperation>(element.Attribute("name")?.Value);
        }

        /// <summary>
        /// Sets the 'name' attribute to the indicated <paramref name="product"/> value.
        /// </summary>
        /// <param name="element">The element to set the 'name' value for.</param>
        /// <param name="product">The <see cref="DataServiceOperation"/> value to set.</param>
        private static void Product(this XElement element, DataServiceOperation product)
        {
            element.SetAttributeValue("name", product);
        }

        /// <summary>
        /// Sets the 'rate' attribute to the indicated <paramref name="rate"/> value.
        /// </summary>
        /// <param name="element">The element to set the 'rate' value for.</param>
        /// <param name="rate">The PPU <see cref="Decimal"/> value to set.</param>
        private static void Rate(this XElement element, Decimal rate)
        {
            element.SetAttributeValue("rate", rate);
        }

        /// <summary>
        /// Accesses the 'rate' attribute on the supplied <paramref name="element"/> containing the PPU.
        /// </summary>
        /// <param name="element">The element to acquire the parsed 'rate' value from.</param>
        /// <returns>The value parsed as a <see cref="Decimal"/> value.</returns>
        private static Decimal Rate(this XElement element)
        {
            return Convert.ToDecimal(element.Attribute("rate")?.Value);
        }

        /// <summary>
        /// Sets the 'matches' attribute to the indicated <paramref name="matches"/> value.
        /// </summary>
        /// <param name="element">The element to set the 'matches' value for.</param>
        /// <param name="matches">The estimated number of matches value to set.</param>
        private static void EstimatedMatches(this XElement element, Int32 matches)
        {
            element.SetAttributeValue("matches", matches);
        }

        /// <summary>
        /// Accesses the 'matches' attribute on the supplied <paramref name="element"/> containing estimated number of matches for the product.
        /// </summary>
        /// <param name="element">The element to acquire the parsed 'matches' value from.</param>
        /// <returns>The value parsed as an <see cref="Int32"/> value.</returns>
        private static Int32 EstimatedMatches(this XElement element)
        {
            return Convert.ToInt32(element.Attribute("matches")?.Value);
        }

        /// <summary>
        /// Sets the sales quote xml content for the <paramref name="cart"/>.
        /// </summary>
        /// <param name="cart">The <see cref="Cart"/> to set the quote for.</param>
        /// <param name="product">The <see cref="PublicProduct"/> to set the value for.</param>
        /// <param name="productEstimatedMatches">The number of estimated matches that we expect to append.</param>
        /// <param name="productQuotedRate">The rate, based on <paramref name="productEstimatedMatches"/>, that we expect the PPU for appended matches.</param>
        public static void EnterQuotedRate(this Cart cart, DataServiceOperation product, Int32 productEstimatedMatches, Decimal productQuotedRate)
        {
            if (cart == null) throw new ArgumentNullException(nameof(cart));
            if (productEstimatedMatches < 0) throw new ArgumentOutOfRangeException(nameof(productEstimatedMatches), productEstimatedMatches, $"{nameof(productEstimatedMatches)} must be at least 0");
            if (productQuotedRate < Decimal.Zero) throw new ArgumentOutOfRangeException(nameof(productQuotedRate), productQuotedRate, $"{nameof(productQuotedRate)} must be at least 0");

            var xml = cart.Quote ?? new XElement("Quote");
            var element = xml.Operations().FirstOrDefault(e => e.Product() == product);

            if (element == null)
            {
                element = new XElement("Operation");
                element.Product(product);
                element.EstimatedMatches(productEstimatedMatches);
                element.Rate(productQuotedRate);

                xml.Add(element);
            }
            else
            {
                element.SetValue(productEstimatedMatches);
                element.Rate(productQuotedRate);
            }

            cart.Quote = xml;
        }

        /// <summary>
        /// Iterates the quote xml content extracting values as a projected tuple.
        /// </summary>
        /// <param name="cart">The <see cref="Cart"/> to access the quote for.</param>
        /// <returns>The sequence of parsed and extracted values from the analyzed <see cref="Cart"/>.</returns>
        public static IEnumerable<Tuple<DataServiceOperation, Int32, Decimal>> QuotedProducts(this Cart cart)
        {
            if (cart == null) throw new ArgumentNullException(nameof(cart));

            return cart.Quote == null
                ? Enumerable.Empty<Tuple<DataServiceOperation, Int32, Decimal>>()
                : QuotedProducts(cart.Quote);
        }

        /// <summary>
        /// Iterates the quote xml content extracting values as a projected tuple.
        /// </summary>
        /// <param name="quote">The <see cref="Cart"/> to access the quote for.</param>
        /// <returns>The sequence of parsed and extracted values from the analyzed <see cref="Cart"/>.</returns>
        public static IEnumerable<Tuple<DataServiceOperation, Int32, Decimal>> QuotedProducts(XElement quote)
        {
            if (quote == null) throw new ArgumentNullException(nameof(quote));

            foreach (var operation in quote.Operations())
            {
                var operationName = Product(operation);
                var records = operation.EstimatedMatches();
                var rate = operation.Rate();

                yield return Tuple.Create(operationName, records, rate);
            }
        }

        /// <summary>
        /// Sets the 'total' attribute to the indicated <paramref name="total"/> value.
        /// </summary>
        /// <param name="cart">The <see cref="Cart"/> to set the quote for.</param>
        /// <param name="total">The estimated total the order will be quoted at.</param>
        public static void QuotedTotal(this Cart cart, Decimal total)
        {
            if (cart == null) throw new ArgumentNullException(nameof(cart));

            var xml = cart.Quote ?? new XElement("Quote");
            xml.SetAttributeValue("total", total);

            cart.Quote = xml;
        }

        /// <summary>
        /// Accesses the 'total' attribute for the <paramref name="cart"/>.
        /// </summary>
        /// <param name="cart">The <see cref="Cart"/> to access the quoted estimated total for.</param>
        /// <returns>The estimated total for the order that was quoted.</returns>
        public static Decimal QuotedTotal(this Cart cart)
        {
            if (cart == null) throw new ArgumentNullException(nameof(cart));

            return cart.Quote == null
                ? Decimal.Zero
                : QuotedTotal(cart.Quote);
        }

        /// <summary>
        /// Accesses the 'total' attribute for the <paramref name="quote"/>.
        /// </summary>
        /// <param name="quote">The <see cref="Cart"/> to access the quoted estimated total for.</param>
        /// <returns>The estimated total for the order that was quoted.</returns>
        public static Decimal QuotedTotal(XElement quote)
        {
            if (quote == null) throw new ArgumentNullException(nameof(quote));

            var value = quote.Attribute("total")?.Value;
            return value == null ? Decimal.Zero : Decimal.Parse(value);
        }

        /// <summary>
        /// Sets the 'minimum' attribute to the indicated <paramref name="orderMinimum"/> value.
        /// </summary>
        /// <param name="cart">The <see cref="Cart"/> to set the quote for.</param>
        /// <param name="orderMinimum">The minimum total the order will be quoted at. Setting this to null will mean the order has no minimum value.</param>
        public static void OrderMinimum(this Cart cart, Decimal? orderMinimum)
        {
            if (cart == null) throw new ArgumentNullException(nameof(cart));
            if (orderMinimum.HasValue && orderMinimum < Decimal.Zero) throw new ArgumentOutOfRangeException(nameof(orderMinimum), orderMinimum, $"{nameof(orderMinimum)} must be at least 0");
            
            var xml = cart.Quote ?? new XElement("Quote");
            xml.SetAttributeValue("minimum", orderMinimum);

            cart.Quote = xml;
        }

        /// <summary>
        /// Accesses the 'minimum' attribute for the <paramref name="cart"/>.
        /// </summary>
        /// <param name="cart">The <see cref="Cart"/> to access the quoted order minimum for.</param>
        /// <returns>The estimated order minimum for the order that was quoted.</returns>
        public static Decimal OrderMinimum(this Cart cart)
        {
            if (cart == null) throw new ArgumentNullException(nameof(cart));

            if (cart.Quote == null) return Decimal.Zero;

            var value = cart.Quote.Attribute("minimum")?.Value;
            return value == null ? Decimal.Zero : Decimal.Parse(value);
        }
    }
}