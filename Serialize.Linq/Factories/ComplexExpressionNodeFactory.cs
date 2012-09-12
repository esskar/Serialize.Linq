using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Internals;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Factories
{
    public class ComplexExpressionNodeFactory : IExpressionNodeFactory
    {
        private readonly IExpressionNodeFactory _innerFactory;
        private readonly Type[] _types;

        public ComplexExpressionNodeFactory(Type type)
            : this(new [] { type }) { }
        
        public ComplexExpressionNodeFactory(Type[] types)
        {
            if(types == null)
                throw new ArgumentNullException("types");
            if(types.Length == 0 || types.Any(t => t == null))
                throw new ArgumentException("types");

            _types = types;
            _innerFactory = CreateFactory();
        }

        public ExpressionNode Create(Expression expression)
        {
            return _innerFactory.Create(expression);
        }

        private IExpressionNodeFactory CreateFactory()
        {
            var expectedTypes = new HashSet<Type>();
            foreach (var type in _types)
                expectedTypes.UnionWith(GetComplexMemberTypes(type));
            return new TypeResolverExpressionNodeFactory(expectedTypes);
        }

        private static IEnumerable<Type> GetComplexMemberTypes(Type type)        
        {
            var typeFinder = new ComplexPropertyMemberTypeFinder();
            return typeFinder.FindTypes(type);
        }
    }
}
