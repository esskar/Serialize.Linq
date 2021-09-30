﻿using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.NewTests
{
    /// <summary>
    /// https://github.com/esskar/Serialize.Linq/issues/105
    /// Test and fix provided by https://github.com/oahrens
    /// </summary>
    [TestClass]
    public class TimeSpanTestsGeneric
    {

        [TestMethod]
        public void SerializeDeserializeTimeSpan()
        {
            SerializeDeserializeTimeSpanInternal(new BinarySerializer());
            SerializeDeserializeTimeSpanInternal(new XmlSerializer());
            SerializeDeserializeTimeSpanInternal(new JsonSerializer());
        }

        private static void SerializeDeserializeTimeSpanInternal<T>(IGenericSerializer<T> serializer)
        {
            var now = DateTime.Now;
            var span = TimeSpan.FromSeconds(1);
            Expression<Func<DateTime, bool>> expression = date => date + span > now;
            if (serializer is JsonSerializer)
            {
                // prevents MissingMethodException in version 2.0.0.0; no longer needed in the new version:
                // ValueConverter.AddCustomConverter(typeof(TimeSpan), (object inSpan, Type type) => XmlConvert.ToTimeSpan((string)inSpan));
            }
            var value = serializer.SerializeGeneric(expression);
            var actualExpression = (Expression<Func<DateTime, bool>>)serializer.DeserializeGeneric(value);
            var func = actualExpression.Compile();

            // in case of JsonSerializer the next line throws a MissingMethodException for version 2.0.0.0 if 'prevents' line is commented out
            Assert.IsTrue(func(now), "TimeSpan failed.");
        }
    }
}