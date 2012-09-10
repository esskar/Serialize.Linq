using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Serialize.Linq.Internals
{
    internal class ComplexPropertyMemberTypeEnumerator : PropertyMemberTypeEnumerator
    {
        private static readonly Type[] __builtinTypes;
        
        static ComplexPropertyMemberTypeEnumerator()
        {
            __builtinTypes = new [] { typeof(bool), typeof(byte), typeof(sbyte), typeof(char), typeof(decimal), typeof(double), typeof(float), 
                typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(object), typeof(short), typeof(ushort), typeof(string),
                typeof(Guid), typeof(Int16),typeof(Int32),typeof(Int64), typeof(UInt16), typeof(UInt32), typeof(UInt64), typeof(TimeSpan), typeof(DateTime) };
        }

        public ComplexPropertyMemberTypeEnumerator(Type type, BindingFlags bindingFlags)
            : this(new HashSet<Type>(), type, bindingFlags) { }

        public ComplexPropertyMemberTypeEnumerator(HashSet<Type> seenTypes, Type type, BindingFlags bindingFlags)
            : base(seenTypes, type, bindingFlags) { }
       
        private static bool IsBuiltinType(Type type)
        {
            return __builtinTypes.Contains(type);
        }

        protected override bool IsConsideredType(Type type)
        {
            return !ComplexPropertyMemberTypeEnumerator.IsBuiltinType(type)
                && base.IsConsideredType(type);
        }
    }
}
