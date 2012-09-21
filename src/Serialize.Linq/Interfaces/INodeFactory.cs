using System;
using System.Linq.Expressions;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Interfaces
{
    public interface INodeFactory
    {
        ExpressionNode Create(Expression expression);

        TypeNode Create(Type type);
    }    
}