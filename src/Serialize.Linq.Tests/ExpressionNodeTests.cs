using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Factories;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Tests.Internals;

namespace Serialize.Linq.Tests
{
    [TestClass]
    public class ExpressionNodeTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
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

        [TestMethod]
        public void SimpleConditionalTest()
        {
            this.AssertExpression(Expression.Condition(Expression.Constant(true), Expression.Constant(5), Expression.Constant(10)));
            this.AssertExpression(Expression.Condition(Expression.Constant(false), Expression.Constant(5), Expression.Constant(10)));
        }

        [TestMethod]
        public void SimpleConditionalWithNullConstantTest()
        {
            var argParam = Expression.Parameter(typeof(Type), "type");
            var stringProperty = Expression.Property(argParam, "AssemblyQualifiedName");

            this.AssertExpression(Expression.Condition(Expression.Constant(true), stringProperty, Expression.Constant(null, typeof(string))));
        }

        [TestMethod]
        public void SimpleUnaryTest()
        {
            this.AssertExpression(Expression.UnaryPlus(Expression.Constant(43)));
        }

        [TestMethod]
        public void SimpleTypedNullConstantTest()
        {
            this.AssertExpression(Expression.Constant(null, typeof(string)));
        }

        [TestMethod]
        public void SimpleLambdaTest()
        {
            this.AssertExpression(Expression.Lambda(Expression.Constant("body"), Expression.Parameter(typeof(string))));
        }

        [TestMethod]
        public void SimpleTypeBinaryTest()
        {
            this.AssertExpression(Expression.TypeIs(Expression.Variable(this.GetType()), typeof(object)));
        }

        [TestMethod]
        public void SimpleMemberTest()
        {
            var type = this.GetType();
            var property = type.GetProperty("TestContext");

            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);

            this.AssertExpression(propertyAccess);
        }

        private void AssertExpression(Expression expression, string message = null)
        {
            this.AssertExpression<NodeFactory>(expression, message);
        }

        private void AssertExpression<TFactory>(Expression expression, string message = null)
            where TFactory : INodeFactory, new()
        {
            var factory = new TFactory();
            var expressionNode = factory.Create(expression);
            var createdExpression = expressionNode.ToExpression();

            PublicInstancePropertiesAssert.AreEqual(expression, createdExpression, message);
            this.TestContext.WriteLine("'{0}' == '{1}'", expression.ToString(), createdExpression.ToString());            
        }

        private void AssertExpression<TFactory>(Expression expression, Expression expectedExpression, string message = null)
            where TFactory : INodeFactory, new()
        {
            var factory = new TFactory();
            var expressionNode = factory.Create(expression);
            var createdExpression = expressionNode.ToExpression();

            ExpressionAssert.AreEqual(expectedExpression, createdExpression, message);
            this.TestContext.WriteLine("'{0}' == '{1}'", expression.ToString(), createdExpression.ToString());
        }

        private void AssertExpression<TFactory, T, TResult>(Expression<Func<T, TResult>> expression, Expression<Func<T, TResult>> expectedExpression, string message = null)
            where TFactory : INodeFactory, new()
        {
            this.AssertExpression<TFactory>(expression, expectedExpression, message);
        }
    }
}
