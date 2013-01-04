using System;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [DataContract]
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
    public abstract class Node
    {
        protected Node() { }

        protected Node(INodeFactory factory)
        {
            if(factory == null)
                throw new ArgumentNullException("factory");

            this.Factory = factory;
        }

        [IgnoreDataMember]
        public INodeFactory Factory
        {
            get; private set;
        }
    }
}
