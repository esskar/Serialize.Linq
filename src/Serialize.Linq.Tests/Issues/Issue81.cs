using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Extensions;

namespace Serialize.Linq.Tests.Issues
{
    /// <summary>
    /// https://github.com/esskar/Serialize.Linq/issues/81
    /// </summary>
    [TestClass]
    public class Issue81
    {
        class PropertyContainer
        {
            public Guid? Property1 { get; set; }
        }

        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test1()
        {
            var pc1 = new PropertyContainer { Property1 = null };
            var property1 = pc1.Property1;
            var expression1 = (Expression<Func<Guid, bool>>)(x => property1 != null && x != property1);
            var expressionNode = expression1.ToExpressionNode();

            this.TestContext.WriteLine("'{0}' == '{1}'", expression1.ToString(), expressionNode.ToString());

            var expression2 = expressionNode.ToBooleanExpression<Guid>();
            Assert.IsNotNull(expression2);
        }

        [TestMethod]
        public void Test2()
        {
            var pc1 = new PropertyContainer { Property1 = null };
            var expression1 = (Expression<Func<Guid, bool>>)(x => pc1.Property1 != null && x != pc1.Property1);
            var expressionNode = expression1.ToExpressionNode();

            this.TestContext.WriteLine("'{0}' == '{1}'", expression1.ToString(), expressionNode.ToString());

            var expression2 = expressionNode.ToBooleanExpression<Guid>();
            Assert.IsNotNull(expression2);
        }

        [TestMethod]
        public void Test3()
        {
            var pc1 = new PropertyContainer { Property1 = Guid.NewGuid() };
            var expression1 = (Expression<Func<Guid, bool>>)(x => pc1.Property1 != null);
            var expressionNode = expression1.ToExpressionNode();

            this.TestContext.WriteLine("'{0}' == '{1}'", expression1.ToString(), expressionNode.ToString());

            var expression2 = expressionNode.ToBooleanExpression<Guid>();
            Assert.IsNotNull(expression2);
        }
    }
}
