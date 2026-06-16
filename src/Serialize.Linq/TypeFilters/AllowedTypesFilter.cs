using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.TypeFilters
{
    /// <summary>
    /// An allow-list <see cref="ITypeFilter"/>: only the explicitly listed types (and,
    /// optionally, types in the listed namespaces) are permitted; everything else is rejected.
    ///
    /// For generic types, list the open generic definition (e.g. <c>typeof(List&lt;&gt;)</c>),
    /// because closed generics are resolved one component at a time during deserialization
    /// (the open definition plus each generic argument). Primitive expression machinery such
    /// as <c>System.Func&lt;&gt;</c>, <c>bool</c> and <c>object</c> is typically needed too.
    /// </summary>
    public class AllowedTypesFilter : ITypeFilter
    {
        private readonly HashSet<Type> _types;
        private readonly HashSet<string> _namespaces;

        public AllowedTypesFilter(params Type[] allowedTypes)
            : this((IEnumerable<Type>)allowedTypes, null) { }

        public AllowedTypesFilter(IEnumerable<Type> allowedTypes, IEnumerable<string> allowedNamespaces = null)
        {
            _types = new HashSet<Type>(allowedTypes ?? Enumerable.Empty<Type>());
            _namespaces = new HashSet<string>(allowedNamespaces ?? Enumerable.Empty<string>(), StringComparer.Ordinal);
        }

        /// <summary>
        /// Adds a type to the allow-list. For generic types pass the open definition,
        /// e.g. <c>typeof(Dictionary&lt;,&gt;)</c>.
        /// </summary>
        public AllowedTypesFilter Allow(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            _types.Add(type);
            return this;
        }

        /// <summary>
        /// Allows every type whose namespace equals or is nested under <paramref name="namespacePrefix"/>.
        /// </summary>
        public AllowedTypesFilter AllowNamespace(string namespacePrefix)
        {
            if (string.IsNullOrWhiteSpace(namespacePrefix))
                throw new ArgumentNullException(nameof(namespacePrefix));
            _namespaces.Add(namespacePrefix);
            return this;
        }

        public bool IsAllowed(Type type)
        {
            if (type == null)
                return false;

            if (_types.Contains(type))
                return true;

            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && _types.Contains(type.GetGenericTypeDefinition()))
                return true;

            var ns = type.Namespace;
            if (ns != null)
            {
                foreach (var allowed in _namespaces)
                {
                    if (ns == allowed || ns.StartsWith(allowed + ".", StringComparison.Ordinal))
                        return true;
                }
            }

            return false;
        }
    }
}
