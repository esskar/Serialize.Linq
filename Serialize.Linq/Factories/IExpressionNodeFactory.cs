using System.Linq.Expressions;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Factories
{
    public interface IExpressionNodeFactory
    {
        ExpressionNode Create(Expression expression);
    }

    public interface IExpressionNodeFactory<T> : IExpressionNodeFactory { }
}
