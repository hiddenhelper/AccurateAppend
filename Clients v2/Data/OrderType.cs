namespace AccurateAppend.Websites.Clients.Data
{
    /// <summary>
    /// One of the possible types of Client Orders.
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// Corresponds to the <see cref="BatchOrder"/> type.
        /// </summary>
        Batch,

        /// <summary>
        /// Corresponds to the <see cref="NationBuilderOrder"/> type.
        /// </summary>
        Push,

        /// <summary>
        /// Corresponds to the <see cref="DirectClientOrder"/> type.
        /// </summary>
        Client
    }
}