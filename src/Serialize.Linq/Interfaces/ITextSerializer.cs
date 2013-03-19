namespace Serialize.Linq.Interfaces
{
    /// <summary>
    /// A text serializer interface.
    /// </summary>
    public interface ITextSerializer : ISerializer
    {
        /// <summary>
        /// Serializes the specified obj to text.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        string Serialize<T>(T obj);

        /// <summary>
        /// Deserializes a object of type T from the specified text.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        T Deserialize<T>(string text);
    }
}
