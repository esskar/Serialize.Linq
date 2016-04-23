#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Serialize.Linq.Extensions;

namespace Serialize.Linq.Internals
{
    internal class MemberTypeEnumerator : IEnumerator<Type>
    {
        private int _currentIndex;
        private readonly Type _type;
        private readonly BindingFlags _bindingFlags;
        private readonly HashSet<Type> _seenTypes;
        private Type[] _allTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberTypeEnumerator"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        //public MemberTypeEnumerator(Type type, BindingFlags bindingFlags = BindingFlags.Default)
        //    : this(new HashSet<Type>(), type, bindingFlags) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberTypeEnumerator"/> class.
        /// </summary>
        /// <param name="seenTypes">The seen types.</param>
        /// <param name="type">The type.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <exception cref="System.ArgumentNullException">
        /// seenTypes
        /// or
        /// type
        /// </exception>
        public MemberTypeEnumerator(HashSet<Type> seenTypes, Type type, BindingFlags bindingFlags)
        {
            if (seenTypes == null)
                throw new ArgumentNullException("seenTypes");
            if (type == null)
                throw new ArgumentNullException("type");

            _seenTypes = seenTypes;
            _type = type;
            _bindingFlags = bindingFlags;

            _currentIndex = -1;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is considered.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is considered; otherwise, <c>false</c>.
        /// </value>
        public bool IsConsidered
        {
            get { return IsConsideredType(_type); }
        }

        /// <summary>
        /// Determines whether [is considered type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [is considered type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsConsideredType(Type type)
        {
            return true;
        }

        /// <summary>
        /// Determines whether [is considered member] [the specified member].
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>
        ///   <c>true</c> if [is considered member] [the specified member]; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsConsideredMember(MemberInfo member)
        {
            return true;
        }

        /// <summary>
        /// Determines whether [is seen type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [is seen type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsSeenType(Type type)
        {
            return _seenTypes.Contains(type);
        }

        /// <summary>
        /// Adds the type of the seen.
        /// </summary>
        /// <param name="type">The type.</param>
        protected void AddSeenType(Type type)
        {
            _seenTypes.Add(type);
        }

        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <value>
        /// The current.
        /// </value>
        public virtual Type Current
        {
            get { return _allTypes[_currentIndex]; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Gets the current.
        /// </summary>
        /// <value>
        /// The current.
        /// </value>
        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        /// <summary>
        /// Gets the type of the types of.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        protected Type[] GetTypesOfType(Type type)
        {
            var types = new List<Type> { type };
            if (type.HasElementType)
                types.AddRange(GetTypesOfType(type.GetElementType()));

            if (type.GetTypeInfo().IsGenericType)
            {
                foreach (var genericType in type.GetGenericArguments())
                    types.AddRange(GetTypesOfType(genericType));

            }
            return types.ToArray();
        }

        /// <summary>
        /// Builds the types.
        /// </summary>
        /// <returns></returns>
        protected virtual Type[] BuildTypes()
        {
            var types = new List<Type>();
            var members = _type.GetMembers(_bindingFlags);
            foreach (var memberInfo in members.Where(IsConsideredMember))
                types.AddRange(GetTypesOfType(memberInfo.GetReturnType()));
            return types.ToArray();
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        public virtual bool MoveNext()
        {
            if (!IsConsidered)
                return false;

            if (_allTypes == null)
                _allTypes = BuildTypes();

            while (++_currentIndex < _allTypes.Length)
            {
                if (IsSeenType(Current)) continue;
                AddSeenType(Current);
                if (IsConsideredType(Current)) break;
            }

            return _currentIndex < _allTypes.Length;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            _currentIndex = -1;
        }
    }
}