using System;

namespace AccurateAppend.Websites.Clients.Areas.Shared.Views
{
    /// <summary>
    /// Contains the possible cart order type used to indicate the current active tab.
    /// </summary>
    [Serializable()]
    public enum CartTabActive
    {
        /// <summary>
        /// Csv upload tab.
        /// </summary>
        Csv,

        /// <summary>
        /// NationBuilder list tab.
        /// </summary>
        NationBuilder,

        /// <summary>
        /// Auto Processing Rules tab
        /// </summary>
        Automation
    }
}