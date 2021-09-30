using System;
using System.Linq.Expressions;
using System.Reflection;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Interfaces
{
    public interface IExpressionContext
    {
        BindingFlags BindingFlags { get; }

        BindingFlags? GetBindingFlags();

        ParameterExpression GetParameterExpression(ParameterExpressionNode node);

        Type ResolveType(TypeNode node);
    }
}
