using System;
using System.Linq.Expressions;
using Serialize.Linq.Examples.NetStandardLibrary;
using Serialize.Linq.Extensions;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Examples.NetCoreApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            // This is needed for NETStandard 1.x and NETCoreApp 1.x
            ExpressionExtensions.AssemblyLoader = new NetCoreAppAssemblyLoader();

            Expression expression = Expression.Parameter(typeof(Person), "x");

            // Serialize expression
            var serializer = new ExpressionSerializer(new JsonSerializer());
            string value = serializer.SerializeText(expression);
            Console.WriteLine("value:" + value);

            // Deserialize expression
            var actualExpression = serializer.DeserializeText(value);
            Console.WriteLine("actualExpression:" + actualExpression.ToJson());
        }
    }
}