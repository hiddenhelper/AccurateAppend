using System;
using System.Collections.Generic;
using System.Linq;
using AccurateAppend.Core;
using AccurateAppend.Core.Definitions;
using NServiceBus;

namespace AccurateAppend.Websites.Clients.Areas.NationBuilder.Order.Messages
{
    /// <summary>
    /// Contains the event data for when a NationBuilder order was placed via the public system.
    /// </summary>
    [Serializable()]
    public class NationBuilderOrderPlacedEvent : IEvent
    {
        #region Fields

        private DateTime dateSubmitted;
        private List<Orderproduct> products = new List<Orderproduct>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the date and time (in UTC) when the order was submitted
        /// </summary>
        /// <value>The date and time (in UTC) when the order was submitted.</value>
        public DateTime DateSubmitted
        {
            get => this.dateSubmitted;
            set => this.dateSubmitted = value.Coerce();
        }

        /// <summary>
        /// Gets the identifier of the cart that was ordered.
        /// </summary>
        public Guid CartId { get; set; }

        /// <summary>
        /// Gets the identifier of the client that ordered.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets the list of products that was ordered.
        /// </summary>
        public List<Orderproduct> Products
        {
            get
            {
                if (this.products == null) this.products = new List<Orderproduct>();
                return this.products;
            }
            set => this.products = value ?? new List<Orderproduct>();
        }

        /// <summary>
        /// Gets the minimum order amount, if any.
        /// </summary>
        public Decimal? OrderMinimum { get; set; }

        /// <summary>
        /// Gets the name of the NationBuilder list.
        /// </summary>
        public String ListName { get; set; }

        /// <summary>
        /// Gets the remote identifier of the NationBuilder list.
        /// </summary>
        public Int32 ListId { get; set; }

        /// <summary>
        /// Gets the total number of contacts in the NationBuilder list.
        /// </summary>
        public Int32 TotalRecords { get; set; }

        /// <summary>
        /// Gets the unique internal Nation identifier.
        /// </summary>
        public Int32 IntegrationId { get; set; }

        /// <summary>
        /// Gets the SLUG of the Nation.
        /// </summary>
        public String Slug { get; set; }
        
        #endregion

        #region Methods

        /// <summary>
        /// Calculates the estimated order total.
        /// </summary>
        /// <returns>The total, in USD, estimated for this entire order.</returns>
        public Decimal Total()
        {
            var total = this.Products.Any() ? this.Products.Sum(p => p.Total()) : 0;
            if (this.OrderMinimum == null) return total;

            total = Math.Max(total, this.OrderMinimum.Value);
            return total;
        }

        #endregion
    }

    /// <summary>
    /// Contains the data for a single product, price per unit, and estimated matches on an order.
    /// </summary>
    [Serializable()]
    public class Orderproduct
    {
        /// <summary>
        /// Gets the friendly or display name of the <see cref="Product"/>.
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// Gets the <see cref="PublicProduct"/> that was ordered.
        /// </summary>
        public PublicProduct Product { get; set; }

        /// <summary>
        /// Get the price per unit, in USD, that was quoted in the order.
        /// </summary>
        // ReSharper disable InconsistentNaming
        public Decimal PPU { get; set; }
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// Gets the estimated number of matches on an order.
        /// </summary>
        public Int32 EstimatedMatches { get; set; }

        #region Methods

        /// <summary>
        /// Calculates the estimated product total.
        /// </summary>
        /// <returns>The total, in USD, estimated for this single product.</returns>
        public Decimal Total()
        {
            var total = this.EstimatedMatches * this.PPU;
            return Math.Max(total, 0);
        }

        #endregion
    }
}