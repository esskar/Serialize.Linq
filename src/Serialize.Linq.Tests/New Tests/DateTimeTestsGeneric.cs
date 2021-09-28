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
    public class DateTimeTestsGeneric
    {

        [TestMethod]
        public void SerializeDeserializeDate()
        {
            SerializeDeserializeDateInternal(new BinarySerializer());
            SerializeDeserializeDateInternal(new XmlSerializer());
            SerializeDeserializeDateInternal(new JsonSerializer());
        }

        private static void SerializeDeserializeDateInternal<T>(Interfaces.IGenericSerializer<T> serializer)
        {
            var localDate = DateTime.Now;
            if (serializer is JsonSerializer)
            {
                localDate = new DateTime(localDate.Ticks - (localDate.Ticks % TimeSpan.TicksPerMillisecond), localDate.Kind);
            }
            var utcDate = DateTime.SpecifyKind(localDate, DateTimeKind.Utc);

            Expression<Func<DateTime, bool>> localExpression = date => localDate == date && localDate.Kind == date.Kind && localDate.ToUniversalTime() == date.ToUniversalTime() && localDate.ToUniversalTime().Kind == date.ToUniversalTime().Kind;
            Expression<Func<DateTime, bool>> utcExpression = date => utcDate == date && utcDate.Kind == date.Kind && utcDate.ToUniversalTime() == date.ToUniversalTime() && utcDate.ToUniversalTime().Kind == date.ToUniversalTime().Kind;
            Expression<Func<DateTimeKind>> utcKindExpression = () => utcDate.Kind;
            Expression<Func<DateTimeKind>> localKindExpression = () => localDate.Kind;
            Expression<Func<DateTime>> localDateExpression = () => localDate;
            Expression<Func<DateTime>> utcDateExpression = () => utcDate;

            var localValue = serializer.SerializeGeneric(localExpression);
            var utcValue = serializer.SerializeGeneric(utcExpression);
            var utcKindValue = serializer.SerializeGeneric(utcKindExpression);
            var localKindValue = serializer.SerializeGeneric(localKindExpression);
            var localDateValue = serializer.SerializeGeneric(localDateExpression);
            var utcDateValue = serializer.SerializeGeneric(utcDateExpression);

            var actualLocalExpression = (Expression<Func<DateTime, bool>>)serializer.DeserializeGeneric(localValue);
            var actualUtcExpression = (Expression<Func<DateTime, bool>>)serializer.DeserializeGeneric(utcValue);
            var actualLocalKindExpression = (Expression<Func<DateTimeKind>>)serializer.DeserializeGeneric(localKindValue);
            var actualUtcKindExpression = (Expression<Func<DateTimeKind>>)serializer.DeserializeGeneric(utcKindValue);
            var actualLocalDateExpression = (Expression<Func<DateTime>>)serializer.DeserializeGeneric(localDateValue);
            var actualUtcDateExpression = (Expression<Func<DateTime>>)serializer.DeserializeGeneric(utcDateValue);

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
    }
}