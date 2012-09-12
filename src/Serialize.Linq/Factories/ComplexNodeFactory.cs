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

        public ComplexNodeFactory(Type type)
            : this(new [] { type }) { }
        
        public ComplexNodeFactory(Type[] types)
        {
            if(types == null)
                throw new ArgumentNullException("types");
            if(types.Length == 0 || types.Any(t => t == null))
                throw new ArgumentException("types");

            _types = types;
            _innerFactory = CreateFactory();
        }

        public bool UseAssemblyQualifiedName
        {
            get { return _innerFactory.UseAssemblyQualifiedName; }
            set { _innerFactory.UseAssemblyQualifiedName = value; }
        }

        public bool UseReferences
        {
            get { return _innerFactory.UseReferences; }
            set { _innerFactory.UseReferences = value; }
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
