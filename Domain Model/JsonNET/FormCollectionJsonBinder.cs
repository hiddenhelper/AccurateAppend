using System;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace DomainModel.JsonNET
{
    /// <summary>
    /// Custom model binder that will locate posted form content using the model parameter name and type for deserialization of json text content.
    /// </summary>
    public class FormCollectionJsonBinder : IModelBinder
    {
        #region Fields

        private readonly String formFieldName;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FormCollectionJsonBinder"/> class. Created instances
        /// will use the name of the <see cref="ModelBindingContext.ModelName">model parameter</see> for the
        /// form value to bind the content to.
        /// </summary>
        public FormCollectionJsonBinder()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FormCollectionJsonBinder"/> class. Created instances
        /// will use the supplied <paramref name="formFieldName"/> to locate the form value to bind the content to.
        /// </summary>
        /// <param name="formFieldName">
        /// The name of the form field to use for locating the json content. If null, the default behavior will be used instead.
        /// </param>
        public FormCollectionJsonBinder(String formFieldName)
        {
            this.formFieldName = formFieldName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the optional name of the form field to locate the json content from.
        /// </summary>
        /// <value>The optional name of the form field to locate the json content from.</value>
        public String FormFieldName { get { return this.formFieldName; } }

        #endregion

        #region IModelBinder Members

        /// <summary>
        /// Binds the model to a value by using the specified controller context and binding context.
        /// </summary>
        /// <returns>
        /// The bound value.
        /// </returns>
        /// <param name="controllerContext">The controller context.</param><param name="bindingContext">The binding context.</param>
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var request = controllerContext.HttpContext.Request;

            // Check if request validation is required
            var shouldPerformRequestValidation = controllerContext.Controller.ValidateRequest && bindingContext.ModelMetadata.RequestValidationEnabled;

            var formField = String.IsNullOrWhiteSpace(this.FormFieldName)
                ? bindingContext.ModelName
                : this.FormFieldName;

            var modelType = bindingContext.ModelType;

            var json = (shouldPerformRequestValidation ? request.Form : request.Unvalidated.Form).Get(formField);
            if (String.IsNullOrEmpty(json)) return null;

            var model = JsonConvert.DeserializeObject(json, modelType,
                new JsonConverter[]
                {
                    new JsonEnumConvertor(),
                    new DateTimeConverter(),
                    new JsonGuidConverter()
                });

            return model;
        }

        #endregion
    }
}
