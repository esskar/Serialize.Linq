#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using Serialize.Linq.Nodes;

namespace Serialize.Linq.Interfaces
{
    public interface IBinarySerializer : ISerializer
    {
        /// <summary>
        /// Serializes the specified obj to an array of bytes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        byte[] Serialize<T>(T obj) where T : Node;

        /// <summary>
        /// Deserializes the specified bytes to an object of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        T Deserialize<T>(byte[] bytes) where T : Node;
    }
}
