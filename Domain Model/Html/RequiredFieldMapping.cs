using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using AccurateAppend.JobProcessing.Manifest;

namespace DomainModel.Html
{
    /// <summary>
    /// Html Helper for required field mappings.
    /// </summary>
    public static class RequiredFieldMapping
    {
        /// <summary>
        /// Utility method for constructing a drop down of the available input fields for column mapping.
        /// </summary>
        public static MvcHtmlString BuildRequiredFieldsDropDown(IEnumerable<Field> inputFields, Int32 i)
        {
            var select = new StringBuilder();
            select.AppendLine($"<select  name=\"{i}\" class=\"ct\">");
            select.AppendLine("<option value=\"\">-- Select Column -</option>");

            foreach (var field in inputFields)
            {
                select.AppendLine($"<option required='{field.Required}' value='{field.MetaFieldName}'>{field.MetaFieldName}{(field.Required ? "*" : "")}</option>");
            }

            select.AppendLine("</select>");

            return new MvcHtmlString(select.ToString());
        }
    }
}
