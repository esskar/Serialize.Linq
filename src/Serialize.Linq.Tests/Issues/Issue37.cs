using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Serializers;
using Serialize.Linq.Tests.Internals;

namespace Serialize.Linq.Tests.Issues
{
    [TestClass]
    public class Issue37
    {
        [TestMethod]
        public void DynamicsTests()
        {
            var expressions = new List<Expression>();

            Expression<Func<Item, dynamic>> objectExp = item => new {item.Name, item.ProductId};
            Expression<Func<string, dynamic>> stringExp = str => new { Text = str };

            expressions.Add(objectExp);
            expressions.Add(stringExp);

            foreach (var textSerializer in new ITextSerializer[] { new JsonSerializer(), new XmlSerializer() })
            {
                var serializer = new ExpressionSerializer(textSerializer);
                foreach (var expected in expressions)
                {
                    var serialized = serializer.SerializeText(expected);
                    var actual = serializer.DeserializeText(serialized);

                    ExpressionAssert.AreEqual(expected, actual);
                }
            }
        }

        public class Item
        {
            public string Name { get; set; }

            public string ProductId { get; set; }
        }
    }
}
