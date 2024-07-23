using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using AccurateAppend.Core.Definitions;
using DAL.Databases;
using DAL.Typed;

namespace AccurateAppend.ListBuilder.Models
{
    /// <summary>
    /// Contains the set of named values for a single row output from the list builder.
    /// </summary>
    public class Record
    {    
        // List rather than dictionary because ordering is important. 
        // First string is the field name. second string is the field value.
        private readonly IList<KeyValuePair<String, String>> fields = new List<KeyValuePair<String, String>>();

        /// <summary>
        /// Appends a field name and field value pair to the record.
        /// </summary>
        /// <param name="fieldName">What the field will be called in the output header row, for example FirstName</param>
        /// <param name="fieldValue">The value of the field, for example Steve</param>
        public void Add(String fieldName, String fieldValue)
        {
            this.fields.Add(new KeyValuePair<String, String>(fieldName, fieldValue ?? String.Empty));
        }
     
        public override String ToString()
        {
            return String.Join(",", fields.Select(kvp=>kvp.Value));
        }        


        public virtual String GetHeaderRow()
        {
            return String.Join(",", this.fields.Select(kvp => kvp.Key));
        }

        /// <summary>
        /// Factory method which builds a record containing the fields of interest using the flat typed record
        /// </summary>        
        /// <param name="fieldsOfInterest">Fields in the reocrds. Exactly these exist in the output regardless of what values are provided.</param>
        /// <param name="values">The values to use for the fields</param>
        public static Record BuildRecord(IEnumerable<KeyValuePair<FieldName,IPartialQualifiedType>> fieldsOfInterest, FlatTypedRecord values)
        {
            Contract.Ensures(Contract.Result<Record>() != null);

            // TODO: Pull in the Fieldname along with fieldsOfInterest as a pair of some kind.

            var rec = new Record();
            foreach (var field in fieldsOfInterest)
            {
                var qt = DAL.DataAccess.DataBuilder.ConsumerProfile.GetParentType(field.Value);

                var datum = values.GetOrDefault(qt);

                var value = String.Empty;
                
                if (datum != null)
                    value = DAL.DataAccess.DataBuilder.ConsumerProfile.UnpackType(field.Value, values[qt].Value);
                
                rec.Add(field.Key.ToString(), value);
            }

            return rec;
        }
    }
}
