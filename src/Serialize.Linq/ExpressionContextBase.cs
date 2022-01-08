
using System;
using System.Collections.Generic;
using System.Reflection;
#if !WINDOWS_PHONE
using System.Collections.Concurrent;
#else
using Serialize.Linq.Internals;
#endif
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

            return _typeCache.GetOrAdd(node.Name, n =>
            {
                var type = Type.GetType(n);
                if (type == null)
                {
                    foreach (var assembly in GetAssemblies())
                    {
                        type = assembly.GetType(n);
                        if (type != null)
                            break;
                    }

                }
                return type;
            });
        }

        protected abstract IEnumerable<Assembly> GetAssemblies();
    }
}
