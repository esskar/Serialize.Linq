#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System.Collections;
using System.Collections.Generic;

namespace Serialize.Linq.Internals
{
    public class HashSet<T> : ISet<T>
    {
        private readonly Dictionary<T, object> _dictionary;

        public HashSet()
        {
            _dictionary = new Dictionary<T, object>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _dictionary.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(T item)
        {
            return _dictionary.ContainsKey(item);
        }

        public void Add(T item)
        {
            _dictionary[item] = null;
        }

        public void UnionWith(IEnumerable<T> other)
        {
            foreach (var item in other)
            {
                this.Add(item);
            }
        }
    }
}
