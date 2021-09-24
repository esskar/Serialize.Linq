using System;

namespace Serialize.Linq.Internals
{
    public enum AutomaticAddKnownCollections : int
    {
        None = 0,
        AsArray = 1,
        AsList = 2
    }

    [Flags]
    internal enum InternalAutomaticAddKnownCollections : int
    {
        None = 0,
        AsArray = 1,
        AsList = 2,
        AsBoth = 3
    }
}
