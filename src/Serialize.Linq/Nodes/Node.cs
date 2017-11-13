﻿#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    /// <summary>
    /// 
    /// </summary>
    #region DataContract
    [DataContract]
#if !SILVERLIGHT && !NETCOREAPP1_1 && !NETSTANDARD1_6 && !NETCOREAPP2_0 && !NETFX_CORE && !WINDOWS_UWP
    [Serializable]
#endif
    #region KnownTypes
    [KnownType(typeof(BinaryExpressionNode))]
    [KnownType(typeof(ConditionalExpressionNode))]
    [KnownType(typeof(ConstantExpressionNode))]
    [KnownType(typeof(ConstructorInfoNode))]
    [KnownType(typeof(ElementInitNode))]
    [KnownType(typeof(ElementInitNodeList))]
    [KnownType(typeof(ExpressionNode))]
    [KnownType(typeof(ExpressionNodeList))]
    [KnownType(typeof(FieldInfoNode))]
    [KnownType(typeof(InvocationExpressionNode))]
    [KnownType(typeof(LambdaExpressionNode))]
    [KnownType(typeof(ListInitExpressionNode))]
    [KnownType(typeof(MemberAssignmentNode))]
    [KnownType(typeof(MemberBindingNode))]
    [KnownType(typeof(MemberBindingNodeList))]
    [KnownType(typeof(MemberExpressionNode))]
    [KnownType(typeof(MemberInfoNode))]
    [KnownType(typeof(MemberInfoNodeList))]    
    [KnownType(typeof(MemberInitExpressionNode))]
    [KnownType(typeof(MemberListBindingNode))]
    [KnownType(typeof(MemberMemberBindingNode))]
    [KnownType(typeof(MethodCallExpressionNode))]
    [KnownType(typeof(NewArrayExpressionNode))]
    [KnownType(typeof(NewExpressionNode))]
    [KnownType(typeof(ParameterExpressionNode))]
    [KnownType(typeof(PropertyInfoNode))]    
    [KnownType(typeof(TypeBinaryExpressionNode))]
    [KnownType(typeof(TypeNode))]
    [KnownType(typeof(UnaryExpressionNode))]
    #endregion
    #endregion
    public abstract class Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        protected Node() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <exception cref="System.ArgumentNullException">factory</exception>
        protected Node(INodeFactory factory)
        {
            if(factory == null)
                throw new ArgumentNullException("factory");

            this.Factory = factory;
        }

        /// <summary>
        /// Gets the factory.
        /// </summary>
        /// <value>
        /// The factory.
        /// </value>
        [IgnoreDataMember]
#if !SILVERLIGHT && !NETCOREAPP1_1 && !NETSTANDARD1_6 && !NETCOREAPP2_0 && !NETFX_CORE && !WINDOWS_UWP
        [NonSerialized]
#endif
        public readonly INodeFactory Factory;        
    }
}
