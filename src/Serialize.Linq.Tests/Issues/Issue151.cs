using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq;
using Serialize.Linq.Serializers;
using Serialize.Linq.TypeFilters;

namespace Serialize.Linq.Tests.Issues
{
    /// <summary>
    /// https://github.com/esskar/Serialize.Linq/issues/151
    /// Allow callers to restrict which types may be resolved during deserialization
    /// (the Serialize.Linq equivalent of BinaryFormatter's SerializationBinder), via an
    /// <see cref="Serialize.Linq.Interfaces.ITypeFilter"/> set on the <see cref="ExpressionContext"/>.
    /// </summary>
    [TestClass]
    public class Issue151
    {
        private sealed class Forbidden
        {
            public int Value { get; set; }
        }

        [TestMethod]
        public void AllowedExpressionRoundTripsWithFilter()
        {
            var serializer = new ExpressionSerializer(new JsonSerializer());

            Expression<Func<int, bool>> expression = x => x > 5;
            var text = serializer.SerializeText(expression);

            var filter = new AllowedTypesFilter(
                typeof(int), typeof(bool), typeof(object), typeof(Func<,>));
            var context = new ExpressionContext(filter);

            var actual = (Expression<Func<int, bool>>)serializer.DeserializeText(text, context);

            var func = actual.Compile();
            Assert.IsTrue(func(6));
            Assert.IsFalse(func(4));
        }

        [TestMethod]
        public void DisallowedTypeIsRejectedDuringDeserialization()
        {
            var serializer = new ExpressionSerializer(new JsonSerializer());

            Expression<Func<Forbidden, bool>> expression = f => f.Value > 5;
            var text = serializer.SerializeText(expression);

            // Allow-list deliberately omits Forbidden.
            var filter = new AllowedTypesFilter(
                typeof(int), typeof(bool), typeof(object), typeof(Func<,>));
            var context = new ExpressionContext(filter);

            var ex = Assert.ThrowsExactly<TypeNotAllowedException>(
                () => serializer.DeserializeText(text, context));
            Assert.AreEqual(typeof(Forbidden), ex.Type);
        }

        [TestMethod]
        public void NamespaceAllowListPermitsMatchingTypes()
        {
            var serializer = new ExpressionSerializer(new JsonSerializer());

            Expression<Func<Forbidden, bool>> expression = f => f.Value > 5;
            var text = serializer.SerializeText(expression);

            var filter = new AllowedTypesFilter(
                    typeof(int), typeof(bool), typeof(object), typeof(Func<,>))
                .AllowNamespace(typeof(Forbidden).Namespace);
            var context = new ExpressionContext(filter);

            var actual = (Expression<Func<Forbidden, bool>>)serializer.DeserializeText(text, context);

            var func = actual.Compile();
            Assert.IsTrue(func(new Forbidden { Value = 6 }));
            Assert.IsFalse(func(new Forbidden { Value = 4 }));
        }

        [TestMethod]
        public void DelegateTypeFilterCanRejectTypes()
        {
            var serializer = new ExpressionSerializer(new JsonSerializer());

            Expression<Func<Forbidden, bool>> expression = f => f.Value > 5;
            var text = serializer.SerializeText(expression);

            // Reject anything declared in this test assembly's namespace.
            var filter = new DelegateTypeFilter(t => t != typeof(Forbidden));
            var context = new ExpressionContext(filter);

            Assert.ThrowsExactly<TypeNotAllowedException>(
                () => serializer.DeserializeText(text, context));
        }

        [TestMethod]
        public void NoFilterAllowsAllTypes()
        {
            var serializer = new ExpressionSerializer(new JsonSerializer());

            Expression<Func<Forbidden, bool>> expression = f => f.Value > 5;
            var text = serializer.SerializeText(expression);

            // Default context: no filter -> everything resolves as before.
            var actual = (Expression<Func<Forbidden, bool>>)serializer.DeserializeText(text, new ExpressionContext());

            var func = actual.Compile();
            Assert.IsTrue(func(new Forbidden { Value = 6 }));
        }

        [TestMethod]
        public void GenericArgumentsAreFiltered()
        {
            var serializer = new ExpressionSerializer(new JsonSerializer());

            Expression<Func<List<Forbidden>, int>> expression = list => list.Count;
            var text = serializer.SerializeText(expression);

            // List<> is allowed but its argument Forbidden is not.
            var filter = new AllowedTypesFilter(
                typeof(int), typeof(object), typeof(Func<,>), typeof(List<>));
            var context = new ExpressionContext(filter);

            Assert.ThrowsExactly<TypeNotAllowedException>(
                () => serializer.DeserializeText(text, context));
        }
    }
}
