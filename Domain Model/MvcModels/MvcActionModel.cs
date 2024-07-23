using System;

namespace DomainModel.MvcModels
{
    /// <summary>
    /// Provides an MVC model type that is used to indicate to a presenter a specific action to interact with.
    /// Generally used just with a view shared in multiple controllers, especially with form posts.
    /// </summary>
    /// <remarks>
    /// <note type="warning">Use of this type as a nested model generally requires the use of the <see cref="McvActionModelBinder"/> type.</note>
    /// </remarks>
    [Serializable()]
    public class MvcActionModel
    {
        /// <summary>
        /// Holds the area name that is needed by the view to properly create back end requests to the area specific controller via common order interfaces
        /// </summary>
        public String AreaName { get; set; }

        /// <summary>
        /// Holds the action name that is needed by the view to properly create back end requests to the area specific controller via common order interfaces
        /// </summary>
        public String ActionName { get; set; }

        /// <summary>
        /// Holds the controller name that is needed by the view to properly create back end requests to the area specific controller via common order interfaces
        /// </summary>
        public String ControllerName { get; set; }
    }
}