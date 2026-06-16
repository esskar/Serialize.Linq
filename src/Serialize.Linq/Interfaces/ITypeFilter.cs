using System;

namespace Serialize.Linq.Interfaces
{
    /// <summary>
    /// Controls which <see cref="Type"/>s are allowed to be resolved while an expression
    /// tree is being deserialized. This is the Serialize.Linq equivalent of
    /// <c>BinaryFormatter</c>'s <c>SerializationBinder</c>: it lets callers restrict
    /// deserialization to a known set of types and reject everything else.
    ///
    /// A filter is consulted for every type encountered during deserialization, including
    /// the open generic definition and each generic argument of a closed generic type
    /// (e.g. <c>List&lt;Customer&gt;</c> is checked as <c>List&lt;&gt;</c> and <c>Customer</c>).
    /// </summary>
    public interface ITypeFilter
    {
        /// <summary>
        /// Determines whether the given <paramref name="type"/> may be resolved during deserialization.
        /// </summary>
        /// <param name="type">The resolved type. Never <c>null</c>.</param>
        /// <returns><c>true</c> to allow the type; <c>false</c> to reject it.</returns>
        bool IsAllowed(Type type);
    }
}
