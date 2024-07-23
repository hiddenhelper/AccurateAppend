using System.Text.RegularExpressions;
using DAL.Databases.ProspectStore;

namespace DomainModel.ReadModel.Prospector
{
    /// <summary>
    /// View model used to organize and display Prospector data for one column in a file
    /// </summary>
    public class CommonValueModel : FactViewModel
    {
        /// <summary>
        /// Holds the "Empty" value fact model.
        /// </summary>
        public static readonly CommonValueModel Empty = new CommonValueModel();

        /// <summary>
        /// Private constructor.
        /// </summary>
        private CommonValueModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonValueModel"/> class.
        /// </summary>
        public CommonValueModel(Fact fact) : base(fact)
        {
            // pulls string value out of FactType
            //  Commonest:_5 "SMITHFIELD"
            this.FactType = Regex.Match(fact.FactType, "\"([^\"]*)\"").Groups[0].Value.Replace("\"", "");
            this.Count = fact.Value;
        }
    }
}