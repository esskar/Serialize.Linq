using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Serialize.Linq.Factories;
using Serialize.Linq.Serializers;
using Serialize.Linq.Tests.Internals;

namespace Serialize.Linq.Tests.Issues
{
    // https://github.com/esskar/Serialize.Linq/issues/169
    [TestClass]
    public class Issue169
    {
        public class Test
        {
            public int Method1(int num) => num + 1;
            public int Method2() => 5;
        }
        
        private static Expression<Func<Test, IEnumerable<string>>> Test4()
        {
            return t => new[] { t.Method1(t.Method2()).ToString() };
        }

        [TestMethod]
        public void Reproduce()
        {
            var expression = Test4();
            var serializerr = new ExpressionSerializer(new JsonSerializer());
            var str = serializerr.SerializeText(expression);
            
            var actual = serializerr.DeserializeText(str);
            ExpressionAssert.AreEqual(expression, actual);
        }
    }
}
