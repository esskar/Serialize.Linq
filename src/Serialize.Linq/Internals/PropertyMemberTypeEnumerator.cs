using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serialize.Linq.Internals
{
    internal class PropertyMemberTypeEnumerator : MemberTypeEnumerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMemberTypeEnumerator"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        public PropertyMemberTypeEnumerator(Type type, BindingFlags bindingFlags)
            : this(new HashSet<Type>(), type, bindingFlags) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMemberTypeEnumerator"/> class.
        /// </summary>
        /// <param name="seenTypes">The seen types.</param>
        /// <param name="type">The type.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        public PropertyMemberTypeEnumerator(HashSet<Type> seenTypes, Type type, BindingFlags bindingFlags)
            : base(seenTypes, type, bindingFlags | BindingFlags.SetProperty | BindingFlags.GetProperty) { }

        /// <summary>
        /// Determines whether [is considered member] [the specified member].
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>
        ///   <c>true</c> if [is considered member] [the specified member]; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsConsideredMember(MemberInfo member)
        {
            return (member.MemberType & MemberTypes.Property) == MemberTypes.Property && base.IsConsideredMember(member);
        }
    }
}
