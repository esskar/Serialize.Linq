using System.Linq.Expressions;
using Serialize.Linq.Factories;

namespace Serialize.Linq.Interfaces
{
    public interface IGenericExpressionSerializer<TSerialize>
    {
        /// <summary>
        /// Serializes the specified <see cref="Expression"/> to an array of bytes or to a string depending on <typeparamref name="TSerialize"/>.
        /// </summary>
        /// <param name="expression">The object inheriting from <see cref="Expression"/> to be serialized.</param>
        /// <returns>Depending on <typeparamref name="TSerialize"/>: an array of bytes or a string with the serialized <paramref name="expression"/>.</returns>
        TSerialize SerializeGeneric(Expression expression, FactorySettings factorySettings = null);

        /// <summary>
        /// Deserializes the specified array of bytes or the specified string, depending on <typeparamref name="TSerialize"/>, to a <see cref="Expression"/> object.
        /// </summary>
        /// <param name="data">Depending on <typeparamref name="TSerialize"/>: an array of bytes or a string with the serialized <see cref="Expression"/> object.</param>
        /// <returns>The deserialized object, inheriting from <see cref="Expression"/></returns>
        Expression DeserializeGeneric(TSerialize data, IExpressionContext context = null);
    }
}
