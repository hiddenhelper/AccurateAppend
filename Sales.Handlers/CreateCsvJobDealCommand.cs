using System;

namespace AccurateAppend.Sales.Handlers
{
    /// <summary>
    /// Command used to construct/update a deal for a given public key, if not already completed.
    /// </summary>
    /// <remarks>
    /// Used internally to keep the complexity under wraps.
    /// </remarks>
    [Serializable()]
    public class CreateCsvJobDealCommand : CreateJobDealCommand
    {
        /// <summary>
        /// The name of the completed file that a deal is being generated for.
        /// </summary>
        public String CustomerFileName { get; set; }
    }
}