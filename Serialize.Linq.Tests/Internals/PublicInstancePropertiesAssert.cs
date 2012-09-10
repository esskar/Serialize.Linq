using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Serialize.Linq.Tests.Internals
{
    internal class PublicInstancePropertiesAssert
    {
        private static IEnumerable<PropertyInfo> GetProperties<T>()
        {
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        public static void AreEqual<T>(T expected, T actual, Type attributeType, string message = null) where T : class
        {
            var ignore = new List<string>();
            foreach (var propertyInfo in GetProperties<T>())
            {
                var attributes = propertyInfo.GetCustomAttributes(attributeType, true);
                if (attributes.Length == 0)
                    ignore.Add(propertyInfo.Name);
            }

            AreEqual(expected, actual, message, ignore.ToArray());
        }

        /// <summary>
        /// Checks whether the public properties of two object instances are equal
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expected">The first object instance to be compared.</param>
        /// <param name="actual">The second object instance to be compared.</param>
        /// <param name="message"></param>
        /// <param name="ignore"></param>
        public static void AreEqual<T>(T expected, T actual, string message = null, params string[] ignore) where T : class
        {
            AreEqual<T>(expected, actual, null, message, ignore);
        }

        public static void AreEqual<T>(T expected, T actual, Type deepCompareType, string message = null, params string[] ignore) where T : class
        {
            if (expected != null && actual != null)
            {
                foreach (var propertyInfo in GetProperties<T>())
                {
                    if (ignore.Contains(propertyInfo.Name)) 
                        continue;

                    var expectedValue = typeof(T).GetProperty(propertyInfo.Name).GetValue(expected, null);
                    var actualValue = typeof(T).GetProperty(propertyInfo.Name).GetValue(actual, null);                    
                    var additionalMessage = string.Format("Objects differ in property {0}", propertyInfo.Name);
                    var testMessage = string.IsNullOrWhiteSpace(message) 
                            ? additionalMessage
                            : message + " (" + additionalMessage + ")";
                    if (expectedValue != null && expectedValue.GetType().IsArray)
                        CollectionAssert.AreEqual((Array) expectedValue, (Array) actualValue, testMessage);
                    else
                        Assert.AreEqual(expectedValue, actualValue, testMessage);
                }
            }
            else
            {
                Assert.AreEqual(expected, actual, message);
            }            
        }
    }
}
