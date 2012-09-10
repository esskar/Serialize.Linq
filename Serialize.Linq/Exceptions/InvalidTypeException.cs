using System;

namespace Serialize.Linq.Exceptions
{
    public class InvalidTypeException : Exception
    {
        public InvalidTypeException(string message)
            : base(message) { }

        public InvalidTypeException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
