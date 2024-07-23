using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using AccurateAppend.Core.Definitions;

namespace AccurateAppend.Websites.Clients.Areas.Order.Analyzers
{
    /// <summary>
    /// Base type for an <see cref="IObserver{T}"/> that is designed to operate on a single CSV row data
    /// (represented as a <see cref="String"/> array).
    /// </summary>
    [DebuggerDisplay("{" + nameof(Title) + "}")]
    public abstract partial class OperationSuitabilityObserver : IObserver<String[]>
    {
        #region Fields

        private Int32 suitableRows;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationSuitabilityObserver"/> class.
        /// </summary>
        /// <param name="operation">The <see cref="DataServiceOperation"/> that this observer is operating for.</param>
        protected OperationSuitabilityObserver(DataServiceOperation operation)
        {
            if (operation == DataServiceOperation.UNKNOWN) throw new ArgumentException($"{nameof(operation)} cannot be {DataServiceOperation.UNKNOWN}", nameof(operation));

            this.Operation = operation;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="DataServiceOperation"/> that this observer is operating for.
        /// </summary>
        public DataServiceOperation Operation { get; }

        /// <summary>
        /// Gets a human readable description of the current observer.
        /// </summary>
        protected abstract String Title { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Method that performs the actual analysis of the provided row.
        /// </summary>
        /// <param name="row">The CSV row content as a <see cref="String"/> array.</param>
        /// <returns>True if the row is suitable for the provided operation; Otherwise false.</returns>
        protected abstract Boolean Evaluate(String[] row);

        /// <summary>
        /// Calculates the total number of suitable rows observed.
        /// </summary>
        /// <returns>The number of suitable rows observed.</returns>
        public Int32 Matches()
        {
            Contract.Ensures(Contract.Result<Int32>() >= 0);

            return this.suitableRows;
        }
        
        #endregion

        #region IObserver<in String[]> Members

        /// <inheritdoc />
        public virtual void OnNext(String[] value)
        {
            var result = this.Evaluate(value);

            Console.WriteLine($"{this.Title} votes: {result}");

            if (result) this.suitableRows = this.suitableRows + 1;
        }

        /// <inheritdoc />
        /// <remarks>
        /// The default implementation is a no-op.
        /// </remarks>
        public virtual void OnError(Exception error)
        {
        }

        /// <inheritdoc />
        public virtual void OnCompleted()
        {
        }

        #endregion
    }
}