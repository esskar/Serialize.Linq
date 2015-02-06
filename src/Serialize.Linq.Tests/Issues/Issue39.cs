using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Extensions;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Tests.Issues
{
    /// <summary>
    /// https://github.com/esskar/Serialize.Linq/issues/39
    /// </summary>
    [TestClass]
    public class Issue39
    {
        private class DataPoint
        {
            public DateTime Timestamp;
            public int AcctId;
        }

        [TestMethod]
        public void ToExpressionNodeWithSimilarConstantNames()
        {
            var feb1 = new DateTime(2015, 2, 1);
            var feb15 = new DateTime(2015, 2, 15);

            Expression<Func<DataPoint, bool>> expression =
                dp => dp.Timestamp >= feb1 && dp.Timestamp < feb15 && dp.AcctId == 1;

            var result = expression.ToExpressionNode();

            Assert.IsNotNull(result);
        }
    }
}
