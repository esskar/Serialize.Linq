using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Extensions;
using Serialize.Linq.Tests.Container;
using Serialize.Linq.Tests.Internals;

namespace Serialize.Linq.Tests.Issues
{
    [TestClass]
    public class Issue40
    {
        [TestMethod]
        public void ToExpressionWithInterferingType()
        {
            Expression<Func<IEnumerable<int>, IEnumerable<int>>> expression = c =>
                from x in c
                let randomVar123456789 = 8
                where x == randomVar123456789
                select x;

            var node = expression.ToExpressionNode();

            var result = node.ToExpression();

            ExpressionAssert.AreEqual(expression, result);
        }

        [TestMethod]
        public void ToExpressionWithInterferingTypeFromOtherAssembly()
        {
            var container = new ExpressionContainer();
            var expression = container.GetExpression();
            var node = expression.ToExpressionNode();

            var result = node.ToExpression();

            ExpressionAssert.AreEqual(expression, result);
        }
    }
}
