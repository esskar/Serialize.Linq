using System;
using System.Linq.Expressions;
using Serialize.Linq.Examples.NetStandardLibrary;
using Serialize.Linq.Extensions;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Examples.NetCoreApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            // No need to do this : ExpressionExtensions.AssemblyLoader = new NetCoreAppAssemblyLoader();

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