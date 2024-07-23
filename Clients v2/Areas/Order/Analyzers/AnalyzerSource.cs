using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccurateAppend.Core.ComponentModel;
using AccurateAppend.Core.Evaluations;
using AccurateAppend.Core.Utilities;

namespace AccurateAppend.Websites.Clients.Areas.Order.Analyzers
{
    /// <summary>
    /// Works as an aggregate to individual observers bridging the CSV row data derived from a <see cref="CsvFileContent"/> source.
    /// </summary>
    /// <remarks>
    /// Allows the actual observer logic to be as simple as possible by hiding the complexities of accessing the CSV
    /// rows data and then supplying it to the contained types. Provides a strong boundary between the source and the analysis
    /// logic by interceding as an <see cref="IObservable{T}"/>.
    /// </remarks>
    public class AnalyzerSource : IObservable<String[]>
    {
        #region Fields

        private readonly IList<IObserver<String[]>> observers = new List<IObserver<String[]>>();
        private readonly IRule<String[], Boolean> sampler = True<String[]>.Instance;

        #endregion

        #region Methods

        /// <summary>
        /// Instructs all the contained analyzers to inspect each CSV content row.
        /// </summary>
        /// <param name="csvFile">The source of the CSV formatted data. This file must be formatted and configured correctly prior to calling this method.</param>
        public async Task Execute(CsvFileContent csvFile)
        {
            foreach (var taskToReadLine in csvFile.ReadRecords(true))
            {
                var row = await taskToReadLine.ConfigureAwait(false);

                if (this.sampler.Evaluate(row)) this.Push(row);
            }

            this.Complete();
        }

        /// <summary>
        /// Pushes the content to each <see cref="IObserver{T}"/> contained in this aggregate.
        /// </summary>
        /// <param name="value">The CSV row data formatted as a <see cref="String"/> array.</param>
        protected virtual void Push(String[] value)
        {
            foreach (var observer in this.observers)
            {
                observer.OnNext(value);
            }
        }

        /// <summary>
        /// Cascades the <see cref="IObserver{T}.OnCompleted"/> notification to each <see cref="IObserver{T}"/> contained in this aggregate.
        /// </summary>
        protected virtual void Complete()
        {
            foreach (var observer in this.observers)
            {
                observer.OnCompleted();
            }
        }

        #endregion

        #region IObservable<out String[]> Members

        /// <inheritdoc />
        public IDisposable Subscribe(IObserver<String[]> observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));

            this.observers.Add(observer);

            return new Unsubscriber(this.observers, observer);
        }

        #endregion

        #region Nested Types

        protected class Unsubscriber : DisposeableObject
        {
            private readonly IList<IObserver<String[]>> observers;
            private readonly IObserver<String[]> observer;

            public Unsubscriber(IList<IObserver<String[]>> observers, IObserver<String[]> observer)
            {
                if (observers == null) throw new ArgumentNullException(nameof(observers));
                if (observers.IsReadOnly) throw new ArgumentException("List cannot be read only", nameof(observers));
                if (observer == null) throw new ArgumentNullException(nameof(observer));

                this.observers = observers;
                this.observer = observer;
            }

            #region Overrides

            protected override void Dispose(Boolean isDisposing)
            {
                base.Dispose(isDisposing);
                if (!isDisposing) return;

                if (this.observer != null && this.observers.Contains(this.observer)) this.observers.Remove(this.observer);
            }

            #endregion
        }

        #endregion
    }
}