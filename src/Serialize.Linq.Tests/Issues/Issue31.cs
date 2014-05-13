using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.Issues
{
    // https://github.com/esskar/Serialize.Linq/issues/31
    [TestClass]
    public class Issue31
    {
        [TestMethod]
        public void SerializeLambdaWithEnumTest()
        {
            foreach (var serializer in new ITextSerializer[] { new JsonSerializer(), new XmlSerializer() })
            {
                serializer.AddKnownType(typeof(Gender));

                var expressionSerializer = new ExpressionSerializer(serializer);                
                var fish = new[]
                {
                    new ItemWithEnum {Gender = Gender.Male},
                    new ItemWithEnum {Gender = Gender.Female},
                    new ItemWithEnum(),
                    new ItemWithEnum {Gender = Gender.Female}
                };
                var some = Gender.Female;
                Expression<Func<ItemWithEnum, bool>> expectedExpression = f => f.Gender == some;
                var expected = fish.Where(expectedExpression.Compile()).Count();

                var serialized = expressionSerializer.SerializeText(expectedExpression); // throws SerializationException
                var actualExpression = (Expression<Func<ItemWithEnum, bool>>)expressionSerializer.DeserializeText(serialized);
                var actual = fish.Where(actualExpression.Compile()).Count();

                Assert.AreEqual(expected, actual);
            }
        }

        private class ItemWithEnum
        {
            public Gender Gender { get; set; }
        }

        private enum Gender
        {
            Male,
            Female
        }
    }
}
