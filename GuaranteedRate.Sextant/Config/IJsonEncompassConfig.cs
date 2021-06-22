
namespace GuaranteedRate.Sextant.Config
{
    public interface IJsonEncompassConfig : IEncompassConfig
    {
        /// <summary>
        /// Get a templated value, based on a key and default value if the value is invalid or empty.
        /// </summary>
        /// <typeparam name="T">Class or Interface</typeparam>
        /// <param name="key">Key name</param>
        /// <param name="defaultValue">Default value if the value is invalid or empty.</param>
        /// <returns></returns>
        T GetValue<T>(string key, T defaultValue = default(T));

        /// <summary>
        /// This takes a type of T, a key and an optional default value.
        /// If the json value assigned to the key is an empty string it 
        /// will return a null object or an empty string depending on the 
        /// type of T
        /// </summary>
        /// <typeparam name="T">Object type to be returned</typeparam>
        /// <param name="key">Full path to the key(e.g. "Shape.Square.Color")</param>
        /// <param name="defaultValue">The value to be returned if the requested value is not found</param>
        /// <param name="orgId">The organization ID to pull from, if passed</param>
        /// <returns>A value of the requested type</returns>
        T GetValue<T>(string key, T defaultValue, string orgId);
    }
}
