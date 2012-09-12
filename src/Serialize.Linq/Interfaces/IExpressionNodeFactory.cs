using System.Linq.Expressions;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Interfaces
{
    public interface IExpressionNodeFactory
    {
        bool UseAssemblyQualifiedName { get; set; }

        ExpressionNode Create(Expression expression);
    }    
}
