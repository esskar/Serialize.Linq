
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Nodes;

namespace Serialize.Linq
{
    public abstract class ExpressionContextBase : IExpressionContext
    {
        private readonly ConcurrentDictionary<string, ParameterExpression> _parameterExpressions;
        private readonly ConcurrentDictionary<string, Type> _typeCache;

        protected ExpressionContextBase()
        {
            _parameterExpressions = new ConcurrentDictionary<string, ParameterExpression>();
            _typeCache = new ConcurrentDictionary<string, Type>();
        }

        public bool AllowPrivateFieldAccess { get; set; }

        /// <summary>
        /// Optional allow-list applied to every type resolved during deserialization.
        /// When set, any resolved type rejected by the filter causes a
        /// <see cref="TypeNotAllowedException"/> to be thrown. This is the equivalent of
        /// <c>BinaryFormatter</c>'s <c>SerializationBinder</c> for guarding which types
        /// may be reconstructed from untrusted payloads. <c>null</c> (the default) allows all types.
        /// </summary>
        public ITypeFilter TypeFilter { get; set; }

        public virtual BindingFlags? GetBindingFlags()
        {
            if (!AllowPrivateFieldAccess)
                return null;

            return BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        }

        public virtual ParameterExpression GetParameterExpression(ParameterExpressionNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));
            var key = node.Type.Name + Environment.NewLine + node.Name;
            return _parameterExpressions.GetOrAdd(key, k => Expression.Parameter(node.Type.ToType(this), node.Name));
        }

        public virtual Type ResolveType(TypeNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (string.IsNullOrWhiteSpace(node.Name))
                return null;

            var type = _typeCache.GetOrAdd(node.Name, n =>
            {
                var resolved = Type.GetType(n);
                if (resolved == null)
                {
                    foreach (var assembly in GetAssemblies())
                    {
                        resolved = assembly.GetType(n);
                        if (resolved != null)
                            break;
                    }

                }
                return resolved;
            });

            // Apply the allow-list on every resolution (including cache hits) so that
            // restricted contexts cannot be bypassed once a type has been cached.
            if (type != null && TypeFilter != null && !TypeFilter.IsAllowed(type))
                throw new TypeNotAllowedException(type);

            return type;
        }

        protected abstract IEnumerable<Assembly> GetAssemblies();
    }
}
