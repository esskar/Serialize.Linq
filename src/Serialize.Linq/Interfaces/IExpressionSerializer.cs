using Serialize.Linq.Factories;
using System.IO;
using System.Linq.Expressions;

namespace Serialize.Linq.Interfaces
{
    public interface IExpressionSerializer
    {
        void Serialize(Stream stream, Expression expression, FactorySettings factorySettings = null);

        Expression Deserialize(Stream stream, IExpressionContext context = null);
    }
}
