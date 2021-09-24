using System;

namespace Serialize.Linq.Internals
{
    public enum AutoAddCollectionTypes : int
    {
        None = 0,
        AsArray = 1,
        AsList = 2
    }

    [Flags]
    internal enum InternalAutoAddCollectionTypes : int
    {
        None = 0,
        AsArray = 1,
        AsList = 2,
        AsBoth = 3
    }
}
