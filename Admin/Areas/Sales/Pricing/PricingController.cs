using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Core.Definitions;
using AccurateAppend.Data;
using AccurateAppend.Sales;
using AccurateAppend.Sales.DataAccess;
using AccurateAppend.Websites.Admin.Areas.Sales.Pricing.Models;
using AccurateAppend.Websites.Admin.Controllers;
using DomainModel.ActionResults;
using Kendo.Mvc.UI;

namespace AccurateAppend.Websites.Admin.Areas.Sales.Pricing
{
    /// <summary>
    /// Controller managing rate cards.
    /// </summary>
    [Authorize()]
    public class PricingController : ActivityLoggingController2
    {
        #region Fields

        private readonly DefaultContext context;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PricingController"/> class.
        /// </summary>
        /// <param name="context">The <see cref="DefaultContext"/> to use for this controller instance.</param>
        public PricingController(DefaultContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Contract.EndContractBlock();

            this.context = context;
        }

        #endregion

        #region Action Methods

        /// <summary>
        /// Action to render the product selection form.
        /// </summary>
        public virtual async Task<ActionResult> Index(Guid? userId, CancellationToken cancellation)
        {
            var model = new ClientModel()
            {
                UserId = userId,
            };

            if (userId != null)
            {
                var userName = await this.context
                    .SetOf<ClientRef>()
                    .Where(u => u.UserId == userId)
                    .Select(u => u.UserName)
                    .FirstOrDefaultAsync(cancellation);
                model.UserName = userName;
                model.DownloadLink = this.Url.Action("Download", new {userId});
            }
            
            return this.View(model);
        }

        /// <summary>
        /// Action to delete all .
        /// </summary>
        public virtual async Task<ActionResult> DeleteAll(Guid userId, CancellationToken cancellation)
        {
            var ra = await this.context.Database.ExecuteSqlCommandAsync("DELETE FROM [sales].[Cost] WHERE [Category]=Convert(varchar(50), @p0)", cancellation, userId);

            return this.RedirectToAction("Index", "Pricing", new {area = "Sales", userId});
        }

        /// <summary>
        /// Action to delete all .
        /// </summary>
        public virtual async Task<ActionResult> DeleteCard(Guid userId, Int32 productId, CancellationToken cancellation)
        {
            var ra = await this.context.Database.ExecuteSqlCommandAsync("DELETE FROM [sales].[Cost] WHERE [Category]=Convert(varchar(50), @p0) AND ProductId = @p1", cancellation, userId, productId);

            return this.RedirectToAction("Index", "Pricing", new {area = "Sales", userId});
        }

        /// <summary>
        /// Returns list of products
        /// </summary>
        public virtual async Task<ActionResult> AvailableProducts(CancellationToken cancellation)
        {
            using (this.context.CreateScope(ScopeOptions.ReadOnly))
            {
                var query = await this.context
                    .SetOf<Product>()
                    .Where(p => p.Usage == ProductUsage.Admin)
                    .Select(
                        p => new
                        {
                            Name = p.Key,
                            p.Title,
                            Category = p.Category.Name
                        }
                    )
                    .ToListAsync(cancellation);

                var allData = query.Where(p =>
                    {
                        if (p.Category == "XMLServices") return true;

                        if (!Enum.TryParse(p.Name, out DataServiceOperation operation)) return false;

                        return !operation.IsPreference();
                    }).ToArray();

                var apiProducts = allData.Where(p => p.Category == "XMLServices")
                        .Select(p => new {p.Name, p.Title, Order = 2})
                        .ToArray();
                var operationData = allData.Where(p => p.Category != "XMLServices")
                        .Select(p => new { p.Name, p.Title, Order = 1 })
                        .ToArray();

                var jsonNetResult = new JsonNetResult
                {
                    Data = (operationData.Union(apiProducts)).OrderBy(p => p.Order).ThenBy(p => p.Name)
                };

                return jsonNetResult;
            }
        }

        /// <summary>
        /// Returns list of products
        /// </summary>
        public virtual async Task<ActionResult> ExistingCards([DataSourceRequest()] DataSourceRequest request, Guid userId, CancellationToken cancellation)
        {
            using (this.context.CreateScope(ScopeOptions.ReadOnly))
            {
                var query = await this.context
                    .SetOf<Cost>()
                    .Where(p => p.Category == userId.ToString())
                    .GroupBy(x => new { x.Product.Key, x.Product.Description, x.Product.Id })
                    .Select(g => new { g.Key.Key, g.Key.Description, g.Key.Id})
                    .Distinct()
                    .ToListAsync(cancellation);

                var data = query.Select(a => new
                {
                    Name = a.Key,
                    a.Description,

                    Links = new
                    {
                        EditCost = this.Url.Action("EditCost", "Pricing", new {Area = "Sales", product = a.Key, category = userId.ToString()}),
                        DeleteCard = this.Url.Action("DeleteCard", "Pricing", new {Area = "Sales", product = a.Key, userid = userId.ToString(), productId = a.Id}),
                    }
                }).ToArray();
                var result = Kendo.Mvc.Extensions.QueryableExtensions.ToDataSourceResult(data, request);
                result.Total = data.Length;
                
                var jsonNetResult = new JsonNetResult
                {
                    Data = result
                };
                return jsonNetResult;
            }
        }

