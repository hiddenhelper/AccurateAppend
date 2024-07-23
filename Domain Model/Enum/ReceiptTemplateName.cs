using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DomainModel.Enum
{
    [Serializable()]
    public enum ReceiptTemplateName
    {
        /// <summary>
        /// Build Subscription based content.
        /// </summary>
        [Display, Description("Subscription")]
        Subscription,

        /// <summary>
        /// Build Usage based content.
        /// </summary>
        [Display, Description("Usage")]
        Usage,

        /// <summary>
        /// Build NationBuilder based content.
        /// </summary>
        [Display, Description("Nation Builder")]
        NationBuilder,

        /// <summary>
        /// Build standard content with MatchType aggregation.
        /// </summary>
        [Display, Description("Default - IND,HH")]
        IndHh,

        /// <summary>
        /// Build standard content with MatchLevel aggregation.
        /// </summary>
        [Display, Description("Default - Match Level")]
        MatchLevel,

        /// <summary>
        /// Build Refund based content.
        /// </summary>
        [Display, Description("Refund")]
        Refund,

        /// <summary>
        /// Build Public Order based content.
        /// </summary>
        [Display, Description("Public")]
        Public
    }
}