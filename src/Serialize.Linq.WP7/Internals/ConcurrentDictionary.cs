#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Collections;
using System.Collections.Generic;

namespace Serialize.Linq.Internals
{
    internal class ConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly object _sync = new object();
        private readonly Dictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

        public int Count
        {
            get
            {
                lock (_sync)
                {
                    return _dictionary.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (_sync)
                {
                    return _dictionary[key];
                }
            }
            set
            {
                lock (_sync)
                {
                    _dictionary[key] = value;
                }
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                lock (_sync)
                {
                    return new List<TKey>(_dictionary.Keys);
                }
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                lock (_sync)
                {
                    return new List<TValue>(_dictionary.Values);
                }
            }
        }
        
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        public void Add(TKey key, TValue value)
        {
            lock (_sync)
            {
                _dictionary.Add(key, value);
            }
        }      

        public void Clear()
        {
            lock (_sync)
            {
                _dictionary.Clear();
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return this.ContainsKey(item.Key);
        }

        public bool ContainsKey(TKey key)
        {
            lock (_sync)
            {
                return _dictionary.ContainsKey(key);
            }
        }        

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            // is this how it should be done?

            Dictionary<TKey, TValue> copy;
            lock (_sync)
            {
                copy = new Dictionary<TKey, TValue>(_dictionary);
            }
            return copy.GetEnumerator();
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> retriever)
        {
            lock (_sync)
            {
                TValue value;
                if (!_dictionary.TryGetValue(key, out value))
                {
                    value = retriever(key);
                    _dictionary.Add(key, value);
                }
                return value;
            }
        }        
        
        public bool Remove(TKey key)
        {
            lock (_sync)
            {
                return _dictionary.Remove(key);
            }
        }

        public bool TryAdd(TKey key, TValue value)
        {
            lock (_sync)
            {
                if (_dictionary.ContainsKey(key))
                    return false;
                _dictionary.Add(key, value);
                return true;
            }
        }


        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_sync)
            {
                return _dictionary.TryGetValue(key, out value);
            }
        }        

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }
    }
}
