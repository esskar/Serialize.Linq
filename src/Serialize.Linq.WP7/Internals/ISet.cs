#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System.Collections.Generic;

namespace Serialize.Linq.Internals
{
    internal interface ISet<T> : IEnumerable<T>
    {
        bool Contains(T item);

        void Add(T item);

        void UnionWith(IEnumerable<T> other);
    }
}
