using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace AccurateAppend.Sales.Contracts.ViewModels
{
    /// <summary>
    /// View Model for representing common attachment files to include in a bill.
    /// </summary>
    [Serializable()]
    [DebuggerDisplay("ConsumerMatchCodes={" + nameof(CommonProcessingCodes) + ("}, NationBuilderProcessingOptions={" + nameof(NationBuilderProcessingOptions) + "}"))]
    public class CommonAttachments
    {
        /// <summary>
        /// Indicates whether the Common Processing Codes link should be included in the email template
        /// </summary>
        [Required()]
        [DisplayName("Common Processing Codes")]
        public Boolean CommonProcessingCodes { get; set; } 

        /// <summary>
        /// Indicates whether the NationBuilder instructions should be included.
        /// </summary>
        [Required()]
        [DisplayName("NationBuilder Processing Options")]
        public Boolean NationBuilderProcessingOptions { get; set; } 

        /// <summary>
        /// Determines if the core processing type documents should be attached. This does not include the <see cref="NationBuilderProcessingOptions"/> option.
        /// </summary>
        /// <returns></returns>
        public Boolean ContainsAttachments()
        {
            return this.CommonProcessingCodes;
        }
    }
}