using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Internals;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Factories
{
    public class ComplexNodeFactory : INodeFactory
    {
        private readonly INodeFactory _innerFactory;
        private readonly Type[] _types;

        public ComplexNodeFactory(ISerializerSettings settings, Type type)
            : this(settings, new [] { type }) { }
        
        public ComplexNodeFactory(ISerializerSettings settings, IEnumerable<Type> types)
        {
            if(settings == null)
                throw new ArgumentNullException("settings");
            if(types == null)
                throw new ArgumentNullException("types");
            
            _types = types.ToArray();
            if(_types.Length == 0 || _types.Any(t => t == null))
                throw new ArgumentException("types");
            
            _innerFactory = this.CreateFactory(settings);
        }

        public ISerializerSettings Settings
        {
            get { return _innerFactory.Settings; }
        }

        public ExpressionNode Create(Expression expression)
        {
            return _innerFactory.Create(expression);
        }

        public TypeNode Create(Type type)
        {
            return _innerFactory.Create(type);
        }

        public Type ResolveTypeRef(int typeRef)
        {
            return _innerFactory.ResolveTypeRef(typeRef);
        }

        private INodeFactory CreateFactory(ISerializerSettings settings)
        {
            var expectedTypes = new HashSet<Type>();
            foreach (var type in _types)
                expectedTypes.UnionWith(GetComplexMemberTypes(type));
            return new TypeResolverNodeFactory(settings, expectedTypes);
        }

        private static IEnumerable<Type> GetComplexMemberTypes(Type type)        
        {
            var typeFinder = new ComplexPropertyMemberTypeFinder();
            return typeFinder.FindTypes(type);
        }
    }
}
