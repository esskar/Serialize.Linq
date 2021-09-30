using System.Linq.Expressions;
using Serialize.Linq.Factories;

namespace Serialize.Linq.Interfaces
{
    public interface ITypeSerializer
    {
        bool CanSerializeBinary { get; }

        bool CanSerializeText { get; }

        string SerializeText(Expression expression, FactorySettings factorySettings = null);

        Expression DeserializeText(string text, IExpressionContext context = null);

        byte[] SerializeBinary(Expression expression, FactorySettings factorySettings = null);

        Expression DeserializeBinary(byte[] bytes, IExpressionContext context = null);
    }
}
