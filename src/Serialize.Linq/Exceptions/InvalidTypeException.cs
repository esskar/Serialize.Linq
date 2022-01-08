
using System;

namespace Serialize.Linq.Exceptions
{
    /// <summary>
    /// Thrown when given type does not match expected type or type interface.
    /// </summary>
    public class InvalidTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTypeException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidTypeException(string message)
            : base(message) { }
    }
}