        /// <summary>
        /// Displays the view to edit the <see cref="Cost"/> for a particular <see cref="Product"/>.
        /// </summary>
        public async Task<ActionResult> EditCost(String category, String product, Boolean? copyExisting, CancellationToken cancellation)
        {
            using (this.context.CreateScope(ScopeOptions.ReadOnly))
            {
                IEnumerable<CostModel> data;

                // Copy existing Rate Card, only works with Default
                if (copyExisting != null && copyExisting.Value)
                {
                    data = await this.context
                        .SetOf<Cost>()
                        .Where(c => c.Category == Cost.DefaultCategory && c.Product.Key == product)
                        .Select(
                            c => new CostModel
                            {
                                Ceiling = c.Ceiling,
                                FileMinimum = c.FileMinimum,
                                Floor = c.Floor,
                                PerMatch = c.PerMatch,
                                PerRecord = 0 // set to zero default is per match
                            }
                        )
                        .OrderBy(c => c.Floor)
                        .ThenBy(c => c.Ceiling)
                        .ToListAsync(cancellation);
                }
                else
                {
                    data = await this.context
                        .SetOf<Cost>()
                        .Where(c => c.Category == category && c.Product.Key == product)
                        .Select(
                            c => new CostModel
                            {
                                Ceiling = c.Ceiling,
                                FileMinimum = c.FileMinimum,
                                Floor = c.Floor,
                                PerMatch = c.PerMatch,
                                PerRecord = c.PerRecord
                            }
                        )
                        .OrderBy(c => c.Floor)
                        .ThenBy(c => c.Ceiling).ToListAsync(cancellation);
                }

                Guid.TryParse(category, out var userid);
                var userName = await this.context
                    .SetOf<ClientRef>()
                    .Where(u => u.UserId == userid)
                    .Select(u => u.UserName)
                    .FirstOrDefaultAsync(cancellation);

                var result = new RateCardModel(category, product) {UserId = userName == null ? null : new Guid?(userid), UserName = userName ?? String.Empty};
                result.Costs.AddRange(data);

                return this.View(result);
            }
        }

        /// <summary>
        /// Action to commit the new pricing information.
        /// </summary>
        [HttpPost()]
        public async Task<ActionResult> EditCost(RateCardModel model, CancellationToken cancellation)
        {
            if (model == null) return this.RedirectToAction(nameof(this.Index));
            if (!this.ModelState.IsValid) return this.View(model);

            model.NormalizeRates();

            try
            {
                var product = await this.context
                    .SetOf<Product>()
                    .FirstOrDefaultAsync(p => p.Key == model.Product, cancellation);
                if (product == null) return this.DisplayErrorResult($"The product {model.Product} does not exist in our system.");

                var costTable = this.context.SetOf<Cost>();
                var costs = await costTable
                    .Where(c => c.Category == model.CardName && c.Product.Key == model.Product)
                    .OrderBy(c => c.Floor)
                    .ToListAsync(cancellation);

                costs.ForEach(c => costTable.Remove(c));

                foreach (var thisModel in model.Costs)
                {
                    var thisCost = new Cost(product, model.CardName)
                    {
                        Floor = thisModel.Floor,
                        Ceiling = thisModel.Ceiling,
                        PerMatch = thisModel.PerMatch,
                        PerRecord = thisModel.PerRecord
                    };

                    costTable.Add(thisCost);
                }

                await this.context.SaveChangesAsync(cancellation);

                return this.RedirectToAction("Index", "Pricing", new {area = "Sales", model.UserId});
            }
            catch (Exception ex)
            {
                EventLogger.Logger.LogEvent(ex, Severity.Medium, $"Failure editing {model.CardName} for {model.Product}");

                return this.DisplayErrorResult(ex.Message);
            }
        }

        /// <summary>
        /// Action to render a Cost range row.
        /// </summary>
        public virtual ActionResult CostRow(Int32 floor, Int32 ceiling)
        {
            var model = new CostModel{ Floor = floor, Ceiling = ceiling };
            return this.View(model);
        }

        public virtual async Task<ActionResult> Download(Guid userId, CancellationToken cancellation)
        {
            var costTable = this.context.SetOf<Cost>();
            var costs = await costTable
                .Where(c => c.Category == userId.ToString())
                .Select(c => new {Product = c.Product.Key, c.Floor, c.Ceiling, c.PerMatch, c.PerRecord})
                .OrderBy(c => c.Product)
                .ThenBy(c => c.Floor)
                .ToListAsync(cancellation);

            var sb = new StringBuilder();
            sb.AppendLine("Product, Range Lower, Range Upper, Per Record, Per Match");

            foreach (var cost in costs)
            {
                sb.AppendLine($"{cost.Product}, {cost.Floor}, {cost.Ceiling}, {cost.PerRecord}, {cost.PerMatch}");
            }

            return this.File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "Custom Rates.csv");
        }

        #endregion
    }
}