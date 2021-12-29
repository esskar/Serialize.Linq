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
using System.Runtime.Serialization;
using Serialize.Linq.Exceptions;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    #if SERIALIZE_LINQ_BORKED_VERION
    [DataContract]
    #else
    [DataContract(Name = "MemberNodeGeneric")]
#endif
#else
    [DataContract(Name = "MN")]
#endif
#if !WINDOWS_UWP
    [Serializable]
#endif
    #endregion
    public abstract class MemberNode<TMemberInfo> : Node where TMemberInfo : MemberInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberNode{TMemberInfo}"/> class.
        /// </summary>
        protected MemberNode() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemberNode{TMemberInfo}"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="memberInfo">The member info.</param>
        protected MemberNode(INodeFactory factory, TMemberInfo memberInfo)
            : base(factory)
        {
            if (memberInfo != null)
                Initialize(memberInfo);
        }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        /// <summary>
        /// Gets or sets the type of the declaring.
        /// </summary>
        /// <value>
        /// The type of the declaring.
        /// </value>
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "D")]
#endif
        #endregion
        public TypeNode DeclaringType { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        /// <summary>
        /// Gets or sets the signature.
        /// </summary>
        /// <value>
        /// The signature.
        /// </value>
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "S")]
#endif
        #endregion
        public string Signature { get; set; }

        /// <summary>
        /// Initializes the instance using specified member info.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        protected virtual void Initialize(TMemberInfo memberInfo)
        {
            DeclaringType = Factory.Create(memberInfo.DeclaringType);
            Signature = memberInfo.ToString();
        }

        /// <summary>
        /// Gets the the declaring type.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">DeclaringType is not set.</exception>
        /// <exception cref="System.TypeLoadException">Failed to load DeclaringType:  + this.DeclaringType</exception>
        protected Type GetDeclaringType(IExpressionContext context)
        {
            if (DeclaringType == null)
                throw new InvalidOperationException("DeclaringType is not set.");

            var declaringType = DeclaringType.ToType(context);
            if (declaringType == null)
                throw new TypeLoadException("Failed to load DeclaringType: " + DeclaringType);

            return declaringType;
        }

        /// <summary>
        /// Converts this instance to an expression.
        /// </summary>
        /// <param name="context">The expression context.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        protected abstract IEnumerable<TMemberInfo> GetMemberInfosForType(IExpressionContext context, Type type);

        /// <summary>
        /// Converts this instance to a member info object of type TMemberInfo.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public virtual TMemberInfo ToMemberInfo(IExpressionContext context)
        {
            if (string.IsNullOrWhiteSpace(Signature))
                return null;

            var declaringType = GetDeclaringType(context);
            var members = GetMemberInfosForType(context, declaringType);

            var member = members.FirstOrDefault(m => m.ToString() == Signature);
            if (member == null)
                throw new MemberNotFoundException("MemberInfo not found. See DeclaringType and MemberSignature properties for more details.", 
                    declaringType, Signature);
            return member;
        }
    }
}