using System;
using System.Web.Mvc;

namespace DomainModel.JsonNET
{
    /// <summary>
    /// Represents an attribute that is used to associate a model type to a model-builder type while also allowing input parameters to be supplied to the binder instance.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class ModelBinderExAttribute : CustomModelBinderAttribute
    {
        #region Fields

        private Object[] inputs;

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets or sets the type of the binder.
        /// </summary>
        /// 
        /// <returns>
        /// The type of the binder.
        /// </returns>
        public Type BinderType { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelBinderExAttribute"/> class.
        /// </summary>
        /// <param name="binderType">The type of the binder.</param><exception cref="T:System.ArgumentNullException">The <paramref name="binderType"/> parameter is null.</exception>
        /// <param name="inputs">The optional set of constructor arguments that should be supplied to the <see cref="BinderType"/> being created. Parameters should be of the correct type and in order.</param>
        public ModelBinderExAttribute(Type binderType, params Object[] inputs)
        {
            if (binderType == null) throw new ArgumentNullException(nameof(binderType));
            if (!typeof (IModelBinder).IsAssignableFrom(binderType)) throw new ArgumentException($"Type '{binderType.FullName}' is not a model binder", nameof(binderType));

            this.BinderType = binderType;
            this.inputs = inputs;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Retrieves an instance of the model binder.
        /// </summary>
        /// <returns>
        /// A reference to an object that implements the <see cref="T:System.Web.Mvc.IModelBinder"/> interface.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">An error occurred while an instance of the model binder was being created.</exception>
        public override IModelBinder GetBinder()
        {
            try
            {
                var binder = (IModelBinder) Activator.CreateInstance(this.BinderType, this.inputs);
                return binder;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Cannot create binder '{this.BinderType.FullName}'. See the {nameof(ex.InnerException)} property for more details.", ex);
            }
        }

        #endregion
    }
}
