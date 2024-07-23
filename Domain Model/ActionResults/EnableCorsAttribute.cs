using System;
using System.Web.Mvc;

namespace DomainModel.ActionResults
{
    /// <summary>
    /// Allows CORS support via a custom <see cref="Attribute"/> in MVC.
    /// </summary>
    public sealed class EnableCorsAttribute : ActionFilterAttribute
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="EnableCorsAttribute"/> class.
        /// </summary>
        /// <remarks>
        /// Defaults to allowing the "*" value.
        /// </remarks>
        public EnableCorsAttribute()
        {
            this.AllowOrigin = "*";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the value of the "Access-Control-Allow-Origin" header.
        /// </summary>
        public String AllowOrigin { get; set; }

        #endregion

        #region Overrides
        
        /// <summary>
        /// Called by the ASP.NET MVC framework before the action result executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        
            if (!String.IsNullOrWhiteSpace(this.AllowOrigin))
            {
                filterContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", this.AllowOrigin);
            }
        }

        #endregion
    }
}