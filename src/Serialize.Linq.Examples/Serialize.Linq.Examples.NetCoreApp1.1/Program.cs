using System;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Serialize.Linq.Extensions;
using Serialize.Linq.Factories;
using Serialize.Linq.Serializers;
using JsonSerializer = Serialize.Linq.Serializers.JsonSerializer;

namespace Serialize.Linq.Examples.NetCoreApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Expression expression = Expression.Condition(Expression.Constant(true), Expression.Constant(Formatting.None), Expression.Constant(Formatting.Indented));

            var settings = new FactorySettings
            {
                AllowPrivateFieldAccess = true
            };
            var serializer = new ExpressionSerializer(new JsonSerializer());
            string value = serializer.SerializeText(expression, settings);
            Console.WriteLine("value" + value);

            // Deserialize expression
            var actualExpression = serializer.DeserializeText(value, new ExpressionContext { AllowPrivateFieldAccess = true });
            
            Console.WriteLine("actualExpression" + actualExpression.ToJson());
        }
    }
}