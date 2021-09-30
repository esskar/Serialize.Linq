using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.Issues
{
#pragma warning disable CA1822 // mark members as static
    // https://github.com/esskar/Serialize.Linq/issues/113
    [TestClass]
    public class Issue113
    {
        [TestMethod]
        public void AllowVariableInsideTryCatch()
        {
            var tc = new TestClass();
            tc.VariableInsideTryCatch("foo");
        }

        [TestMethod]
        public async Task AllowVariableInsideTryCatchAsync()
        {
            var tc = new TestClass();
            await tc.VariableInsideTryCatchAsync("foo");
        }

        [TestMethod]
        public void AllowConstInsideTryCatch()
        {
            var tc = new TestClass();
            tc.ConstInsideTryCatch("foo");
        }

        [TestMethod]
        public async Task AllowConstInsideTryCatchAsync()
        {
            var tc = new TestClass();
            await tc.ConstInsideTryCatchAsync("foo");
        }

        public class TestClass
        {
            public void VariableInsideTryCatch(string productId)
            {
                try
                {
                    var userId = "12313";
                    DoSerialization<ProductTest>(x => x.UserId == userId && x.ProductId == productId);
                }
                catch (Exception ex)
                {
                    Assert.Fail("Exception thrown: {0}", ex);
                }
            }

            public async Task VariableInsideTryCatchAsync(string productId)
            {
                await Task.Yield();

                try
                {
                    var userId = "12313";
                    DoSerialization<ProductTest>(x => x.UserId == userId && x.ProductId == productId);
                }
                catch (Exception ex)
                {
                    Assert.Fail("Exception thrown: {0}", ex);
                }
            }

            public void ConstInsideTryCatch(string productId)
            {
                try
                {
                    const string userId = "12313";
                    DoSerialization<ProductTest>(x => x.UserId == userId && x.ProductId == productId);
                }
                catch (Exception ex)
                {
                    Assert.Fail("Exception thrown: {0}", ex);
                }
            }

            public async Task ConstInsideTryCatchAsync(string productId)
            {
                await Task.Yield();

                try
                {
                    const string userId = "12313";
                    DoSerialization<ProductTest>(x => x.UserId == userId && x.ProductId == productId);
                }
                catch (Exception ex)
                {
                    Assert.Fail("Exception thrown: {0}", ex);
                }
            }

            private static void DoSerialization<T>(Expression<Func<T, bool>> predicate)
            {
#pragma warning disable CS0618 // type or member is obsolete
                var expressionSerializer = new ExpressionSerializer(new JsonSerializer());
#pragma warning restore CS0618 // type or member is obsolete
                expressionSerializer.SerializeText(predicate);
            }
        }

        public class ProductTest
        {
            public string ProductId { get; set; }

            public string UserId { get; set; }
        }

        public class UserTest
        {
            public string Email { get; set; }
            public string UserId { get; set; }
        }
    }
#pragma warning restore CA1822 // mark members as static
}
