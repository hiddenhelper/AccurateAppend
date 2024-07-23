using System.Collections.Generic;
using AccurateAppend.ListBuilder.Models;
using DAL.Typed;

namespace AccurateAppend.ListBuilder.DataSources
{
    internal static class Utilities
    {
        /// <summary>
        /// Denormalizes a TypedRecord into multilpe FlatTypedRecords
        /// </summary>
        /// <remarks>
        /// This function generates the cartesian product of all the values in the typed record, which sounds terrible, but it's generally not because the count 
        /// ends up being however many the database returned before they were merged. There are exceptions but they are rare.
        /// </remarks>
        /// <param name="tr">The record to be converted</param>
        /// <returns>One or more FlatTypedRecords</returns>
        public static IEnumerable<FlatTypedRecord> Denormalize(this TypedRecord tr)
        {
            IEnumerable<FlatTypedRecord> denorms = new[] { new FlatTypedRecord() }; // start with a single blank record

            foreach (var kvp in tr)
            {
                denorms = DenormalizeWorker(denorms, kvp.Key, kvp.Value);
            }

            return denorms;
        }

        private static IEnumerable<FlatTypedRecord> DenormalizeWorker(IEnumerable<FlatTypedRecord> set, QualifiedType qt, DatumList dl)
        {
            foreach (var ftr in set)
            {
                foreach (var datum in dl)
                {
                    yield return ftr.Mutate(qt, datum);
                }
            }
        }
    }
}
