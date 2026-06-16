using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Factories;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.Issues
{
    // https://github.com/esskar/Serialize.Linq/issues/158
    [TestClass]
    public class Issue158
    {
        public enum OutputChannelCodes
        {
            Sms,
            Email
        }

        public class OutputChannelLog
        {
            public OutputChannelCodes Channel { get; set; }
        }

        private static OutputChannelLog[] BuildItems() => new[]
        {
            new OutputChannelLog { Channel = OutputChannelCodes.Sms },
            new OutputChannelLog { Channel = OutputChannelCodes.Sms },
            new OutputChannelLog { Channel = OutputChannelCodes.Email },
            new OutputChannelLog { Channel = OutputChannelCodes.Email }
        };

        [TestMethod]
        public void AutoDiscoverKnownTypes_RoundTripsEnumConstant_WithoutManualKnownType()
        {
            foreach (var serializer in new ITextSerializer[] { new JsonSerializer(), new XmlSerializer() })
            {
                serializer.AutoDiscoverKnownTypes = true;
                var expressionSerializer = new ExpressionSerializer(serializer);

                // The captured local becomes an enum-typed constant after closure compression - the exact
                // shape that previously required a manual AddKnownType(typeof(OutputChannelCodes)) call.
                var channel = OutputChannelCodes.Email;
                Expression<Func<OutputChannelLog, bool>> filter = x => x.Channel == channel;

                var text = expressionSerializer.SerializeText(filter);
                var deserialized = (Expression<Func<OutputChannelLog, bool>>)expressionSerializer.DeserializeText(text);

                var count = BuildItems().AsQueryable().Where(deserialized).Count();
                Assert.AreEqual(2, count, $"Serializer: {serializer.GetType().Name}");
            }
        }

        [TestMethod]
        public void AutoDiscoverKnownTypes_SerializesEnumCollectionConstant_WithoutManualKnownType()
        {
            // Runtime type of the captured collection is OutputChannelCodes[]; the discoverer must register
            // both the array and its element type. Compare Issue156, which needed AddKnownType(SomeEnum[])
            // to get past serialization.
            IReadOnlyCollection<OutputChannelCodes> allowed = new[] { OutputChannelCodes.Email };

            var serializer = new JsonSerializer { AutoDiscoverKnownTypes = true };
            var expressionSerializer = new ExpressionSerializer(serializer);

            Expression<Func<OutputChannelLog, bool>> filter = x => allowed.Contains(x.Channel);

            // Without discovery this throws SerializationException ("Error converting type").
            var text = expressionSerializer.SerializeText(filter, new FactorySettings { AllowPrivateFieldAccess = true });
            Assert.IsFalse(string.IsNullOrEmpty(text));
        }

        [TestMethod]
        public void WithoutAutoDiscoverKnownTypes_SerializingEnumConstant_Throws()
        {
            // Documents the historical behaviour (issue #158) that AutoDiscoverKnownTypes resolves.
            var expressionSerializer = new ExpressionSerializer(new JsonSerializer());

            var channel = OutputChannelCodes.Email;
            Expression<Func<OutputChannelLog, bool>> filter = x => x.Channel == channel;

            Assert.ThrowsException<SerializationException>(() => expressionSerializer.SerializeText(filter));
        }
    }
}
