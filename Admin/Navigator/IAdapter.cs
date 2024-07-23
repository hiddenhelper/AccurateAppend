namespace AccurateAppend.Websites.Admin.Navigator
{
    /// <summary>
    /// Interface to provide an anti-corruption layer on the Navigator system. 
    /// </summary>
    /// <typeparam name="T">The type to adapt to.</typeparam>
    internal interface IAdapter<out T>
    {
        /// <summary>
        /// Gets the <typeparamref name="T"/> instance associated with this instance.
        /// </summary>
        T Item { get; }
    }
}