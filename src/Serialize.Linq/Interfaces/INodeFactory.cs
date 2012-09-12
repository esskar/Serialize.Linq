using System;
using System.Linq.Expressions;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Interfaces
{
    public interface INodeFactory
    {
        bool UseAssemblyQualifiedName { get; set; }

        bool UseReferences { get; set; }

        ExpressionNode Create(Expression expression);

        TypeNode Create(Type type);

        Type ResolveTypeRef(int typeRef);
    }    
}