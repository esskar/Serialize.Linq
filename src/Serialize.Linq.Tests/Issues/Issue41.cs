using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;
using Serialize.Linq.Extensions;
using Serialize.Linq.Tests.Container;
using Serialize.Linq.Tests.Internals;

namespace Serialize.Linq.Tests.Issues
{
    /// <summary>
    /// https://github.com/esskar/Serialize.Linq/issues/41
    /// </summary>
    
    public class Issue41
    {
        [Fact]
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

        [Fact]
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
