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
    internal abstract class MemberTypeEnumerator
    {
        private readonly Type _type;
        private readonly BindingFlags _bindingFlags;
        private readonly ICollection<Type> _seenTypes;
        private IEnumerable<Type> _allTypes;
        private ICollection<Type> _referedTypes;

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
        public MemberTypeEnumerator(IEnumerable<Type> seenTypes, Type type, BindingFlags bindingFlags)
        {
            if (seenTypes == null)
                throw new ArgumentNullException(nameof(seenTypes));
            _seenTypes = new HashSet<Type>(seenTypes);
            _type = type ?? throw new ArgumentNullException(nameof(type));
            _bindingFlags = bindingFlags;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is considered.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is considered; otherwise, <c>false</c>.
        /// </value>
        public bool IsConsidered => this.IsConsideredType(_type);

        /// <summary>
        /// Determines whether [is considered type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [is considered type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        protected abstract bool IsConsideredType(Type type);

        /// <summary>
        /// Determines whether [is considered member] [the specified member].
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>
        ///   <c>true</c> if [is considered member] [the specified member]; otherwise, <c>false</c>.
        /// </returns>
        protected abstract bool IsConsideredMember(MemberInfo member);

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
            _referedTypes = null;
        }

        public IEnumerable<Type> ReferedTypes
        {
            get
            {
                if (_referedTypes == null)
                {
                    _referedTypes = new HashSet<Type>();
                    AddTypes();
                }
                return _referedTypes;
            }
        }

        /// <summary>
        /// Gets the type of the types of.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        protected IEnumerable<Type> GetTypesOfType(Type type)
        {
            var types = new List<Type> { type };
            if (type.HasElementType)
                types.AddRange(this.GetTypesOfType(type.GetElementType()));
            if (type.IsGenericType())
            {
                foreach (var genericType in type.GetGenericArguments())
                    types.AddRange(this.GetTypesOfType(genericType));

            }
            return types;
        }

        /// <summary>
        /// Builds the types.
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<Type> BuildTypes()
        {
            var types = new List<Type>();
            var members = _type.GetMembers(_bindingFlags);
            foreach (var memberInfo in members.Where(this.IsConsideredMember))
                types.AddRange(this.GetTypesOfType(memberInfo.GetReturnType()));
            return types;
        }

        protected virtual void AddTypes()
        {
            if (this.IsConsidered)
            {
                if (_allTypes == null)
                    _allTypes = this.BuildTypes();
                foreach (var type in _allTypes)
                {
                    if (!this.IsSeenType(type))
                    {
                        _seenTypes.Add(type);
                        if (this.IsConsideredType(type))
                        {
                            _referedTypes.Add(type);
                        }
                    }
                }
            }
        }
    }
}