using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Internals;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Factories
{
    public class ComplexExpressionNodeFactory<T> : IExpressionNodeFactory<T>
    {
        private readonly IExpressionNodeFactory _innerFactory;

        public ComplexExpressionNodeFactory()
        {
            _innerFactory = CreateFactory();
        }

        public ExpressionNode Create(Expression expression)
        {
            return _innerFactory.Create(expression);
        }

        private static IExpressionNodeFactory CreateFactory()
        {
            var expectedTypes = new HashSet<Type>();
            expectedTypes.UnionWith(GetComplexMemberTypes(typeof(T)));
            return new TypeResolverExpressionNodeFactory(expectedTypes);
        }

        private static IEnumerable<Type> GetComplexMemberTypes(Type type)        
        {
            var typeFinder = new ComplexPropertyMemberTypeFinder();
            return typeFinder.FindTypes(type);
        }
    }
}
