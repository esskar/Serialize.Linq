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
            var realUtcDate = localDate.ToUniversalTime();
            var testDate = localDate;

            Expression<Func<DateTime, bool>> localExpression = date => localDate == date && localDate.Kind == date.Kind && localDate.ToUniversalTime() == date.ToUniversalTime() && localDate.ToUniversalTime().Kind == date.ToUniversalTime().Kind;
            Expression<Func<DateTime, bool>> utcExpression = date => utcDate == date && utcDate.Kind == date.Kind && utcDate.ToUniversalTime() == date.ToUniversalTime() && utcDate.ToUniversalTime().Kind == date.ToUniversalTime().Kind;
            Expression<Func<DateTimeKind>> utcKindExpression = () => utcDate.Kind;
            Expression<Func<DateTimeKind>> localKindExpression = () => localDate.Kind;
            Expression<Func<DateTime>> localStringExpression = () => localDate;

            var localValue = serializer.SerializeGeneric(localExpression);
            var utcValue = serializer.SerializeGeneric(utcExpression);
            var utcKindValue = serializer.SerializeGeneric(utcKindExpression);
            var localKindValue = serializer.SerializeGeneric(localKindExpression);
            var localStringValue = serializer.SerializeGeneric(localStringExpression);

            var actualLocalExpression = (Expression<Func<DateTime, bool>>)serializer.DeserializeGeneric(localValue);
            var actualUtcExpression = (Expression<Func<DateTime, bool>>)serializer.DeserializeGeneric(utcValue);
            var actualLocalKindExpression = (Expression<Func<DateTimeKind>>)serializer.DeserializeGeneric(localKindValue);
            var actualUtcKindExpression = (Expression<Func<DateTimeKind>>)serializer.DeserializeGeneric(utcKindValue);
            var actualLocalStringExpression = (Expression<Func<DateTime>>)serializer.DeserializeGeneric(localStringValue);

            var localFunc = actualLocalExpression.Compile();
            var utcFunc = actualUtcExpression.Compile();
            var localKindFunc = actualLocalKindExpression.Compile();
            var utcKindFunc = actualUtcKindExpression.Compile();
            var localStringFunc = actualLocalStringExpression.Compile();

            Assert.IsFalse(localDate.ToUniversalTime() == utcDate.ToUniversalTime());
            Assert.IsTrue(localFunc(localDate), "local date failed.");
            Assert.IsTrue(utcFunc(utcDate), "UTC date failed.");
            Assert.IsFalse(localFunc(utcDate), "local vs UTC date failed.");
            Assert.IsFalse(utcFunc(localDate), "UTC vs local date failed.");
            Assert.IsTrue(localKindFunc() == localDate.Kind, "local date kind failed.");
            Assert.IsTrue(utcKindFunc() == utcDate.Kind, "utc date kind failed.");
        }
    }
}