using System;
using System.ComponentModel;
using System.Web.Mvc;

namespace DomainModel.MvcModels
{
    /// <summary>
    /// Customized version of the <see cref="DefaultModelBinder"/> altered so that it will work
    /// with being a nested complex type in another model (which the default binder can't figure
    /// out for it's life).
    /// </summary>
    /// <remarks>
    /// The <see cref="DefaultModelBinder"/> simply fails a check for building so cannot bind
    /// to this model type. By subverting this initialization check and simply bypassing it (by
    /// overriding the <see cref="DefaultModelBinder.BindModel"/> method) we can then take
    /// the path of the code found in the internal implementation (if the path was allowed) and
    /// simply duplicate the method's here.
    /// 
    /// <example>
    /// How to install this model binder:
    /// <code>
    /// <![CDATA[
    /// // At app start up
    /// ModelBinders.Binders.Add(typeof(MvcActionModel), new McvActionModelBinder());
    /// ]]>
    /// </code>
    /// </example>
    /// </remarks>
    public class McvActionModelBinder : DefaultModelBinder
    {
        /// <inheritdoc />
        public override Object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (!bindingContext.ModelType.IsAssignableFrom(typeof(MvcActionModel))) throw new InvalidOperationException($"The type {bindingContext.ModelType} cannot be assigned {typeof(MvcActionModel)} as required by this model binder.");

            var model = (MvcActionModel)bindingContext.Model ?? this.CreateModel(controllerContext, bindingContext, bindingContext.ModelType);

            this.BindComplexElementalModel(controllerContext,bindingContext,bindingContext.Model);

            return model;
        }

        internal void BindComplexElementalModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Object model)
        {
            // need to replace the property filter + model object and create an inner binding context
            var newBindingContext = this.CreateComplexElementalModelBindingContext(controllerContext, bindingContext, model);

            // validation
            if (!this.OnModelUpdating(controllerContext, newBindingContext)) return;

            this.BindProperties(controllerContext, newBindingContext);
            this.OnModelUpdated(controllerContext, newBindingContext);
        }

        internal ModelBindingContext CreateComplexElementalModelBindingContext(ControllerContext controllerContext, ModelBindingContext bindingContext, object model)
        {
            var bindAttr = (BindAttribute) this.GetTypeDescriptor(controllerContext, bindingContext).GetAttributes()[typeof(BindAttribute)];
            var newPropertyFilter = (bindAttr != null)
                                                      ? propertyName => bindAttr.IsPropertyAllowed(propertyName) && bindingContext.PropertyFilter(propertyName)
                                                      : bindingContext.PropertyFilter;

            var newBindingContext = new ModelBindingContext()
            {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, bindingContext.ModelType),
                ModelName = bindingContext.ModelName,
                ModelState = bindingContext.ModelState,
                PropertyFilter = newPropertyFilter,
                ValueProvider = bindingContext.ValueProvider
            };

            return newBindingContext;
        }
        private void BindProperties(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var properties = this.GetModelProperties(controllerContext, bindingContext);
            var propertyFilter = bindingContext.PropertyFilter;

            // Loop is a performance sensitive codepath so avoid using enumerators.
            for (var i = 0; i < properties.Count; i++)
            {
                var property = properties[i];
                if (ShouldUpdateProperty(property, propertyFilter))
                {
                    this.BindProperty(controllerContext, bindingContext, property);
                }
            }
        }

        private static Boolean ShouldUpdateProperty(PropertyDescriptor property, Predicate<String> propertyFilter)
        {
            if (property.IsReadOnly && !CanUpdateReadonlyTypedReference(property.PropertyType))
            {
                return false;
            }

            // if this property is rejected by the filter, move on
            if (!propertyFilter(property.Name))
            {
                return false;
            }

            // otherwise, allow
            return true;
        }

        private static Boolean CanUpdateReadonlyTypedReference(Type type)
        {
            // value types aren't strictly immutable, but because they have copy-by-value semantics
            // we can't update a value type that is marked readonly
            if (type.IsValueType)
            {
                return false;
            }

            // arrays are mutable, but because we can't change their length we shouldn't try
            // to update an array that is referenced readonly
            if (type.IsArray)
            {
                return false;
            }

            // special-case known common immutable types
            return type != typeof(String);
        }
    }
}