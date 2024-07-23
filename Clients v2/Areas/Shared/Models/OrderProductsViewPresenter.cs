using System;
using DomainModel.MvcModels;

namespace AccurateAppend.Websites.Clients.Areas.Shared.Models
{
    /// <summary>
    /// Is a wrapper for a <see cref="NewOrderModel"/> type that instructs the shared select products view on how to render and acquire data.
    /// </summary>
    [Serializable()]
    public class OrderProductsViewPresenter
    {
        /// <summary>
        /// Contains the full MVC action that should receive the posted order content.
        /// </summary>
        public MvcActionModel Postback { get; set; }

        /// <summary>
        /// Contains the actual order information that needs to be passed along.
        /// </summary>
        public NewOrderModel Order { get; set; }

        /// <summary>
        /// Gets the literal page help text for the view.
        /// </summary>
        public String HelpText { get; set; }

        /// <summary>
        /// Gets the literal text of a special notice that is popped up in the view
        /// </summary>
        public String SpecialNotice { get; set; }

        /// <summary>
        /// Gets the path (relative) to the partial view containing the order layout to be used.
        /// </summary>
        /// <remarks>
        /// Holds the literal path that would be provided to the <see cref=" System.Web.Mvc.Html.PartialExtensions.Partial(System.Web.Mvc.HtmlHelper, String)"/> method in a view.
        /// Allows the controller specific product layout view to be specified from the controller and keeping the view code simple (smart controllers, dumb views)
        /// </remarks>
        public String OrderViewPath { get; set; }
    }
}