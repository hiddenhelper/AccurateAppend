using System;
using System.Linq;
using AccurateAppend.Core.Definitions;
using AccurateAppend.JobProcessing.Manifest;

namespace AccurateAppend.Websites.Clients.Areas.Order.Analyzers
{
    /// <summary>
    /// An operation suitability observer that finds suitable records in CSV data
    /// according to a single <see cref="FieldName"/> in a <see cref="ColumnMap"/>.
    /// Suitable defined as having a particular field present.
    /// </summary>
    public class UnaryBasedOperationSuitabilityObserver : OperationSuitabilityObserver
    {
        #region Fields

        private readonly FieldMap columnMap;
        private readonly FieldName fieldName;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="PartyBasedOperationSuitabilityObserver"/> class.
        /// </summary>
        /// <param name="operation">The <see cref="DataServiceOperation"/> that this observer is operating for.</param>
        /// <param name="requiredField">The <see cref="FieldName"/> that must be present to be suitable.</param>
        /// <param name="columnMap">The <see cref="ColumnMap"/> that this observer should use when performing analysis on CSV row data.</param>
        public UnaryBasedOperationSuitabilityObserver(PublicProduct operation, FieldName requiredField, ColumnMap columnMap) : base(operation.Convert())
        {
            this.columnMap = columnMap.Fields
                                 .Select((f, i) => new FieldMap {Field = f, Ordinal = i})
                                 .FirstOrDefault(f => requiredField.ToString() == f.Field) ??
                             FieldMap.None;
            this.fieldName = requiredField;
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override String Title => $"Unary Search Evaluation on {this.fieldName} for {this.Operation}";

        /// <inheritdoc />
        protected override Boolean Evaluate(String[] row)
        {
            return this.columnMap.Vote(row);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reviews the supplied <paramref name="columnMap"/> to determine if at least one <see cref="FieldName"/> entry is present.
        /// </summary>
        /// <param name="columnMap">The <see cref="ColumnMap"/> to investigate.</param>
        /// <returns>True if the minimum set of fields are mapped; Otherwise false.</returns>
        public static Boolean CanAnalyze(ColumnMap columnMap, OperationDefinition operation)
        {
            if (operation.InputFields.Where(f => f.Required).Distinct().Count() > 1) return false;

            var requiredFields = operation.InputFields.Where(f => f.Required).Select(f => f.OperationParamName).Distinct().ToArray();
            return requiredFields.All(f => columnMap.Fields.Contains(f));
        }

        #endregion
    }
}