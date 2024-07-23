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
    public class CreateNationBuilderDealCommand : CreateJobDealCommand
    {
    }
}