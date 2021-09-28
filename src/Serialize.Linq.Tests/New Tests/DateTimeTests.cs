using System;
using System.Collections.Generic;
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
            var realUtcDate = localDate.ToUniversalTime();
            var testDate = localDate;

#pragma warning disable CS0618 // type or member is obsolete
            var serializer = new ExpressionSerializer(new BinarySerializer());
#pragma warning restore CS0618 // type or member is obsolete

            Expression<Func<DateTime, bool>> localExpression = date => localDate == date && localDate.Kind == date.Kind && localDate.ToUniversalTime() == date.ToUniversalTime() && localDate.ToUniversalTime().Kind == date.ToUniversalTime().Kind;
            Expression<Func<DateTime, bool>> utcExpression = date => utcDate == date && utcDate.Kind == date.Kind && utcDate.ToUniversalTime() == date.ToUniversalTime() && utcDate.ToUniversalTime().Kind == date.ToUniversalTime().Kind;
            Expression<Func<DateTimeKind>> utcKindExpression = () => utcDate.Kind;
            Expression<Func<DateTimeKind>> localKindExpression = () => localDate.Kind;
            Expression<Func<DateTime>> localDateExpression = () => localDate;
            Expression<Func<DateTime>> utcDateExpression = () => utcDate;

            var localValue = serializer.SerializeBinary(localExpression);
            var utcValue = serializer.SerializeBinary(utcExpression);
            var utcKindValue = serializer.SerializeBinary(utcKindExpression);
            var localKindValue = serializer.SerializeBinary(localKindExpression);
            var localDateValue = serializer.SerializeBinary(localDateExpression);
            var utcDateValue = serializer.SerializeBinary(utcDateExpression);

            var actualLocalExpression = (Expression<Func<DateTime, bool>>)serializer.DeserializeBinary(localValue);
            var actualUtcExpression = (Expression<Func<DateTime, bool>>)serializer.DeserializeBinary(utcValue);
            var actualLocalKindExpression = (Expression<Func<DateTimeKind>>)serializer.DeserializeBinary(localKindValue);
            var actualUtcKindExpression = (Expression<Func<DateTimeKind>>)serializer.DeserializeBinary(utcKindValue);
            var actualLocalDateExpression = (Expression<Func<DateTime>>)serializer.DeserializeBinary(localDateValue);
            var actualUtcDateExpression = (Expression<Func<DateTime>>)serializer.DeserializeBinary(utcDateValue);

            var localFunc = actualLocalExpression.Compile();
            var utcFunc = actualUtcExpression.Compile();
            var localKindFunc = actualLocalKindExpression.Compile();
            var utcKindFunc = actualUtcKindExpression.Compile();
            var localDateFunc = actualLocalDateExpression.Compile();
            var utcDateFunc = actualUtcDateExpression.Compile();

            Assert.IsFalse(localDate.ToUniversalTime() == utcDate.ToUniversalTime());
            Assert.IsTrue(localFunc(localDate), "internal comparison of local date failed.");
            Assert.IsTrue(utcFunc(utcDate), "internal comparison of UTC date failed.");
            Assert.IsFalse(localFunc(utcDate), "internal comparison of local vs UTC date failed.");
            Assert.IsFalse(utcFunc(localDate), "internal comparison of UTC vs local date failed.");
            Assert.IsTrue(localKindFunc() == localDate.Kind, "internal comparison of local date kind failed.");
            Assert.IsTrue(utcKindFunc() == utcDate.Kind, "internal comparison of utc date kind failed.");
            Assert.IsTrue(localDateFunc() == localDate, "return value of local date failed.");
            Assert.IsTrue(localDateFunc().Kind == localDate.Kind, "return value of local date kind failed.");
            Assert.IsTrue(utcDateFunc() == utcDate, "return value of UTC date failed.");
            Assert.IsTrue(utcDateFunc().Kind == utcDate.Kind, "return value of UTC date kind failed.");
        }

        [TestMethod]
        public void SerializeDeserializeDateXml()
        {
            var localDate = DateTime.Now;
            localDate = new DateTime(localDate.Ticks - (localDate.Ticks % TimeSpan.TicksPerMillisecond), localDate.Kind);
            var utcDate = DateTime.SpecifyKind(localDate, DateTimeKind.Utc);
            var realUtcDate = localDate.ToUniversalTime();
            var testDate = localDate;

#pragma warning disable CS0618 // type or member is obsolete
            var serializer = new ExpressionSerializer(new XmlSerializer());
#pragma warning restore CS0618 // type or member is obsolete

            Expression<Func<DateTime, bool>> localExpression = date => localDate == date && localDate.Kind == date.Kind && localDate.ToUniversalTime() == date.ToUniversalTime() && localDate.ToUniversalTime().Kind == date.ToUniversalTime().Kind;
            Expression<Func<DateTime, bool>> utcExpression = date => utcDate == date && utcDate.Kind == date.Kind && utcDate.ToUniversalTime() == date.ToUniversalTime() && utcDate.ToUniversalTime().Kind == date.ToUniversalTime().Kind;
            Expression<Func<DateTimeKind>> utcKindExpression = () => utcDate.Kind;
            Expression<Func<DateTimeKind>> localKindExpression = () => localDate.Kind;
            Expression<Func<DateTime>> localDateExpression = () => localDate;
            Expression<Func<DateTime>> utcDateExpression = () => utcDate;

            var localValue = serializer.SerializeText(localExpression);
            var utcValue = serializer.SerializeText(utcExpression);
            var utcKindValue = serializer.SerializeText(utcKindExpression);
            var localKindValue = serializer.SerializeText(localKindExpression);
            var localDateValue = serializer.SerializeText(localDateExpression);
            var utcDateValue = serializer.SerializeText(utcDateExpression);

            var actualLocalExpression = (Expression<Func<DateTime, bool>>)serializer.DeserializeText(localValue);
            var actualUtcExpression = (Expression<Func<DateTime, bool>>)serializer.DeserializeText(utcValue);
            var actualLocalKindExpression = (Expression<Func<DateTimeKind>>)serializer.DeserializeText(localKindValue);
            var actualUtcKindExpression = (Expression<Func<DateTimeKind>>)serializer.DeserializeText(utcKindValue);
            var actualLocalDateExpression = (Expression<Func<DateTime>>)serializer.DeserializeText(localDateValue);
            var actualUtcDateExpression = (Expression<Func<DateTime>>)serializer.DeserializeText(utcDateValue);

            var localFunc = actualLocalExpression.Compile();
            var utcFunc = actualUtcExpression.Compile();
            var localKindFunc = actualLocalKindExpression.Compile();
            var utcKindFunc = actualUtcKindExpression.Compile();
            var localDateFunc = actualLocalDateExpression.Compile();
            var utcDateFunc = actualUtcDateExpression.Compile();

            Assert.IsFalse(localDate.ToUniversalTime() == utcDate.ToUniversalTime());
            Assert.IsTrue(localFunc(localDate), "internal comparison of local date failed.");
            Assert.IsTrue(utcFunc(utcDate), "internal comparison of UTC date failed.");
            Assert.IsFalse(localFunc(utcDate), "internal comparison of local vs UTC date failed.");
            Assert.IsFalse(utcFunc(localDate), "internal comparison of UTC vs local date failed.");
            Assert.IsTrue(localKindFunc() == localDate.Kind, "internal comparison of local date kind failed.");
            Assert.IsTrue(utcKindFunc() == utcDate.Kind, "internal comparison of utc date kind failed.");
            Assert.IsTrue(localDateFunc() == localDate, "return value of local date failed.");
            Assert.IsTrue(localDateFunc().Kind == localDate.Kind, "return value of local date kind failed.");
            Assert.IsTrue(utcDateFunc() == utcDate, "return value of UTC date failed.");
            Assert.IsTrue(utcDateFunc().Kind == utcDate.Kind, "return value of UTC date kind failed.");
        }

        [TestMethod]
        public void SerializeDeserializeDateJson()
        {
            var localDate = DateTime.Now;
            localDate = new DateTime(localDate.Ticks - (localDate.Ticks % TimeSpan.TicksPerMillisecond), localDate.Kind);
            var utcDate = DateTime.SpecifyKind(localDate, DateTimeKind.Utc);
            var realUtcDate = localDate.ToUniversalTime();
            var testDate = localDate;

#pragma warning disable CS0618 // type or member is obsolete
            var serializer = new ExpressionSerializer(new JsonSerializer());
#pragma warning restore CS0618 // type or member is obsolete

            Expression<Func<DateTime, bool>> localExpression = date => localDate == date && localDate.Kind == date.Kind && localDate.ToUniversalTime() == date.ToUniversalTime() && localDate.ToUniversalTime().Kind == date.ToUniversalTime().Kind;
            Expression<Func<DateTime, bool>> utcExpression = date => utcDate == date && utcDate.Kind == date.Kind && utcDate.ToUniversalTime() == date.ToUniversalTime() && utcDate.ToUniversalTime().Kind == date.ToUniversalTime().Kind;
            Expression<Func<DateTimeKind>> utcKindExpression = () => utcDate.Kind;
            Expression<Func<DateTimeKind>> localKindExpression = () => localDate.Kind;
            Expression<Func<DateTime>> localDateExpression = () => localDate;
            Expression<Func<DateTime>> utcDateExpression = () => utcDate;

            var localValue = serializer.SerializeText(localExpression);
            var utcValue = serializer.SerializeText(utcExpression);
            var utcKindValue = serializer.SerializeText(utcKindExpression);
            var localKindValue = serializer.SerializeText(localKindExpression);
            var localDateValue = serializer.SerializeText(localDateExpression);
            var utcDateValue = serializer.SerializeText(utcDateExpression);

            var actualLocalExpression = (Expression<Func<DateTime, bool>>)serializer.DeserializeText(localValue);
            var actualUtcExpression = (Expression<Func<DateTime, bool>>)serializer.DeserializeText(utcValue);
            var actualLocalKindExpression = (Expression<Func<DateTimeKind>>)serializer.DeserializeText(localKindValue);
            var actualUtcKindExpression = (Expression<Func<DateTimeKind>>)serializer.DeserializeText(utcKindValue);
            var actualLocalDateExpression = (Expression<Func<DateTime>>)serializer.DeserializeText(localDateValue);
            var actualUtcDateExpression = (Expression<Func<DateTime>>)serializer.DeserializeText(utcDateValue);

            var localFunc = actualLocalExpression.Compile();
            var utcFunc = actualUtcExpression.Compile();
            var localKindFunc = actualLocalKindExpression.Compile();
            var utcKindFunc = actualUtcKindExpression.Compile();
            var localDateFunc = actualLocalDateExpression.Compile();
            var utcDateFunc = actualUtcDateExpression.Compile();

            Assert.IsFalse(localDate.ToUniversalTime() == utcDate.ToUniversalTime());
            Assert.IsTrue(localFunc(localDate), "internal comparison of local date failed.");
            Assert.IsTrue(utcFunc(utcDate), "internal comparison of UTC date failed.");
            Assert.IsFalse(localFunc(utcDate), "internal comparison of local vs UTC date failed.");
            Assert.IsFalse(utcFunc(localDate), "internal comparison of UTC vs local date failed.");
            Assert.IsTrue(localKindFunc() == localDate.Kind, "internal comparison of local date kind failed.");
            Assert.IsTrue(utcKindFunc() == utcDate.Kind, "internal comparison of utc date kind failed.");
            Assert.IsTrue(localDateFunc() == localDate, "return value of local date failed.");
            // the next assert fails for version 2.0.0.0, all DateTime values are giving back with Kind 'UTC'
            Assert.IsTrue(localDateFunc().Kind == localDate.Kind, "return value of local date kind failed.");
            Assert.IsTrue(utcDateFunc() == utcDate, "return value of UTC date failed.");
            Assert.IsTrue(utcDateFunc().Kind == utcDate.Kind, "return value of UTC date kind failed.");
        }
    }
}