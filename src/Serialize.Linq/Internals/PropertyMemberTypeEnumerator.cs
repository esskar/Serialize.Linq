using System;
using System.Collections.Generic;
using System.Reflection;

namespace Serialize.Linq.Internals
{
    internal class PropertyMemberTypeEnumerator : MemberTypeEnumerator
    {
        public PropertyMemberTypeEnumerator(Type type, BindingFlags bindingFlags)
            : this(new HashSet<Type>(), type, bindingFlags) { }

        public PropertyMemberTypeEnumerator(HashSet<Type> seenTypes, Type type, BindingFlags bindingFlags)
            : base(seenTypes, type, bindingFlags | BindingFlags.SetProperty | BindingFlags.GetProperty) { }

        protected override bool IsConsideredMember(MemberInfo member)
        {
            return (member.MemberType & MemberTypes.Property) == MemberTypes.Property && base.IsConsideredMember(member);
        }
    }
}
