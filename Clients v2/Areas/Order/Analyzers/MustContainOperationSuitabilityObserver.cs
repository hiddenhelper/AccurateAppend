using System;
using System.Linq;
using AccurateAppend.JobProcessing.Manifest;

namespace AccurateAppend.Websites.Clients.Areas.Order.Analyzers
{
    public class MustContainOperationSuitabilityObserver : OperationSuitabilityObserver
    {
        #region Fields

        private readonly FieldMap[] columnMap;

        #endregion

        #region Constructor

        public MustContainOperationSuitabilityObserver(OperationDefinition operation, ColumnMap columnMap) : base(operation.Name)
        {
            if (operation == null) throw new ArgumentNullException(nameof(operation));
            var requiredFields = operation.InputFields.Where(f => f.Required).Select(f => f.OperationParamName).Distinct().ToArray();
            
            this.columnMap = columnMap.Fields
                    .Select((f, i) => new FieldMap { Field = f, Ordinal = i })
                    .Where(f => requiredFields.Contains(f.Field))
                    .ToArray();
        }

        #endregion

        #region Overrides

        protected override String Title => $"Unary Search Evaluation for {this.Operation}";

        protected override Boolean Evaluate(String[] row)
        {
            if (row == null || row.Length < this.columnMap.Max(o => o.Ordinal)) return false;

            return this.columnMap.All(fieldMap => fieldMap.Vote(row));
        }

        #endregion

        #region Methods

        public static Boolean CanAnalyze(ColumnMap columnMap, OperationDefinition operation)
        {
            if (columnMap == null || operation == null) return false;

            var requiredFields = operation.InputFields.Where(f => f.Required).Select(f => f.OperationParamName).Distinct().ToArray();
            return requiredFields.All(f => columnMap.Fields.Contains(f));
        }

        #endregion
    }
}