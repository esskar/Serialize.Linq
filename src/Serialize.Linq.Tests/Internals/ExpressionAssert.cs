using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Serialize.Linq.Tests.Internals
{
    internal class ExpressionAssert
    {
        public static void AreEqual<TDelegate>(Expression<TDelegate> expected, Expression<TDelegate> actual, string message = null)
        {
            AreEqual(expected, (Expression)actual, message);
        }

        public static void AreEqual(Expression expected, Expression actual, string message = null)
        {
            var comparer = new ExpressionComparer();
            var result = comparer.AreEqual(expected, actual);
            if (result) 
                return;

            var failMessage = !string.IsNullOrWhiteSpace(message) ? message : string.Empty;
            failMessage += string.Format("Expected was <{0}>, Actual was <{1}>", expected, actual);
            Assert.Fail(failMessage);
        }
    }
}