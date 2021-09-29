using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.NewTests
{
    /// <summary>
    /// https://github.com/esskar/Serialize.Linq/issues/105
    /// Test and fix provided by https://github.com/oahrens
    /// </summary>
    [TestClass]
    public class TimeSpanTests
    {

        [TestMethod]
        public void SerializeDeserializeTimeSpanBinary()
        {
            var now = DateTime.Now;
            var span = TimeSpan.FromSeconds(1);
            Expression<Func<DateTime, bool>> expression = date => date + span > now;
#pragma warning disable CS0618 // type or member is obsolete
            var serializer = new ExpressionSerializer(new BinarySerializer());
#pragma warning restore CS0618 // type or member is obsolete
            var value = serializer.SerializeBinary(expression);
            var actualExpression = (Expression<Func<DateTime, bool>>)serializer.DeserializeBinary(value);
            var func = actualExpression.Compile();

            Assert.IsTrue(func(now), "TimeSpan failed.");
        }

        [TestMethod]
        public void SerializeDeserializeTimeSpanXml()
        {
            var now = DateTime.Now;
            var span = TimeSpan.FromSeconds(1);
            Expression<Func<DateTime, bool>> expression = date => date + span > now;
#pragma warning disable CS0618 // type or member is obsolete
            var serializer = new ExpressionSerializer(new XmlSerializer());
#pragma warning restore CS0618 // type or member is obsolete
            var value = serializer.SerializeText(expression);
            var actualExpression = (Expression<Func<DateTime, bool>>)serializer.DeserializeText(value);
            var func = actualExpression.Compile();

            Assert.IsTrue(func(now), "TimeSpan failed.");
        }

        [TestMethod]
        public void SerializeDeserializeTimeSpanJson()
        {
            var now = DateTime.Now;
            var span = TimeSpan.FromSeconds(1);
            Expression<Func<DateTime, bool>> expression = date => date + span > now;
#pragma warning disable CS0618 // type or member is obsolete
            var serializer = new ExpressionSerializer(new JsonSerializer());
#pragma warning restore CS0618 // type or member is obsolete
            // prevents MissingMethodException in version 2.0.0.0; no longer needed in the new version:
            // ValueConverter.AddCustomConverter(typeof(TimeSpan), (object inSpan, Type type) => XmlConvert.ToTimeSpan((string)inSpan));
            var value = serializer.SerializeText(expression);
            var actualExpression = (Expression<Func<DateTime, bool>>)serializer.DeserializeText(value);
            var func = actualExpression.Compile();

            // the next line throws a MissingMethodException for version 2.0.0.0 if 'prevents' line is commented out
            Assert.IsTrue(func(now), "TimeSpan failed.");
        }
    }
}