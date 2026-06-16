using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.Issues
{
    /// <summary>
    /// https://github.com/esskar/Serialize.Linq/issues/178
    /// Adds the ability to pass an <see cref="ISerializationSurrogateProvider"/> to the underlying
    /// data contract serializer so callers can customize serialization / add support for unknown types.
    ///
    /// Note: the JSON serializer does not honor the surrogate provider on all runtimes
    /// (see https://github.com/dotnet/runtime/issues/100553), so these tests exercise the XML serializer.
    /// </summary>
    [TestClass]
    public class Issue178
    {
        /// <summary>
        /// A surrogate provider that does not substitute anything but records that it was queried,
        /// proving the provider is actually wired into the data contract serializer.
        /// </summary>
        private sealed class RecordingSurrogateProvider : ISerializationSurrogateProvider
        {
            public int GetSurrogateTypeCalls { get; private set; }

            public Type GetSurrogateType(Type type)
            {
                GetSurrogateTypeCalls++;
                return type;
            }

            public object GetObjectToSerialize(object obj, Type targetType) => obj;

            public object GetDeserializedObject(object obj, Type targetType) => obj;
        }

        [TestMethod]
        public void SurrogateProviderIsInvokedDuringXmlRoundTrip()
        {
            var provider = new RecordingSurrogateProvider();
            var xmlSerializer = new XmlSerializer
            {
                SerializationSurrogateProvider = provider
            };
            var serializer = new ExpressionSerializer(xmlSerializer);

            Expression<Func<int, bool>> expression = x => x > 5;

            var text = serializer.SerializeText(expression);
            var actual = (Expression<Func<int, bool>>)serializer.DeserializeText(text);

            Assert.IsTrue(provider.GetSurrogateTypeCalls > 0, "surrogate provider was not invoked.");

            var func = actual.Compile();
            Assert.IsTrue(func(6));
            Assert.IsFalse(func(4));
        }

        [TestMethod]
        public void NoSurrogateProviderStillRoundTrips()
        {
            var serializer = new ExpressionSerializer(new XmlSerializer());

            Expression<Func<int, bool>> expression = x => x > 5;

            var text = serializer.SerializeText(expression);
            var actual = (Expression<Func<int, bool>>)serializer.DeserializeText(text);

            var func = actual.Compile();
            Assert.IsTrue(func(6));
            Assert.IsFalse(func(4));
        }
    }
}
