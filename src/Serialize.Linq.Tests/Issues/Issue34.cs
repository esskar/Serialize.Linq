using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.Issues
{
    // https://github.com/esskar/Serialize.Linq/issues/34
    [TestClass]
    public class Issue34
    {
        [TestMethod]
        public void SerializeWithDateTimeUtcTest()
        {
            foreach (var textSerializer in new ITextSerializer[] { new JsonSerializer(), new XmlSerializer() })
            {
                var serializer = new ExpressionSerializer(textSerializer);
                var yarrs = new[]
                {
                    new Yarr {Date = new DateTime(3000,1,1)},
                    new Yarr {Date = new DateTime(2000,1,1)},
                    new Yarr(),
                    new Yarr { Date = DateTime.Now.AddYears(1) }
                };
                var date = DateTime.UtcNow;
                Expression<Func<Yarr, bool>> expectedExpression = f => f.Date > date;
                var expected = yarrs.Where(expectedExpression.Compile()).Count();

                var serialized = serializer.SerializeText(expectedExpression);
                var actualExpression = (Expression<Func<Yarr, bool>>)serializer.DeserializeText(serialized);
                var actual = yarrs.Where(actualExpression.Compile()).Count();

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void SerializeWithDateTimeLocalTest()
        {
            foreach (var textSerializer in new ITextSerializer[] { new JsonSerializer(), new XmlSerializer() })
            {
                var serializer = new ExpressionSerializer(textSerializer);
                var yarrs = new[]
                {
                    new Yarr {Date = new DateTime(3000,1,1)},
                    new Yarr {Date = new DateTime(2000,1,1)},
                    new Yarr(),
                    new Yarr { Date = DateTime.Now.AddYears(1) }
                };
                var date = DateTime.Now;
                Expression<Func<Yarr, bool>> expectedExpression = f => f.Date > date;
                var expected = yarrs.Where(expectedExpression.Compile()).Count();

                var serialized = serializer.SerializeText(expectedExpression);
                var actualExpression = (Expression<Func<Yarr, bool>>)serializer.DeserializeText(serialized);
                var actual = yarrs.Where(actualExpression.Compile()).Count();

                Assert.AreEqual(expected, actual);
            }
        }

        public class Yarr
        {
            public DateTime Date { get; set; }
        }
    }
}
