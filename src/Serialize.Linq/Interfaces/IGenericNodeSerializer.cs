using System.Linq.Expressions;
using Serialize.Linq.Factories;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Interfaces
{
    public interface IGenericNodeSerializer<TSerialize> : ISerializer
    {
        /// <summary>
        /// Serializes the specified object inheriting from <see cref="Node"/> to an array of bytes or to a string depending on <typeparamref name="TSerialize"/>.
        /// </summary>
        /// <typeparam name="TNode">The specific type inheriting from <see cref="Node"/>.</typeparam>
        /// <param name="obj">The object inheriting from <see cref="Node"/> to be serialized.</param>
        /// <returns>Depending on <typeparamref name="TSerialize"/>: an array of bytes or a string with the serialized <paramref name="obj"/>.</returns>
        TSerialize Serialize<TNode>(TNode obj) where TNode : Node;

        /// <summary>
        /// Deserializes the specified array of bytes or the specified string, depending on <typeparamref name="TSerialize"/>, to a <see cref="Node"/> object.
        /// </summary>
        /// <typeparam name="TNode">The specific type inheriting from <see cref="Node"/>.</typeparam>
        /// <param name="data">Depending on <typeparamref name="TSerialize"/>: an array of bytes or a string with the serialized <see cref="Node"/> object.</param>
        /// <returns>The deserialized object, inheriting from <see cref="Node"/></returns>
        TNode Deserialize<TNode>(TSerialize data) where TNode : Node;
    }
}
