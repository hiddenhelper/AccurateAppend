using System;

namespace AccurateAppend.Websites.Clients.Areas.Order.Analyzers
{
    public abstract partial class OperationSuitabilityObserver
    {
        protected class FieldMap
        {
            #region Fields

            private readonly Func<String[], Boolean> evaluator;
            public static readonly FieldMap None = new FieldMap((row) => false);

            #endregion

            #region Constructors

            public FieldMap()
            {
                this.evaluator = this.IsPresentAndNotEmpty;
            }

            protected FieldMap(Func<String[], Boolean> evaluator)
            {
                this.evaluator = evaluator;
            }

            #endregion

            #region Properties

            public String Field { get; set; }

            public Int32 Ordinal { get; set; }

            #endregion

            #region Methods

            public Boolean Vote(String[] row)
            {
                try
                {
                    var r = this.evaluator(row);
                    return r;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            }

            protected Boolean IsPresentAndNotEmpty(String[] row)
            {
                if (row == null || row.Length <= this.Ordinal) return false;

                return !String.IsNullOrWhiteSpace(row[this.Ordinal]);
            }

            #endregion
        }
    }
}