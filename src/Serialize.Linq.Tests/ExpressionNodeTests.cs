#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xunit;
using Serialize.Linq.Extensions;
using Serialize.Linq.Factories;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Nodes;
using Serialize.Linq.Tests.Internals;

namespace Serialize.Linq.Tests
{
    
    public class ExpressionNodeTests
    {
        [Fact]
        public void SimpleBinaryExpressionTest()
        {
            this.AssertExpression(Expression.Add(Expression.Constant(5), Expression.Constant(10)));
            this.AssertExpression(Expression.Subtract(Expression.Constant(5), Expression.Constant(10)));
            this.AssertExpression(Expression.Multiply(Expression.Constant(5), Expression.Constant(10)));
            this.AssertExpression(Expression.Divide(Expression.Constant(5), Expression.Constant(10)));

            this.AssertExpression(Expression.AddAssign(Expression.Variable(typeof(int), "x"), Expression.Constant(10)));
            this.AssertExpression(Expression.SubtractAssign(Expression.Variable(typeof(int), "x"), Expression.Constant(10)));
            this.AssertExpression(Expression.MultiplyAssign(Expression.Variable(typeof(int), "x"), Expression.Constant(10)));
            this.AssertExpression(Expression.DivideAssign(Expression.Variable(typeof(int), "x"), Expression.Constant(10)));
        }

        [Fact]
        public void SimpleConditionalTest()
        {
            this.AssertExpression(Expression.Condition(Expression.Constant(true), Expression.Constant(5), Expression.Constant(10)));
            this.AssertExpression(Expression.Condition(Expression.Constant(false), Expression.Constant(5), Expression.Constant(10)));
        }

        [Fact]
        public void SimpleConditionalWithNullConstantTest()
        {
            var argParam = Expression.Parameter(typeof(Type), "type");
            var stringProperty = Expression.Property(argParam, "AssemblyQualifiedName");

            this.AssertExpression(Expression.Condition(Expression.Constant(true), stringProperty, Expression.Constant(null, typeof(string))));
        }

        [Fact]
        public void SimpleUnaryTest()
        {
            this.AssertExpression(Expression.UnaryPlus(Expression.Constant(43)));
        }

        [Fact]
        public void SimpleTypedNullConstantTest()
        {
            this.AssertExpression(Expression.Constant(null, typeof(string)));
        }

        [Fact]
        public void SimpleLambdaTest()
        {
            this.AssertExpression(Expression.Lambda(Expression.Constant("body"), Expression.Parameter(typeof(string))));
        }

        [Fact]
        public void SimpleTypeBinaryTest()
        {
            this.AssertExpression(Expression.TypeIs(Expression.Variable(this.GetType()), typeof(object)));
            this.AssertExpression(Expression.TypeEqual(Expression.Variable(this.GetType()), typeof(object)));
        }        

        public string TestContext { get; set; }

        [Fact]
        public void SimpleMemberTest()
        {
            var type = this.GetType();
            var property = type.GetProperty("TestContext");

            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);

            this.AssertExpression(propertyAccess);
        }

        [Fact]
        public void ToExpressionNodeTest()
        {
            AssertToExpressionNode(SerializerTestData.TestExpressions);
            AssertToExpressionNode(SerializerTestData.TestNodesOnlyExpressions);
        }

        private static void AssertToExpressionNode(IEnumerable<Expression> expressions)
        {
            foreach (var expression in expressions)
            {
                ExpressionNode node = null;
                try 
                { 
                    node = expression.ToExpressionNode(); 
                }
                catch (Exception ex)
                {
                    Assert.True(false, $"Failed to convert '{expression}' to expression node: {ex}");                    
                }

                if (expression != null)
                    Assert.NotNull(node); //, "Unable to convert '{0}' to expression node.", expression);
                else
                    Assert.Null(node); //, "Null expression should convert to null expression node.");
            }
        }

        private void AssertExpression(Expression expression, string message = null)
        {
            this.AssertExpression<NodeFactory>(expression, message);
        }

        private void AssertExpression<TFactory>(Expression expression, string message = null)
            where TFactory : INodeFactory
        {
            var factory = (TFactory)Activator.CreateInstance(typeof(TFactory));
            var expressionNode = factory.Create(expression);
            var createdExpression = expressionNode.ToExpression();

            ExpressionAssert.AreEqual(expression, createdExpression, message);

            //PublicInstancePropertiesAssert.Equal(expression, createdExpression, message);
            //Assert.Equal(expression.ToString(), createdExpression.ToString(), message);

            // this.TestContext.WriteLine("'{0}' == '{1}'", expression.ToString(), createdExpression.ToString());
        }
    }
}
