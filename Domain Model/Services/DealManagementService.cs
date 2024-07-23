using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Data;
using AccurateAppend.JobProcessing.Manifest;
using AccurateAppend.JobProcessing.Reporting;
using AccurateAppend.Sales;
using AccurateAppend.Sales.Contracts.Messages;
using AccurateAppend.Sales.Contracts.Services;
using AccurateAppend.Sales.Contracts.ViewModels;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Security;
using NServiceBus;

namespace DomainModel.Services
{
    /// <summary>
    /// High level API for sales order content management.
    /// </summary>
    /// <remarks>
    /// Provides an abstraction that allows the simplified access, use, and logic required to create and edit
    /// <see cref="DealBinder"/> and <see cref="BillableOrder"/> logic. Order processing workflow is encapsulated
    /// in another component allowing this to focus on just content management.
    /// </remarks>
    public class DealManagementService : IDealManagementService, IOrderManagementService
    {
        #region Fields

        private readonly DefaultContext context;
        private readonly IMessageSession bus;
        private readonly IOrderCalculationService calculator;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DealManagementService"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> used to provide data access.</param>
        /// <param name="calculator">The <see cref="IOrderCalculationService"/> that provides order population logic.</param>
        /// <param name="bus">The <see cref="IMessageSession"/> used to send messages.</param>
        public DealManagementService(DefaultContext context, IOrderCalculationService calculator, IMessageSession bus)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (bus == null) throw new ArgumentNullException(nameof(bus));
            if (calculator == null) throw new ArgumentNullException(nameof(calculator));
            Contract.EndContractBlock();

            this.context = context;
            this.calculator = calculator;
            this.bus = bus;
        }

        #endregion

        #region IDealManagementService Members

        /// <summary>
        /// Creates a <see cref="DealModel"/> for the indicated <see cref="ClientRef"/> using the default rules.
        /// </summary>
        /// <param name="userId">The identifier of the user to create the <see cref="DealModel"/> for.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        /// <returns>A new <see cref="DealModel"/> that can be edited or updated to the correct values.</returns>
        public async Task<DealModel> Default(Guid userId, CancellationToken cancellation = default(CancellationToken))
        {
            var query = this.context.Set<ClientRef>()
                .Where(c => c.UserId == userId)
                .Select(c => new
                {
                    c.BusinessName,
                    c.FirstName,
                    c.LastName,
                    c.UserId,
                    Email = c.UserName,
                    c.OwnerId
                });

            var client = await query.FirstOrDefaultAsync(cancellation).ConfigureAwait(false);
            if (client == null) throw new ValidationException($"An error has occured while creating the deal. The user {userId} could not be found.");
            
            var model = new DealModel
            {
                Amount = 0,
                Description = "a new deal",
                Title = $"Deal for {AccurateAppend.Accounting.PartyExtensions.BuildCompositeName(client.FirstName, client.LastName, client.Email)}",
                UserId = userId,
                OwnerId = client.OwnerId,
                SuppressNotifications = true,
                AutoBill = false
            };

            return model;
        }

        /// <summary>
        /// Stores a new <see cref="DealModel"/> to the system.
        /// </summary>
        /// <remarks>
        /// Publishes the <see cref="DealCreatedEvent"/>.
        /// </remarks>
        /// <param name="model">The <see cref="DealModel"/> that needs to be stored.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        /// <returns>The identifier of the new deal.</returns>
        public async Task<Int32> Create(DealModel model, CancellationToken cancellation = default(CancellationToken))
        {
            Validator.ValidateObject(model, new ValidationContext(model, null, null));

            var adminUserId = model.OwnerId.Value;

            var client = await this.context
                .SetOf<ClientRef>()
                .FirstOrDefaultAsync(c => c.UserId == model.UserId, cancellation);
            if (client == null) throw new ValidationException($"User {model.UserId} does not exist");

            var deal = new MutableDeal(client, adminUserId);

            deal.Title = model.Title;
            deal.Description = model.Description;
            deal.Amount = model.Amount;
            deal.Instructions = model.Instructions;
            deal.SuppressNotifications = model.SuppressNotifications;
            
            deal.Notes.Add("Created");

            var order = deal.CreateOrder();
            order.PerformAutoBilling = model.AutoBill;

            this.context.SetOf<MutableDeal>().Add(deal);

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                await this.context.SaveChangesAsync(cancellation);

                Debug.Assert(deal.Id != null);
                Debug.Assert(deal.Orders.First().Id != null);

                // Raise event
                await this.RaiseNewDealEvent(deal);

                transaction.Complete();
            }

