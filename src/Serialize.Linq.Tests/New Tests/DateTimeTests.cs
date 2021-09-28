using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.NewTests
{
    /// <summary>
    /// https://github.com/esskar/Serialize.Linq/issues/105
    /// Test and fix provided by https://github.com/OlegNadymov THX!!!
    /// </summary>
    [TestClass]
    public class DateTimeTests
    {

        [TestMethod]
        public void SerializeDeserializeDateBinary()
        {
            var localDate = DateTime.Now;
            localDate = new DateTime(localDate.Ticks - (localDate.Ticks % TimeSpan.TicksPerMillisecond), localDate.Kind);
            var utcDate = DateTime.SpecifyKind(localDate, DateTimeKind.Utc);

#pragma warning disable CS0618 // type or member is obsolete
            var serializer = new ExpressionSerializer(new BinarySerializer());
#pragma warning restore CS0618 // type or member is obsolete

            Expression<Func<DateTime>> localDateExpression = () => localDate;
            Expression<Func<DateTime>> utcDateExpression = () => utcDate;

            var localDateValue = serializer.SerializeBinary(localDateExpression);
            var utcDateValue = serializer.SerializeBinary(utcDateExpression);

            var actualLocalDateExpression = (Expression<Func<DateTime>>)serializer.DeserializeBinary(localDateValue);
            var actualUtcDateExpression = (Expression<Func<DateTime>>)serializer.DeserializeBinary(utcDateValue);

            var localDateFunc = actualLocalDateExpression.Compile();
            var utcDateFunc = actualUtcDateExpression.Compile();

            Assert.IsFalse(localDate.ToUniversalTime() == utcDate.ToUniversalTime());
            Assert.IsTrue(localDateFunc() == localDate, "return value of local date failed.");
            Assert.IsTrue(localDateFunc().Kind == localDate.Kind, "return value of local date kind failed.");
            Assert.IsTrue(utcDateFunc() == utcDate, "return value of UTC date failed.");
            Assert.IsTrue(utcDateFunc().Kind == utcDate.Kind, "return value of UTC date kind failed.");
            Assert.IsTrue(localDateFunc() == utcDate, "return value of local vs UTC date failed.");
            Assert.IsFalse(localDateFunc().Kind == utcDate.Kind, "return value of local vs UTC date kind failed.");
        }

        [TestMethod]
        public void SerializeDeserializeDateXml()
        {
            var localDate = DateTime.Now;
            localDate = new DateTime(localDate.Ticks - (localDate.Ticks % TimeSpan.TicksPerMillisecond), localDate.Kind);
            var utcDate = DateTime.SpecifyKind(localDate, DateTimeKind.Utc);

#pragma warning disable CS0618 // type or member is obsolete
            var serializer = new ExpressionSerializer(new XmlSerializer());
#pragma warning restore CS0618 // type or member is obsolete

            Expression<Func<DateTime>> localDateExpression = () => localDate;
            Expression<Func<DateTime>> utcDateExpression = () => utcDate;

            var localDateValue = serializer.SerializeText(localDateExpression);
            var utcDateValue = serializer.SerializeText(utcDateExpression);

            var actualLocalDateExpression = (Expression<Func<DateTime>>)serializer.DeserializeText(localDateValue);
            var actualUtcDateExpression = (Expression<Func<DateTime>>)serializer.DeserializeText(utcDateValue);

            var localDateFunc = actualLocalDateExpression.Compile();
            var utcDateFunc = actualUtcDateExpression.Compile();

            Assert.IsFalse(localDate.ToUniversalTime() == utcDate.ToUniversalTime());
            Assert.IsTrue(localDateFunc() == localDate, "return value of local date failed.");
            Assert.IsTrue(localDateFunc().Kind == localDate.Kind, "return value of local date kind failed.");
            Assert.IsTrue(utcDateFunc() == utcDate, "return value of UTC date failed.");
            Assert.IsTrue(utcDateFunc().Kind == utcDate.Kind, "return value of UTC date kind failed.");
            Assert.IsTrue(localDateFunc() == utcDate, "return value of local vs UTC date failed.");
            Assert.IsFalse(localDateFunc().Kind == utcDate.Kind, "return value of local vs UTC date kind failed.");
        }

        [TestMethod]
        public void SerializeDeserializeDateJson()
        {
            var localDate = DateTime.Now;
            localDate = new DateTime(localDate.Ticks - (localDate.Ticks % TimeSpan.TicksPerMillisecond), localDate.Kind);
            var utcDate = DateTime.SpecifyKind(localDate, DateTimeKind.Utc);

#pragma warning disable CS0618 // type or member is obsolete
            var serializer = new ExpressionSerializer(new JsonSerializer());
#pragma warning restore CS0618 // type or member is obsolete

            Expression<Func<DateTime>> localDateExpression = () => localDate;
            Expression<Func<DateTime>> utcDateExpression = () => utcDate;

            var localDateValue = serializer.SerializeText(localDateExpression);
            var utcDateValue = serializer.SerializeText(utcDateExpression);

            var actualLocalDateExpression = (Expression<Func<DateTime>>)serializer.DeserializeText(localDateValue);
            var actualUtcDateExpression = (Expression<Func<DateTime>>)serializer.DeserializeText(utcDateValue);

            var localDateFunc = actualLocalDateExpression.Compile();
            var utcDateFunc = actualUtcDateExpression.Compile();

            Assert.IsFalse(localDate.ToUniversalTime() == utcDate.ToUniversalTime());
            Assert.IsTrue(localDateFunc() == localDate, "return value of local date failed.");
            // the next assert fails for version 2.0.0.0, all DateTime values are giving back with Kind 'UTC'
            Assert.IsTrue(localDateFunc().Kind == localDate.Kind, "return value of local date kind failed.");
            Assert.IsTrue(utcDateFunc() == utcDate, "return value of UTC date failed.");
            Debug.WriteLine(utcDateFunc().Kind);
            Assert.IsTrue(utcDateFunc().Kind == utcDate.Kind, "return value of UTC date kind failed.");
            Assert.IsTrue(localDateFunc() == utcDate, "return value of local vs UTC date failed.");
            // the next assert fails for version 2.0.0.0, all DateTime values are giving back with Kind 'UTC'
            Assert.IsFalse(localDateFunc().Kind == utcDate.Kind, "return value of local vs UTC date kind failed.");
        }
    }
}