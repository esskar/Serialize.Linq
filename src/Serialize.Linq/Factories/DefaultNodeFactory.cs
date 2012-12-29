using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Internals;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Factories
{
    public class DefaultNodeFactory : INodeFactory
    {
        private readonly INodeFactory _innerFactory;
        private readonly Type[] _types;

        public DefaultNodeFactory(Type type)
            : this(new [] { type }) { }
        
        public DefaultNodeFactory(IEnumerable<Type> types)
        {
            if(types == null)
                throw new ArgumentNullException("types");
            
            _types = types.ToArray();
            if(_types.Any(t => t == null))
                throw new ArgumentException("types");
            _innerFactory = this.CreateFactory();
        }
        
        public ExpressionNode Create(Expression expression)
        {
            return _innerFactory.Create(expression);
        }

        public TypeNode Create(Type type)
        {
            return _innerFactory.Create(type);
        }

        private INodeFactory CreateFactory()
        {
            var expectedTypes = new HashSet<Type>();
            foreach (var type in _types)
                expectedTypes.UnionWith(GetComplexMemberTypes(type));
            return new TypeResolverNodeFactory(expectedTypes);
        }

        private static IEnumerable<Type> GetComplexMemberTypes(Type type)        
        {
            var typeFinder = new ComplexPropertyMemberTypeFinder();
            return typeFinder.FindTypes(type);
        }
    }
}
