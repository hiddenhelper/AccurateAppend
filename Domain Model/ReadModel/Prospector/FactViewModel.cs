using System;
using DAL.Databases.ProspectStore;

namespace DomainModel.ReadModel.Prospector
{
    /// <summary>
    /// View model used to organize and display Prospector data for one column in a file
    /// </summary>
    public class FactViewModel
    {
        /// <summary>
        /// Initializes a new empty instance of the <see cref="FactViewModel"/> class.
        /// </summary>
        protected FactViewModel()
        {
            this.FactType = String.Empty;
            this.Count = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FactViewModel"/> class.
        /// </summary>
        public FactViewModel(Fact fact)
        {
            this.FactType = fact.FactType;
            this.Count = fact.Value;
        }

        /// <summary>
        /// Name of fact.
        /// </summary>
        public String FactType { get; protected set; }

        /// <summary>
        /// Count of fact values.
        /// </summary>
        public Int64 Count { get; protected set; }
    }
}