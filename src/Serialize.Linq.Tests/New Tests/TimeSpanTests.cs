using System;
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
    public class TimeSpanTests
    {

        [TestMethod]
        public void SerializeDeserializeTimeSpan()
        {
            SerializeDeserializeTimeSpanInternal(new BinarySerializer());
            SerializeDeserializeTimeSpanInternal(new XmlSerializer());
            SerializeDeserializeTimeSpanInternal(new JsonSerializer());
        }

        private static void SerializeDeserializeTimeSpanInternal<T>(Interfaces.IGenericSerializer<T> serializer)
        {
            var now = DateTime.Now;
            var span = TimeSpan.FromSeconds(1);

            Expression<Func<DateTime, bool>> expression = date => date + span > now;

            var value = serializer.SerializeGeneric(expression);

            var actualExpression = (Expression<Func<DateTime, bool>>)serializer.DeserializeGeneric(value);

            var func = actualExpression.Compile();

            Assert.IsTrue(func(now), "TimeSpan failed."); // fails in previous version for JsonSerializer
        }
    }
}