#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Serialize.Linq.Tests.Internals
{
    internal class ExpressionComparer
    {
        public virtual bool AreEqual(Expression x, Expression y)
        {
            if (object.ReferenceEquals(x, y))
                return true;
            if (x.NodeType != y.NodeType)
                return false;

            switch (x.NodeType)
            {
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                case ExpressionType.UnaryPlus:
                    return this.AreEqualUnary((UnaryExpression)x, (UnaryExpression)y);
                case ExpressionType.Add:
                case ExpressionType.AddAssign:
                case ExpressionType.AddAssignChecked:
                case ExpressionType.AddChecked:
                case ExpressionType.Assign:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractAssign:
                case ExpressionType.SubtractAssignChecked:
                case ExpressionType.SubtractChecked:                
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyAssign:
                case ExpressionType.MultiplyAssignChecked:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.DivideAssign:
                case ExpressionType.Modulo:
                case ExpressionType.ModuloAssign:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    return this.AreEqualBinary((BinaryExpression)x, (BinaryExpression)y);
                case ExpressionType.TypeIs:
                case ExpressionType.TypeEqual:
                    return this.AreEqualTypeBinary((TypeBinaryExpression)x, (TypeBinaryExpression)y);
                case ExpressionType.Conditional:
                    return this.AreEqualConditional((ConditionalExpression)x, (ConditionalExpression)y);
                case ExpressionType.Constant:
                    return this.AreEqualConstant((ConstantExpression)x, (ConstantExpression)y);
                case ExpressionType.Parameter:
                    return this.AreEqualParameter((ParameterExpression)x, (ParameterExpression)y);
                case ExpressionType.MemberAccess:
                    return this.AreEqualMemberAccess((MemberExpression)x, (MemberExpression)y);
                case ExpressionType.Call:
                    return this.AreEqualMethodCall((MethodCallExpression)x, (MethodCallExpression)y);
                case ExpressionType.Lambda:
                    return this.AreEqualLambda((LambdaExpression)x, (LambdaExpression)y);
                case ExpressionType.New:
                    return this.AreEqualNew((NewExpression)x, (NewExpression)y);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    return this.AreEqualNewArray((NewArrayExpression)x, (NewArrayExpression)y);
                case ExpressionType.Invoke:
                    return this.AreEqualInvocation((InvocationExpression)x, (InvocationExpression)y);
                case ExpressionType.MemberInit:
                    return this.AreEqualMemberInit((MemberInitExpression)x, (MemberInitExpression)y);
                case ExpressionType.ListInit:
                    return this.AreEqualListInit((ListInitExpression)x, (ListInitExpression)y);
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", x.NodeType));
            }
        }

        protected virtual bool AreEqualBinding(MemberBinding x, MemberBinding y)
        {
            if (x.BindingType != y.BindingType)
                return false;

            switch (x.BindingType)
            {
                case MemberBindingType.Assignment:
                    return this.AreEqualMemberAssignment((MemberAssignment)x, (MemberAssignment)y);
                case MemberBindingType.MemberBinding:
                    return this.AreEqualMemberMemberBinding((MemberMemberBinding)x, (MemberMemberBinding)y);
                case MemberBindingType.ListBinding:
                    return this.AreEqualMemberListBinding((MemberListBinding)x, (MemberListBinding)y);
                default:
                    throw new Exception(string.Format("Unhandled binding type '{0}'", y.BindingType));
            }
        }

        protected virtual bool AreEqualElementInitializer(ElementInit x, ElementInit y)
        {
            return this.AreEqualExpressionList(x.Arguments, y.Arguments);
        }

        protected virtual bool AreEqualUnary(UnaryExpression x, UnaryExpression y)
        {
            return this.AreEqual(x.Operand, y.Operand);
        }

        protected virtual bool AreEqualBinary(BinaryExpression x, BinaryExpression y)
        {
            return this.AreEqual(x.Left, y.Left)
                && this.AreEqual(x.Right, y.Right)
                && this.AreEqual(x.Conversion, y.Conversion);
        }

        protected virtual bool AreEqualTypeBinary(TypeBinaryExpression x, TypeBinaryExpression y)
        {
            return x.NodeType == y.NodeType 
                && x.TypeOperand == y.TypeOperand
                && this.AreEqual(x.Expression, y.Expression);
        }

        protected virtual bool AreEqualConstant(ConstantExpression x, ConstantExpression y)
        {
            return x.Type == y.Type
                && (object.ReferenceEquals(x.Value, y.Value) || x.Value.Equals(y.Value));
        }

        protected virtual bool AreEqualConditional(ConditionalExpression x, ConditionalExpression y)
        {
            return this.AreEqual(x.Test, y.Test)
                && this.AreEqual(x.IfTrue, y.IfTrue)
                && this.AreEqual(x.IfFalse, y.IfFalse);
        }

        protected virtual bool AreEqualParameter(ParameterExpression x, ParameterExpression y)
        {
            return x.Type == y.Type
                && (object.ReferenceEquals(x.Name, y.Name) || x.Name.Equals(y.Name));
        }

        protected virtual bool AreEqualMemberAccess(MemberExpression x, MemberExpression y)
        {
            return this.AreEqual(x.Expression, y.Expression);
        }

        protected virtual bool AreEqualMethodCall(MethodCallExpression x, MethodCallExpression y)
        {
            var isEqual = this.AreEqual(x.Object, y.Object);
            if (isEqual) isEqual = this.AreEqualExpressionList(x.Arguments, y.Arguments);
            return isEqual;
        }

        protected virtual bool AreEqualExpressionList(ReadOnlyCollection<Expression> x, ReadOnlyCollection<Expression> y)
        {
            var isEqual = x.Count.Equals(y.Count);
            for (var i = 0; isEqual && i < x.Count; ++i)
                isEqual = this.AreEqual(x[i], y[i]);
            return isEqual;
        }

        protected virtual bool AreEqualMemberAssignment(MemberAssignment x, MemberAssignment y)
        {
            return this.AreEqual(x.Expression, y.Expression);
        }

        protected virtual bool AreEqualMemberMemberBinding(MemberMemberBinding x, MemberMemberBinding y)
        {
            return this.AreEqualBindingList(x.Bindings, y.Bindings);
        }

        protected virtual bool AreEqualMemberListBinding(MemberListBinding x, MemberListBinding y)
        {
            return this.AreEqualElementInitializerList(x.Initializers, y.Initializers);
        }

        protected virtual bool AreEqualBindingList(ReadOnlyCollection<MemberBinding> x, ReadOnlyCollection<MemberBinding> y)
        {
            var isEqual = x.Count.Equals(y.Count);
            for (var i = 0; isEqual && i < x.Count; ++i)
                isEqual = this.AreEqualBinding(x[i], y[i]);
            return isEqual;
        }

        protected virtual bool AreEqualElementInitializerList(ReadOnlyCollection<ElementInit> x, ReadOnlyCollection<ElementInit> y)
        {
            var isEqual = x.Count.Equals(y.Count);
            for (var i = 0; isEqual && i < x.Count; ++i)
                isEqual = this.AreEqualElementInitializer(x[i], y[i]);
            return isEqual;
        }

        protected virtual bool AreEqualLambda(LambdaExpression x, LambdaExpression y)
        {
            return this.AreEqual(x.Body, y.Body);
        }

        protected virtual bool AreEqualNew(NewExpression x, NewExpression y)
        {
            return this.AreEqualExpressionList(x.Arguments, y.Arguments);
        }

        protected virtual bool AreEqualMemberInit(MemberInitExpression x, MemberInitExpression y)
        {
            return this.AreEqualNew(x.NewExpression, y.NewExpression)
                && this.AreEqualBindingList(x.Bindings, y.Bindings);
        }

        protected virtual bool AreEqualListInit(ListInitExpression x, ListInitExpression y)
        {
            return this.AreEqualNew(x.NewExpression, y.NewExpression)
                && this.AreEqualElementInitializerList(x.Initializers, y.Initializers);
        }

        protected virtual bool AreEqualNewArray(NewArrayExpression x, NewArrayExpression y)
        {
            return this.AreEqualExpressionList(x.Expressions, y.Expressions);
        }

        protected virtual bool AreEqualInvocation(InvocationExpression x, InvocationExpression y)
        {
            return this.AreEqualExpressionList(x.Arguments, y.Arguments)
                && this.AreEqual(x.Expression, y.Expression);
        }        
    }
}