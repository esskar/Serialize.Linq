#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
#if !WINDOWS_PHONE7
using System.Collections.Generic;
#endif
using System.Linq;
using System.Reflection;

namespace Serialize.Linq.Internals
{
    /// <summary>
    /// 
    /// </summary>
    internal class ComplexPropertyMemberTypeEnumerator : PropertyMemberTypeEnumerator
    {
        private static readonly Type[] _builtinTypes;

        /// <summary>
        /// Initializes the <see cref="ComplexPropertyMemberTypeEnumerator"/> class.
        /// </summary>
        static ComplexPropertyMemberTypeEnumerator()
        {
            _builtinTypes = new [] { typeof(bool), typeof(byte), typeof(sbyte), typeof(char), typeof(decimal), typeof(double), typeof(float), 
                typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(object), typeof(short), typeof(ushort), typeof(string),
                typeof(Guid), typeof(TimeSpan), typeof(DateTime),
                typeof(DateTimeOffset)};
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexPropertyMemberTypeEnumerator"/> class.
        /// </summary>C:\Dev\Esskar\Serialize.Linq\src\Serialize.Linq\Internals\MemberTypeEnumerator.cs
        /// <param name="type">The type.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        public ComplexPropertyMemberTypeEnumerator(Type type, BindingFlags bindingFlags)
            : this(new HashSet<Type>(), type, bindingFlags) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexPropertyMemberTypeEnumerator"/> class.
        /// </summary>
        /// <param name="seenTypes">The seen types.</param>
        /// <param name="type">The type.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        public ComplexPropertyMemberTypeEnumerator(HashSet<Type> seenTypes, Type type, BindingFlags bindingFlags)
            : base(seenTypes, type, bindingFlags) { }

        /// <summary>
        /// Determines whether [is builtin type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [is builtin type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsBuiltinType(Type type)
        {
            return _builtinTypes.Contains(type);
        }

        /// <summary>
        /// Determines whether [is considered type] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [is considered type] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        protected override bool IsConsideredType(Type type)
        {
            return !ComplexPropertyMemberTypeEnumerator.IsBuiltinType(type);
        }
    }
}
