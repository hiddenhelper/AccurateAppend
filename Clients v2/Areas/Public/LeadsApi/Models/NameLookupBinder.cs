using System;
using System.Web.Mvc;

namespace AccurateAppend.Websites.Clients.Areas.Public.LeadsApi.Models
{
    public class NameLookupBinder : DefaultModelBinder
    {
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor)
        {
            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);

            for (var i = 0; i < propertyDescriptor.Attributes.Count; i++)
            {
                var attribute = propertyDescriptor.Attributes[i] as BindingNameAttribute;

                if (attribute == null) continue;

                var value = controllerContext.HttpContext.Request[attribute.Name];
                propertyDescriptor.SetValue(bindingContext.Model, value);
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    [Serializable()]
    public class BindingNameAttribute : Attribute
    {
        public String Name { get; set; }
    }
}