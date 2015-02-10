using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Serialize.Linq.Extensions;
using Serialize.Linq.Serializers;
using Serialize.Linq.Tests.Internals;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Tests.Issues
{
    /// <remarks>
    ///   Serialize.Linq.Tests acts as an interfering assembly.
    ///    
    ///    
    ///   Actually, another expression from this assembly (Serialize.Linq.Tests) affects
    ///   execution results. In a couple of places it has similar expressions which have a "where" clause that 
    ///   pushes it's predicate to a certain anonymous class. This class is later picked 
    ///   while processing the expression under test, and cause an exception.
    ///   </remarks>
    [TestClass]
    public class IssueTypeDiscovery
    {
        [TestMethod]
        public void SerializeWithInterferingAssembly()
        {
            // Interfering expression
            Expression<Func<IEnumerable<int>, IEnumerable<int>>> Expr = c =>
                from x in c
                let anotherConst = 100
                where (x == anotherConst)
                select x;

            var DesiredContainer = new Serialize.Linq.Tests.TestDesiredAssembly.Container();

            var Expression = ((Serialize.Linq.Tests.TestDesiredAssembly.Container)DesiredContainer).GetExpression();

            foreach (var textSerializer in new ITextSerializer[] { new JsonSerializer(), new XmlSerializer() })
            {
                var serializer = new ExpressionSerializer(textSerializer);

                var serialized = serializer.SerializeText(Expression);

                var actual = serializer.DeserializeText(serialized);

                ExpressionAssert.AreEqual(Expression, actual);
            }
        }

        [TestMethod]
        public void ToExpressionWithInterferingAssembly()
        {
            // Interfering expression
            Expression<Func<IEnumerable<int>, IEnumerable<int>>> Expr = c =>
                from x in c
                let anotherConst = 100
                where (x == anotherConst)
                select x;

            var DesiredContainer = new Serialize.Linq.Tests.TestDesiredAssembly.Container();

            var Expression = ((Serialize.Linq.Tests.TestDesiredAssembly.Container)DesiredContainer).GetExpression();

            var Node = Expression.ToExpressionNode();

            var Result = Node.ToExpression();

            ExpressionAssert.AreEqual(Expression, Result);
        }
    }
}
