using System;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq
{
    /// <summary>
    /// Thrown during deserialization when a <see cref="Type"/> is resolved that is rejected
    /// by the configured <see cref="ITypeFilter"/>.
    /// </summary>
    public class TypeNotAllowedException : Exception
    {
        /// <summary>
        /// The type that was rejected, or <c>null</c> if it could not be determined.
        /// </summary>
        public Type Type { get; }

        public TypeNotAllowedException(Type type)
            : base($"Deserialization of type '{type?.FullName ?? "<unknown>"}' is not allowed by the configured ITypeFilter.")
        {
            Type = type;
        }

        public TypeNotAllowedException(string message)
            : base(message) { }

        public TypeNotAllowedException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
