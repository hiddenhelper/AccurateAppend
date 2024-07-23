using System.Web.Mvc;
using AccurateAppend.Core.Configuration;
using DomainModel.MvcModels;

namespace AccurateAppend.Websites.Admin
{
    /// <summary>
    /// Configures the custom MVC modelbinders in use in the application.
    /// </summary>
    /// <remarks>
    /// Registers all custom model binders-by type.
    /// </remarks>
    public class ModelBinderBootstrapper : AbstractBootstrapper
    {
        #region Overrides

        /// <inheritdoc />
        public override void BeginInit()
        {
            ModelBinders.Binders.Add(typeof(MvcActionModel), new McvActionModelBinder());
        }

        /// <inheritdoc />
        public override void EndInit()
        {
            // no op
        }

        #endregion

        #region Helper

        /// <summary>
        /// Provides a convienent abstraction over this component to run everything in one call.
        /// </summary>
        public static void Execute()
        {
            var boostrapper = new ModelBinderBootstrapper();
            boostrapper.Run();
        }

        #endregion
    }
}