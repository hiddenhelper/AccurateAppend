// ReSharper disable once CheckNamespace
namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Performs validation that a target is greater or equal to 0.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    [Serializable()]
    public sealed class RequiredPositiveAttribute : ValidationAttribute
    {
        /// <inheritdoc />
        /// <remarks>
        /// Supports numeric <paramref name="value"/> inputs only.
        /// </remarks>
        public override Boolean IsValid(Object value)
        {
            if (value == null) return false;
            if (value is Byte) return true;
            if (value is UInt16) return true;
            if (value is UInt32) return true;
            if (value is UInt64) return true;

            if (value is Int16) return this.IsValid((Int16)value);
            if (value is Int32) return this.IsValid((Int32)value);
            if (value is Int64) return this.IsValid((Int64)value);
            if (value is Single) return this.IsValid((Single)value);
            if (value is Double) return this.IsValid((Double)value);
            if (value is Decimal) return this.IsValid((Decimal)value);

            // anything else is not valid
            return false;
        }

        private Boolean IsValid(Int16 value)
        {
            Int16 i;
            return Int16.TryParse(value.ToString(), out i) && i >= 0;
        }

        private Boolean IsValid(Int32 value)
        {
            Int32 i;
            return Int32.TryParse(value.ToString(), out i) && i >= 0;
        }

        private Boolean IsValid(Int64 value)
        {
            Int64 i;
            return Int64.TryParse(value.ToString(), out i) && i >= 0;
        }

        private Boolean IsValid(Single value)
        {
            Single i;
            return Single.TryParse(value.ToString(), out i) && i >= 0f;
        }

        private Boolean IsValid(Double value)
        {
            Double i;
            return Double.TryParse(value.ToString(), out i) && i >= 0d;
        }

        private Boolean IsValid(Decimal value)
        {
            Decimal i;
            return Decimal.TryParse(value.ToString(), out i) && i >= 0m;
        }
    }

    /// <summary>
    /// Performs validation that a target is greater than 0.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    [Serializable()]
    public sealed class RequiredGreaterThanZeroAttribute : ValidationAttribute
    {
        /// <inheritdoc />
        /// <remarks>
        /// Supports numeric <paramref name="value"/> inputs only.
        /// </remarks>
        public override Boolean IsValid(Object value)
        {
            if (value == null) return false;
            if (value is Byte) return true;
            if (value is UInt16) return true;
            if (value is UInt32) return true;
            if (value is UInt64) return true;

            if (value is Int16) return this.IsValid((Int16)value);
            if (value is Int32) return this.IsValid((Int32)value);
            if (value is Int64) return this.IsValid((Int64)value);
            if (value is Single) return this.IsValid((Single)value);
            if (value is Double) return this.IsValid((Double)value);
            if (value is Decimal) return this.IsValid((Decimal)value);

            // anything else is not valid
            return false;
        }

        private Boolean IsValid(Int16 value)
        {
            Int16 i;
            return Int16.TryParse(value.ToString(), out i) && i > 0;
        }

        private Boolean IsValid(Int32 value)
        {
            Int32 i;
            return Int32.TryParse(value.ToString(), out i) && i > 0;
        }

        private Boolean IsValid(Int64 value)
        {
            Int64 i;
            return Int64.TryParse(value.ToString(), out i) && i > 0;
        }

        private Boolean IsValid(Single value)
        {
            Single i;
            return Single.TryParse(value.ToString(), out i) && i > 0f;
        }

        private Boolean IsValid(Double value)
        {
            Double i;
            return Double.TryParse(value.ToString(), out i) && i > 0d;
        }

        private Boolean IsValid(Decimal value)
        {
            Decimal i;
            return Decimal.TryParse(value.ToString(), out i) && i > 0m;
        }
    }

    /// <summary>
    /// Performs validation that a target is not an empty <see cref="Guid"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    [Serializable()]
    public sealed class RequiredNotEmptyGuid : ValidationAttribute
    {
        /// <inheritdoc />
        /// <remarks>
        /// Supports guid <paramref name="value"/> inputs only.
        /// </remarks>
        public override Boolean IsValid(Object value)
        {
            if (value == null) return false;

            Guid result;
            return Guid.TryParse(value.ToString(), out result);
        }
    }
}
 