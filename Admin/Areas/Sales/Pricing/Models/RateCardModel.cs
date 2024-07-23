using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using AccurateAppend.Sales;

namespace AccurateAppend.Websites.Admin.Areas.Sales.Pricing.Models
{
    /// <summary>
    /// A view model capable of representing a product rate card.
    /// </summary>
    [DebuggerDisplay("{" + nameof(CardName) + ("} rate card for {" + nameof(Product) + "}"))]
    public class RateCardModel : IValidatableObject
    {
        #region Fields

        private readonly ICollection<CostModel> costs = new Collection<CostModel>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RateCardModel"/>.
        /// </summary>
        public RateCardModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RateCardModel"/>.
        /// </summary>
        /// <param name="cardName">The name of the rate card.</param>
        /// <param name="product">The name of the product.</param>
        public RateCardModel(String cardName, String product)
        {
            this.CardName = cardName;
            this.Product = product;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection of <see cref="CostModel"/> for the current rate card.
        /// </summary>
        public ICollection<CostModel> Costs => this.costs;

        /// <summary>
        /// Get the product key for the card.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public String Product { get; set; }

        /// <summary>
        /// Gets the name of the rate card.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public String CardName { get; set; }

        /// <summary>
        /// UserId for the owner of the rate card
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// UserName for the owner of the rate card
        /// </summary>
        public String UserName { get; set; }
        #endregion

        #region IValidatableObject Members

        /// <inheritdoc />
        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //validation
            var ranges = this.Costs.Where(c => c != null).OrderBy(c => c.Floor).ToList();
            for (var i = 0; i < ranges.Count; i++)
            {
                var thisItem = ranges[i];
                var previousItem = i > 0 ? ranges[i - 1] : null;

                if (i == 0 && thisItem.Floor != 0)
                {
                    yield return new ValidationResult("A rate card must start with a floor at 0", new[] {nameof(thisItem.Floor)});
                }
                else
                {
                    if (previousItem != null)
                    {
                        if (thisItem.Floor <= previousItem.Ceiling) yield return new ValidationResult("A rate card cannot overlap ranges", new[] { nameof(CostModel.Floor) });

                        if (thisItem.Floor != previousItem.Ceiling + 1) yield return new ValidationResult("A rate card cannot range gaps", new[] { nameof(CostModel.Floor) });
                    }
                }
            }

            if (Cost.IsSystem(this.CardName))
            {
                if (this.Costs.Any(c => c.PerRecord == 0) || this.Costs.Any(c => c.PerMatch == 0))
                {
                    yield return new ValidationResult($"A system rate card must have both {nameof(CostModel.PerMatch)} and {nameof(CostModel.PerRecord)} pricing on a single product", new[] { nameof(CostModel.PerRecord), nameof(CostModel.PerMatch) });
                }
            }
            else
            {
                if (this.Costs.Any(c => c.PerRecord > 0) && this.Costs.Any(c => c.PerMatch > 0))
                {
                    yield return new ValidationResult($"A custom rate card cannot have both {nameof(CostModel.PerMatch)} and {nameof(CostModel.PerRecord)} pricing on a single product", new[] {nameof(CostModel.PerRecord), nameof(CostModel.PerMatch)});
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Normalizes the rate card so that the last segment extends to the maximum range. 
        /// </summary>
        public virtual void NormalizeRates()
        {
            var last = this.Costs.OrderByDescending(c => c.Ceiling).First();
            last.Ceiling = Int32.MaxValue;
        }

        #endregion
    }
}