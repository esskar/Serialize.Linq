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

            Expression<Func<DateTime>> localDateExpression = () => localDate;
            Expression<Func<DateTime>> utcDateExpression = () => utcDate;

            var localDateValue = serializer.SerializeGeneric(localDateExpression);
            var utcDateValue = serializer.SerializeGeneric(utcDateExpression);

            var actualLocalDateExpression = (Expression<Func<DateTime>>)serializer.DeserializeGeneric(localDateValue);
            var actualUtcDateExpression = (Expression<Func<DateTime>>)serializer.DeserializeGeneric(utcDateValue);

            var localDateFunc = actualLocalDateExpression.Compile();
            var utcDateFunc = actualUtcDateExpression.Compile();

            Assert.IsFalse(localDate.ToUniversalTime() == utcDate.ToUniversalTime() && TimeZoneInfo.Local.GetUtcOffset(localDate) != TimeZoneInfo.Utc.GetUtcOffset(utcDate));
            Assert.IsTrue(localDateFunc() == localDate, "return value of local date failed.");
            // the next assert fails for JsonSerializer of version 2.0.0.0, all DateTime values are giving back with Kind 'UTC'
            Assert.IsTrue(localDateFunc().Kind == localDate.Kind, "return value of local date kind failed.");
            Assert.IsTrue(utcDateFunc() == utcDate, "return value of UTC date failed.");
            Assert.IsTrue(utcDateFunc().Kind == utcDate.Kind, "return value of UTC date kind failed.");
            Assert.IsTrue(localDateFunc() == utcDate, "return value of local vs UTC date failed.");
            // the next assert fails for JsonSerializer of version 2.0.0.0, all DateTime values are giving back with Kind 'UTC'
            Assert.IsFalse(localDateFunc().Kind == utcDate.Kind, "return value of local vs UTC date kind failed.");
        }
    }
}