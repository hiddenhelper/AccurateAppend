using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Databases.ProspectStore;

namespace DomainModel.ReadModel.Prospector
{
    /// <summary>
    /// View model used to organize and display Prospector data for one column in a file
    /// </summary>
    public class ColumnViewModel
    {
        #region Properties

        /// <summary>
        /// Column index
        /// </summary>
        public Int32 ColumnIndex { get; }

        /// <summary>
        /// Type of field
        /// </summary>
        public string FieldName { get; }

        /// <summary>
        /// Minimum value length.
        /// </summary>
        public Int64 MinLength { get; }

        /// <summary>
        /// Maximum value length
        /// </summary>
        public Int64 MaxLength { get; }

        /// <summary>
        /// Total records in column.
        /// </summary>
        public Int64 TotalRecords { get; }

        /// <summary>
        /// Total number of records that are not blank.
        /// </summary>
        public Int64 FilledRecords { get; }

        /// <summary>
        /// Cardinality of values in column.
        /// </summary>
        public Int64 Cardinality { get; }

        /// <summary>
        /// Distinct values expressed as a whole number percentage
        /// </summary>
        public Int64 DistinctPct { get; }

        /// <summary>
        /// First most common value in column, excluding blank values
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public CommonValueModel Commonest_1 { get; }

        /// <summary>
        /// First most common value in column, excluding blank values expressed as a percentage
        /// </summary>
        /// ReSharper disable once InconsistentNaming
        public CommonValueModel CommonestPct_1 { get; }

        /// <summary>
        /// First most common value in column, excluding blank values
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public CommonValueModel Commonest_2 { get; }

        /// <summary>
        /// Second most common value in column, excluding blank values expressed as a percentage
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public CommonValueModel CommonestPct_2 { get; }

        /// <summary>
        /// Third most common value in column, excluding blank values
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public CommonValueModel Commonest_3 { get; }

        /// <summary>
        /// Third most common value in column, excluding blank values expressed as a percentage
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public CommonValueModel CommonestPct_3 { get; }

        /// <summary>
        /// Fourth most common value in column, excluding blank values
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public CommonValueModel Commonest_4 { get; }

        /// <summary>
        /// Fourth most common value in column, excluding blank values expressed as a percentage
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public CommonValueModel CommonestPct_4 { get; }

        /// <summary>
        /// Fifth most common value in column, excluding blank values
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public CommonValueModel Commonest_5 { get; }

        /// <summary>
        /// Fifth most common value in column, excluding blank values expressed as a percentage
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public CommonValueModel CommonestPct_5 { get; }

        /// <summary>
        /// The number of fields in this column with values not accounted for by Commonest_1 thru 5
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public Int64 Commonest_Other { get; }

        /// <summary>
        /// The number of fields in this column with values not accounted for by Commonest_1 thru 5 expressed as a percentage
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public Int64 CommonestPct_Other { get; }

        /// <summary>
        /// Count of blank columns
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public Int64 Commonest_Blank { get; }

        /// <summary>
        /// Count of blank columns expressed as a percentage
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public Int64 CommonestPct_Blank { get; }

        /// <summary>
        /// All facts for a given a column
        /// </summary>
        public List<FactViewModel> ColumnFacts { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnViewModel"/> class.
        /// </summary>
        public ColumnViewModel(IEnumerable<Fact> facts)
        {
            var f = facts.ToArray();
            var firstFact = f.First();
            this.ColumnIndex = firstFact.ColumnIndex;

            this.FieldName = firstFact.ColumnName;
            this.MinLength = f.First(a => a.FactType == "MinLength").Value;
            this.MaxLength = f.First(a => a.FactType == "MaxLength").Value;
            this.TotalRecords = f.First(a => a.FactType == "TotalRecords").Value;
            this.FilledRecords = f.First(a => a.FactType == "FilledRecords").Value;
            this.Cardinality = f.First(a => a.FactType == "Cardinality").Value;
            this.DistinctPct = f.First(a => a.FactType == "DistinctPct").Value;

            this.Commonest_1 = LoadCommonest(f, "Commonest:_1");
            this.CommonestPct_1 = LoadCommonest(f, "CommonestPct:_1");
            this.Commonest_2 = LoadCommonest(f, "Commonest:_2");
            this.CommonestPct_2 = LoadCommonest(f, "CommonestPct:_2");
            this.Commonest_3 = LoadCommonest(f, "Commonest:_3");
            this.CommonestPct_3 = LoadCommonest(f, "CommonestPct:_3");
            this.Commonest_4 = LoadCommonest(f, "Commonest:_4");
            this.CommonestPct_4 = LoadCommonest(f, "CommonestPct:_4");
            this.Commonest_5 = LoadCommonest(f, "Commonest:_5");
            this.CommonestPct_5 = LoadCommonest(f, "CommonestPct:_5");
            this.Commonest_Other = f.First(a => a.FactType == "Commonest:_Other").Value;
            this.CommonestPct_Other = f.First(a => a.FactType == "CommonestPct:_Other").Value;
            this.Commonest_Blank = f.First(a => a.FactType == "Commonest:_Blank").Value;
            this.CommonestPct_Blank = f.First(a => a.FactType == "CommonestPct:_Blank").Value;

            this.ColumnFacts = f.Where(a => a.ColumnIndex == ColumnIndex).Select(a => new FactViewModel(a)).ToList();
        }

        #endregion

        #region Helpers

        private static CommonValueModel LoadCommonest(IEnumerable<Fact> f, String factType)
        {
            return f.Where(a => a.FactType.StartsWith(factType))
                       .Select(a => new CommonValueModel(a))
                       .FirstOrDefault()
                   ?? CommonValueModel.Empty;
        }

        #endregion
    }
}