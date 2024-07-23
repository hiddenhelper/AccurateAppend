using System;
using System.Diagnostics.Contracts;
using System.Linq;
using AccurateAppend.Core.Definitions;
using AccurateAppend.JobProcessing.Manifest;

namespace AccurateAppend.Websites.Clients.Areas.Order.Analyzers
{
    /// <summary>
    /// An operation suitability observer that finds suitable records in CSV data
    /// according to a single <see cref="ColumnMap"/>. Suitable defined as having
    /// a first and last name (Party) and street address and either a City and State
    /// OR a postal code value (Address).
    /// </summary>
    public class PartyBasedOperationSuitabilityObserver : OperationSuitabilityObserver
    {
        #region Fields

        private static readonly String[] EvaluatedFields =
        {
            FieldName.FullName.ToString(),
            FieldName.FirstName.ToString(),
            FieldName.LastName.ToString(),
            FieldName.StreetAddress.ToString(),
            FieldName.City.ToString(),
            FieldName.State.ToString(),
            FieldName.PostalCode.ToString()
        };

        private readonly FieldMap[] columnMap;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PartyBasedOperationSuitabilityObserver"/> class.
        /// </summary>
        /// <param name="operation">The <see cref="DataServiceOperation"/> that this observer is operating for.</param>
        /// <param name="columnMap">The <see cref="ColumnMap"/> that this observer should use when performing analysis on CSV row data.</param>
        public PartyBasedOperationSuitabilityObserver(PublicProduct operation, ColumnMap columnMap) : base(operation.Convert())
        {
            if (columnMap == null) throw new ArgumentNullException(nameof(columnMap));
            Contract.EndContractBlock();

            this.columnMap = columnMap.Fields
                    .Select((f, i) => new FieldMap { Field = f, Ordinal = i })
                    .Where(f => EvaluatedFields.Contains(f.Field))
                    .ToArray();
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override String Title => $"Party Search Evaluation for {this.Operation}";

        /// <inheritdoc />
        protected override Boolean Evaluate(String[] row)
        {
            if (row == null) return false;

            var indexedMap = this.columnMap;

            var ordinalFullName = indexedMap.FirstOrDefault(m => m.Field == FieldName.FullName.ToString()) ?? FieldMap.None;
            var ordinalFirstName = indexedMap.FirstOrDefault(m => m.Field == FieldName.FirstName.ToString()) ?? FieldMap.None;
            var ordinalLastName = indexedMap.FirstOrDefault(m => m.Field == FieldName.LastName.ToString()) ?? FieldMap.None;
            var ordinalStreetAddress = indexedMap.FirstOrDefault(m => m.Field == FieldName.StreetAddress.ToString()) ?? FieldMap.None;
            var ordinalCity = indexedMap.FirstOrDefault(m => m.Field == FieldName.City.ToString()) ?? FieldMap.None;
            var ordinalState = indexedMap.FirstOrDefault(m => m.Field == FieldName.State.ToString()) ?? FieldMap.None;
            var ordinalZip = indexedMap.FirstOrDefault(m => m.Field == FieldName.PostalCode.ToString()) ?? FieldMap.None;

            if (
                (!ordinalFirstName.Vote(row) || !ordinalLastName.Vote(row))
                &&
                !ordinalFullName.Vote(row)) return false;
            if (!ordinalStreetAddress.Vote(row)) return false;
            if (ordinalZip.Vote(row)) return true;
            if (ordinalCity.Vote(row) && ordinalState.Vote(row)) return true;

            return false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reviews the supplied <paramref name="columnMap"/> to determine if the required <see cref="FieldName"/> entries are present.
        /// </summary>
        /// <param name="columnMap">The <see cref="ColumnMap"/> to investigate.</param>
        /// <returns>True if the minimum set of fields are mapped; Otherwise false.</returns>
        public static Boolean CanAnalyze(ColumnMap columnMap)
        {
            if (columnMap == null) return false;

            if (!columnMap.Fields.Contains(FieldName.FullName.ToString()))
            {
                if (!columnMap.Fields.Contains(FieldName.FirstName.ToString()) || !columnMap.Fields.Contains(FieldName.LastName.ToString())) return false;
            }

            if (!columnMap.Fields.Contains(FieldName.StreetAddress.ToString())) return false;

            if (columnMap.Fields.Contains(FieldName.PostalCode.ToString())) return true;

            if (!columnMap.Fields.Contains(FieldName.City.ToString()) || !columnMap.Fields.Contains(FieldName.State.ToString())) return false;

            return true;
        }

        #endregion
    }
}