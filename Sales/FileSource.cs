namespace AccurateAppend.Sales
{
    /// <summary>
    /// Indicates a specific source channel for a file that a <see cref="ProductOrder"/> entity was created from.
    /// </summary>
    /// <remarks>
    /// The individual values purposely match those of the Job Source enumeration to reduce cognitive load.
    /// All completed carts source exactly one type of file, one file source may be used for multiple types of carts.
    /// </remarks>
    public enum FileSource
    {
        /// <summary>
        /// A NationBuilder list file.
        /// </summary>
        NationBuilder = 1,

        /// <summary>
        /// A CSV uploaded client file.
        /// </summary>
        ClientFile = 5,

        /// <summary>
        /// A List Builder generated enhanced file.
        /// </summary>
        ListBuilder = 6
    }
}
