using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AccurateAppend.Core.Collections.Generic;
using AccurateAppend.Core.Definitions;
using AccurateAppend.ListBuilder.Models;
using DAL.Databases;

namespace AccurateAppend.ListBuilder.DataSources.ConsumerProfile
{
    /// <summary>
    /// Default implementation of the <see cref="IDataAccess"/> API.
    /// </summary>
    public class DataAccess : IDataAccess
    {
        #region Fields

        private static readonly FieldName[] StandardFields = { FieldName.FirstName, FieldName.MiddleName, FieldName.LastName, FieldName.StreetAddress, FieldName.City, FieldName.State, FieldName.PostalCode, FieldName.DateOfBirth };

        #endregion

        #region IDataAcess Members

        /// <inheritdoc />
        public virtual Task<IEnumerable<Record>> GetRecordsAsync(ListCriteria criteria, CancellationToken cancellation)
        {
            var error = criteria.Validate().FirstOrDefault();
            if (error != null) throw new InvalidOperationException(error.ErrorMessage);

            var dalListCriteria = ListCriteria.ConvertListCriteria(criteria);

            var outputFieldNames = this.DetermineOutputFields(dalListCriteria.Keys);

            var task = DAL.DataAccess.ListBuilder.Database.GetListAsync(dalListCriteria, cancellation)
                .ContinueWith(t => t.Result.SelectMany(tr => tr.Denormalize()), cancellation)
                .ContinueWith(t => t.Result.Select(ftr => Record.BuildRecord(outputFieldNames, ftr)), cancellation);

            return task;            
        }

        /// <inheritdoc />
        Task<Int32> IDataAccess.GetCountAsync(ListCriteria criteria, CancellationToken cancellation)
        {
            var error = criteria.Validate().FirstOrDefault();
            if (error != null) throw new InvalidOperationException(error.ErrorMessage);

            var dalListCriteria = ListCriteria.ConvertListCriteria(criteria);

            return DAL.DataAccess.ListBuilder.Database.GetCountAsync(dalListCriteria, cancellation);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines which fields will be output, and in which order
        /// </summary>
        /// <remarks>
        /// Standard fields then user specified fields
        /// </remarks>
        /// <param name="userSpecifiedFields"></param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<FieldName, IPartialQualifiedType>> DetermineOutputFields(IEnumerable<FieldName> userSpecifiedFields) // this complexity with the KeyValuePair<FieldName,IPartialQualifiedType> is temporary - the easiest way to get the FieldName in until we decide what we're going to use from ListCriteria going forward.
        {
            // decide which fields to will output
            var outputFields = StandardFields.Concat(userSpecifiedFields.Except(StandardFields));  // the standard fields first, then any specified by the user (but no repeats)                                           

            // translate the AA field names to dal QTs
            var outputFieldNames = outputFields // resharper warns about multiple enumeraiton but it only happens in an exceptional case so it can be ignored.
                .Select(field => new KeyValuePair<FieldName, IPartialQualifiedType>(field, DAL.DataAccess.DataBuilder.ConsumerProfile.FieldNames.GetOrDefault(field)))
                //.Where(field => field != null); // How do we handle unexpected fields? This line would cause them to be ignored.
                .ToArray(); // enumerated twice

            if (outputFieldNames.Any(fn => fn.Value == null)) // more aggressive way to handle unexpected fields: Throw an error. I think this is better because it should only happen if a customer crafts a malicious request. 
            {
                var unhandled = outputFieldNames
                    .Where(field => field.Value == null)
                    .Select(field => field.Key.ToString()); // for String.Join

                throw new Exception($"Cannot handle requested fields {String.Join(",", unhandled)}"); // for example the map tells us where AA.Definitions.FieldName.FirstName exists. no problem. But there is no "LandLine3" in the consumer profile. If a ListCriteria specified that, error.
            }

            return outputFieldNames;
        }

        #endregion
    }
}