            return deal.Id.Value;
        }

        /// <summary>
        /// Stores a new <see cref="DealBinder"/> to the system.
        /// </summary>
        /// <remarks>
        /// <note type="implementnotes">This overload only exists as an api for the New Deal From Job use case to put the deal and order data in one go.</note>
        /// Publishes the <see cref="DealCreatedEvent"/>.
        ///
        /// If the supplied <paramref name="model"/> has the <see cref="UserExtensions.SystemUser"/> value, the <see cref="ClientRef.OwnerId"/> value will be used instead.
        /// </remarks>
        /// <param name="publicKey">The public key of the new deal.</param>
        /// <param name="model">The <see cref="DealModel"/> that needs to be stored.</param>
        /// <param name="manifest">The <see cref="ManifestBuilder"/> describing the job instructions.</param>
        /// <param name="report">The <see cref="ProcessingReport"/> describing the match results of the appended file.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        /// <returns>The identifier of the new deal.</returns>
        public async Task<Int32> Create(Guid publicKey, DealModel model, ManifestBuilder manifest, ProcessingReport report, CancellationToken cancellation = default(CancellationToken))
        {
            if (manifest == null) throw new ArgumentNullException(nameof(manifest));
            if (report == null) throw new ArgumentNullException(nameof(report));
            Contract.EndContractBlock();

            Validator.ValidateObject(model, new ValidationContext(model, null, null));

            var userId = model.UserId;
            var costService = await this.CreateCostsForClient(userId, cancellation).ConfigureAwait(false);

            var client = await this.context
                .SetOf<ClientRef>()
                .FirstOrDefaultAsync(c => c.UserId == userId, cancellation);
            if (client == null) throw new ValidationException($"User {userId} does not exist");

            var deal = new MutableDeal(client);
            
            deal.Description = model.Description;
            deal.Title = model.Title;
            deal.SuppressNotifications = true;
            if (model.OwnerId != UserExtensions.SystemUserId) deal.ChangeOwner(model.OwnerId.Value);

            // Create a note detailing the JobId
            if (manifest.JobId > 0) deal.Notes.Add($"Deal created from Job {manifest.JobId}");
            
            // Tag the processing report to the order
            var order = deal.CreateOrder(publicKey);
            order.Processing.AssociateReport(report.ToXml());
            order.PerformAutoBilling = false;

            this.context.SetOf<MutableDeal>().Add(deal);

            await this.FillFromRateCard(order, costService, manifest, report, cancellation);

            deal.Amount = deal.Total(); // HACK: we don't have the events in place to keep this synched

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                await this.context.SaveChangesAsync(cancellation);

                Debug.Assert(deal.Id != null);
                Debug.Assert(deal.Orders.First().Id != null);

                // Raise event
                await this.RaiseNewDealEvent(deal);

                transaction.Complete();
            }
            
            return deal.Id.Value;
        }

        /// <summary>
        /// Stores the updated <see cref="DealModel"/> content.
        /// </summary>
        /// <param name="model">The new content to be updated.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        public async Task Update(DealModel model, CancellationToken cancellation = default(CancellationToken))
        {
            Validator.ValidateObject(model, new ValidationContext(model, null, null));

            var deal = await this.context
                .SetOf<DealBinder>()
                .Where(d => d.Id == model.Id)
                .AreEditable()
                .Include(d => d.Orders)
                .FirstOrDefaultAsync(cancellation)
                .ConfigureAwait(false);
            if (deal == null) throw new InvalidOperationException($"Deal {model.Id} does not exist or is not editable");

            deal.Title = model.Title;
            deal.Description = model.Description;
            deal.Amount = model.Amount;
            deal.Instructions = model.Instructions;
            deal.SuppressNotifications = model.SuppressNotifications;
            
            deal.ChangeOwner(model.OwnerId.Value);

            deal.Orders.OfType<BillableOrder>().ForEach(o => o.PerformAutoBilling = model.AutoBill);
            
            await this.context.SaveChangesAsync(cancellation);
        }

        /// <summary>
        /// Locates the indicated editable deal model, if exists.
        /// </summary>
        /// <param name="dealId">The identifier of the editable deal to find.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        /// <returns>If found and editable, the <see cref="DealModel"/> information; Otherwise null.</returns>
        public async Task<DealModel> FindDeal(Int32 dealId, CancellationToken cancellation = default(CancellationToken))
        {
            var deal = await this.context
                .SetOf<DealBinder>()
                .Where(d => d.Id == dealId)
                .AreEditable()
                .Select(d =>
                    new DealModel
                    {
                        Id = dealId,
                        Instructions = d.Instructions,
                        Amount = d.Amount,
                        Description = d.Description,
                        Title = d.Title,
                        UserId = d.Client.UserId,
                        OwnerId = d.OwnerId,
                        SuppressNotifications = d.SuppressNotifications,
                        AutoBill = d.Orders.OfType<BillableOrder>().FirstOrDefault().PerformAutoBilling
                    })
                .FirstOrDefaultAsync(cancellation)
                .ConfigureAwait(false);

            return deal;
        }

        #endregion

        #region Order

        /// <summary>
        /// Stores the updated <see cref="OrderDetail"/> content.
        /// </summary>
        /// <param name="model">The new content to be updated.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        public async Task Update(OrderDetail model, CancellationToken cancellation = default(CancellationToken))
        {
            Validator.ValidateObject(model, new ValidationContext(model, null, null));

            var orderId = model.Id;
            var userId = model.UserId;

            var entity = await this.context
                .SetOf<AccurateAppend.Sales.Order>()
                .AreEditable()
                .Include(o => o.Lines)
                .Include(o => o.Deal)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.Deal.Client.UserId == userId, cancellation)
                .ConfigureAwait(false);

            if (entity == null) throw new InvalidOperationException("Order does not exist or cannot be edited.");

            // Clear out missing or changed products
            foreach (var existingItem in entity.Lines.ToArray())
            {
                var matchingInput = model.Items.FirstOrDefault(i => i.Id == existingItem.Id);
                if (matchingInput == null || !existingItem.Product.Equals(matchingInput.ProductName)) entity.Lines.Remove(existingItem);
            }

            // Pre-load all the products to prevent N+1
            var allProductKeys = model.Items.Select(p => p.ProductName).ToArray();
            var allProducts = await this.context.SetOf<Product>().Where(p => allProductKeys.Contains(p.Key)).ToArrayAsync(cancellation);

            foreach (var item in model.Items)
            {
                ProductLine line = null;

                if (item.Id > 0 && entity.Lines.Any(i => i.Id == item.Id))
                {
                    line = entity.Lines.First(i => i.Id == item.Id);
                }
                else
                {
                    var product = allProducts.FirstOrDefault(p => p.Key == item.ProductName);
                    if (product != null) line = entity.CreateLine(product);
                }

                if (line == null) continue;

                line.Quantity = item.Quantity;
                line.Price = item.Cost;
            }

            entity.OrderMinimum = model.OrderMinimum;
            entity.Deal.Amount = entity.Total(); // HACK: we don't have the events in place to keep this synched

            await this.context.SaveChangesAsync(cancellation);
        }

        /// <summary>
        /// Stores the updated <see cref="BillableOrder"/> content.
        /// </summary>
        /// <remarks>
        /// <note type="implementnotes">This overload only exists as a temporary workaround for the Update Order From Job use case to put the updated order values.</note>
        /// </remarks>
        /// <param name="orderId">The identifier of the <see cref="BillableOrder"/> to update.</param>
        /// <param name="manifest">The <see cref="ManifestBuilder"/> describing the job instructions.</param>
        /// <param name="report">The <see cref="ProcessingReport"/> describing the match results of the appended file.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        public async Task Update(Int32 orderId, ManifestBuilder manifest, ProcessingReport report, CancellationToken cancellation = default(CancellationToken))
        {
            if (manifest == null) throw new ArgumentNullException(nameof(manifest));
            if (report == null) throw new ArgumentNullException(nameof(report));
            Contract.EndContractBlock();

            var order = await this.LoadOrder(orderId, cancellation).ConfigureAwait(false);

            if (order == null) throw new InvalidOperationException($"An error has occured while updating the order. The order {orderId} could not be found or is not editable.");

            var costService = await this.CreateCostsForClient(order.Deal.Client.UserId, cancellation);

            await this.FillFromRateCard(order, costService, manifest, report, cancellation);

            await this.context.SaveChangesAsync(cancellation);
        }

        /// <summary>
        /// Stores the updated <see cref="BillableOrder"/> content.
        /// </summary>
        /// <remarks>
        /// <note type="implementnotes">This overload only exists as a temporary workaround for the Update Order From Job use case to put the updated order values.</note>
        /// </remarks>
        /// <param name="orderId">The identifier of the <see cref="BillableOrder"/> to update.</param>
        /// <param name="report">The <see cref="ProcessingReport"/> describing the match results of the appended file.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        public async Task Update(Int32 orderId, ProcessingReport report, CancellationToken cancellation = default(CancellationToken))
        {
            if (report == null) throw new ArgumentNullException(nameof(report));
            Contract.EndContractBlock();

            var order = await this.LoadOrder(orderId, cancellation).ConfigureAwait(false);

            if (order == null) throw new InvalidOperationException($"An error has occured while updating the order. The order {orderId} could not be found or is not editable.");

            var costService = await this.CreateCostsForClient(order.Deal.Client.UserId, cancellation);

            await this.FillFromRateCard(order, costService, report, cancellation);

            await this.context.SaveChangesAsync(cancellation);
        }

        private async Task<OrderDetail> FindOrder(IQueryable<AccurateAppend.Sales.Order> query, CancellationToken cancellation)
        {
            var order = await query.Select(o =>
                    new
                    {
                        o.Id,
                        o.OrderMinimum,
                        o.PublicKey,
                        o.Deal.Client.UserId,
                        Lines = o.Lines.Select(l =>
                            new
                            {
                                l.Id,
                                l.Price,
                                l.Description,
                                l.Maximum,
                                ProductName = l.Product.Key,
                                l.Quantity
                            }
                        )
                    }
                )
                .FirstOrDefaultAsync(cancellation)
                .ConfigureAwait(false);
            if (order == null) return null;

            var model = new OrderDetail()
            {
                Id = order.Id.Value,
                OrderMinimum = order.OrderMinimum,
                PublicKey = order.PublicKey,
                UserId = order.UserId
            };

            model.Items.AddRange(
                order.Lines.Select(line =>

                    new OrderItemModel()
                    {
                        Id = line.Id,
                        Cost = line.Price,
                        Description = line.Description,
                        Maximum = line.Maximum,
                        ProductName = line.ProductName,
                        Quantity = line.Quantity,
                        UserId = order.UserId
                    }));

            return model;
        }

        /// <summary>
        /// Locates the indicated editable order model by identifier, if exists.
        /// </summary>
        /// <param name="orderId">The identifier of the editable order to find.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        /// <returns>If found and editable, the <see cref="OrderDetail"/> information; Otherwise null.</returns>
        public async Task<OrderDetail> FindOrder(Int32 orderId, CancellationToken cancellation = default(CancellationToken))
        {
            var query = this.context
                .SetOf<AccurateAppend.Sales.Order>()
                .AreEditable()
                .Where(o => o.Id == orderId);

            var order = await this.FindOrder(query, cancellation).ConfigureAwait(false);
            return order;
        }

        /// <summary>
        /// Locates the indicated editable order model by public key, if exists.
        /// </summary>
        /// <param name="publicKey">The public key of the editable order to find.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        /// <returns>If found and editable, the <see cref="OrderDetail"/> information; Otherwise null.</returns>
        public async Task<OrderDetail> FindOrder(Guid publicKey, CancellationToken cancellation = default(CancellationToken))
        {
            var orderKey = publicKey.ToString();

            var query = this.context
                .SetOf<AccurateAppend.Sales.Order>()
                .AreEditable()
                .Where(o => o.PublicKey == orderKey);

            var order = await this.FindOrder(query, cancellation).ConfigureAwait(false);
            return order;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Loads an editable <see cref="BillableOrder"/> for the indicated <paramref name="orderId"/> ready to be updated.
        /// </summary>
        /// <param name="orderId">The identifier of the editable order to find.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        /// <returns>If found and editable, the <see cref="BillableOrder"/> instance; Otherwise null.</returns>
        protected virtual Task<BillableOrder> LoadOrder(Int32 orderId, CancellationToken cancellation)
        {
            return this.context.SetOf<BillableOrder>()
                .AreEditable()
                .OfType<BillableOrder>()
                .Where(o => o.Id == orderId)
                .Include(o => o.Deal)
                .Include(o => o.Deal.Client)
                .SingleOrDefaultAsync(cancellation);
        }

        /// <summary>
        /// Populates the <paramref name="order"/> for the indicated job processing data.
        /// </summary>
        /// <remarks>
        /// Centralizes the access to the <see cref="IOrderCalculationService"/> for this component.
        /// </remarks>
        /// <param name="order">The <see cref="BillableOrder"/> to populate.</param>
        /// <param name="costService">The <see cref="ICostService"/> providing the appropriate cost structures for the <paramref name="order"/>.</param>
        /// <param name="manifest">The manifest to populate the billing from.</param>
        /// <param name="processingReport">The processing report to populate from.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        protected virtual Task FillFromRateCard(BillableOrder order, ICostService costService, ManifestBuilder manifest, ProcessingReport processingReport, CancellationToken cancellation)
        {
            return this.calculator.FillFromRateCard(order, costService, manifest, processingReport, cancellation);
        }

        /// <summary>
        /// Populates the <paramref name="order"/> for the indicated job processing data.
        /// </summary>
        /// <remarks>
        /// Centralizes the access to the <see cref="IOrderCalculationService"/> for this component.
        /// </remarks>
        /// <param name="order">The <see cref="BillableOrder"/> to populate.</param>
        /// <param name="costService">The <see cref="ICostService"/> providing the appropriate cost structures for the <paramref name="order"/>.</param>
        /// <param name="processingReport">The processing report to populate from.</param>
        /// <param name="cancellation">A <see cref="CancellationToken"/> that is used to signal the intention to cancel an asynchronous operation.</param>
        protected virtual Task FillFromRateCard(BillableOrder order, ICostService costService, ProcessingReport processingReport, CancellationToken cancellation)
        {
            return this.calculator.FillFromRateCard(order, costService, processingReport, cancellation);
        }

        /// <summary>
        /// Publishes the <see cref="DealCreatedEvent"/> for the supplied <see cref="DealBinder"/>.
        /// </summary>
        /// <param name="deal">The deal to publish an event for.</param>
        protected virtual Task RaiseNewDealEvent(DealBinder deal)
        {
            Debug.Assert(deal.Id != null);
            Debug.Assert(deal.Orders.First().Id != null);

            var order = deal.Orders.First();

            var @event = new DealCreatedEvent()
            {
                DealId = deal.Id.Value,
                PublicKey = new Guid(order.PublicKey),
                DateCreated = DateTime.UtcNow,
                Amount = deal.Total(),
                Client = deal.Client.UserId
            };

            return this.bus.Publish(@event, new PublishOptions());
        }

        #endregion

        #region Factories

        /// <summary>
        /// Used to create an <see cref="ICostService"/> specific to the indicated <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The identifier of the client to build a cost service for.</param>
        /// <param name="cancellation">The <see cref="CancellationToken"/> used to signal the desire for an asynchronous process to cancel.</param>
        /// <returns>The <see cref="ICostService"/> specific to the indicated customer.</returns>
        public virtual async Task<ICostService> CreateCostsForClient(Guid userId, CancellationToken cancellation = default(CancellationToken))
        {
            var client = await this.context
                .SetOf<ClientRef>()
                .SingleOrDefaultAsync(c => c.UserId == userId, cancellation)
                .ConfigureAwait(false);

            if (client == null) throw new ValidationException($"User {userId} does not exist");

            var costService = new CustomerCostService(client, this.context);
            return costService;
        }

        #endregion
    }
}
