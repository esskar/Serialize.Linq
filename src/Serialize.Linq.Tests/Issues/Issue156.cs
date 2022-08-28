using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Serialize.Linq.Factories;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.Issues
{
    // https://github.com/esskar/Serialize.Linq/issues/156
    [TestClass]
    public class Issue156
    {
        public enum SomeEnum
        {
            One = 1,
            Two = 2,
            Three = 3
        }

        public record SomeObject(SomeEnum Value);

        private readonly IReadOnlyCollection<SomeEnum> values = new[] { SomeEnum.One, SomeEnum.Two };

        private static bool SomeFunc(IReadOnlyCollection<SomeEnum> values, SomeObject @object) => values.Contains(@object.Value);

        [TestMethod]
        public void Reproduce()
        {
            Expression<Func<SomeObject, bool>> expression = x => SomeFunc(values, x);

            var serializer = new ExpressionSerializer(new JsonSerializer());
            serializer.AddKnownType(typeof(SomeEnum[]));

            serializer.SerializeText(expression, new FactorySettings
            {
                AllowPrivateFieldAccess = true
            });
        }
    }
}
