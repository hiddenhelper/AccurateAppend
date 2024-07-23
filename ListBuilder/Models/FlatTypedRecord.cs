using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AccurateAppend.Core.Collections.Generic;
using DAL.Typed;

namespace AccurateAppend.ListBuilder.Models
{
    /// <summary>
    /// A typed record without normalization. 
    /// </summary>
    /// <remarks>
    /// Still a collection of QualifiedType-to-Datums, but each qualified type stores one datum.
    /// </remarks>
    public class FlatTypedRecord
    {
        public ImmutableDictionary<QualifiedType, Datum> Data;

        public FlatTypedRecord()
        {
            this.Data = Enumerable.Empty<KeyValuePair<QualifiedType, Datum>>().ToImmutableDictionary();
        }

        public FlatTypedRecord(ImmutableDictionary<QualifiedType, Datum> data)
        {
            this.Data = data;
        }

        public FlatTypedRecord Mutate(QualifiedType qt, Datum datum)
        {
            return new FlatTypedRecord(this.Data.SetItem(qt, datum));
        }

        public FlatTypedRecord Mutate(QualifiedType qt, DAL.Typed.DataTypes.DataType dataType)
        {
            return new FlatTypedRecord(this.Data.SetItem(qt, new Datum(dataType)));
        }

        public override String ToString()
        {
            return String.Join(",", this.Data.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        }

        public IEnumerable<QualifiedType> Keys => this.Data.Keys;

        /// <remarks>
        /// set using indexer implies modifying self, which is incompatible with the immutable paradigm. Instead:  ftr=ftr.mutate(QualifiedType.Address,newValue)
        /// </remarks>
        public Datum this[QualifiedType qt] // throws KeyNotFoundException
            => this.Data[qt];

        public Datum GetOrDefault(QualifiedType qt)
        {
            return this.Data.GetOrDefault(qt);
        }

        /// <summary>
        /// Doesn't include the metadata.
        /// </summary>
        /// <returns>Returns contents without metadata</returns>
        public String ToStringDataOnly()
        {
            //return String.Join("\n", this._data.Select(kvp => $"{kvp.Key}={kvp.Value.Value}"));
            return String.Join(",", this.Data.Select(kvp => $"{kvp.Key}={kvp.Value.Value}"));
        }
    }
}