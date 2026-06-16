using System;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.TypeFilters
{
    /// <summary>
    /// An <see cref="ITypeFilter"/> backed by a user supplied predicate. Useful for ad-hoc
    /// rules, e.g. <c>new DelegateTypeFilter(t =&gt; t.Namespace?.StartsWith("MyApp") == true)</c>.
    /// </summary>
    public class DelegateTypeFilter : ITypeFilter
    {
        private readonly Func<Type, bool> _predicate;

        public DelegateTypeFilter(Func<Type, bool> predicate)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        public bool IsAllowed(Type type)
        {
            return type != null && _predicate(type);
        }
    }
}
