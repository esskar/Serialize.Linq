using System.IO;

namespace Serialize.Linq.Interfaces
{
    public interface ISerializer
    {
        /// <summary>
        /// Serializes the specified object to the specified stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">The stream.</param>
        /// <param name="obj">The obj.</param>
        void Serialize<T>(Stream stream, T obj);

        /// <summary>
        /// Deserializes an object of type T from the specified stream.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        T Deserialize<T>(Stream stream);
    }
}
