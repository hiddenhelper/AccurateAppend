using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DAL.Databases.ProspectStore;

namespace DomainModel.ReadModel.Prospector
{
    /// <summary>
    /// View model used to organize and display Prospector data for a file
    /// </summary>
    public class ProspectorViewModel
    {
        #region Proeprties

        /// <summary>
        /// Id of Job
        /// </summary>
        public int JobId { get; set; }
        /// <summary>
        /// Facts that describe the entire file
        /// </summary>
        public IList<FactViewModel> FileStats { get; }

        /// <summary>
        /// View models that describe each column
        /// </summary>
        public IList<ColumnViewModel> Columns { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="facts"></param>
        public ProspectorViewModel(IEnumerable<Fact> facts)
        {
            var f = facts.ToArray();
            this.FileStats = f.Where(a => a.ColumnIndex == -1).Select(a => new FactViewModel(a)).ToImmutableArray();

            this.Columns = new List<ColumnViewModel>();
            foreach (var i in f.Select(a => a.ColumnIndex).Where(a => a > -1).Distinct())
            {
                Columns.Add(new ColumnViewModel(f.Where(a => a.ColumnIndex == i)));
            }
            this.Columns = this.Columns.ToImmutableArray();
        }

        #endregion
    }
}