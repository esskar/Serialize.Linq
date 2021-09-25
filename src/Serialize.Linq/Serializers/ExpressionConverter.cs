using System.Linq;
using System.Linq.Expressions;
using Serialize.Linq.Factories;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Internals;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Serializers
{
    public class ExpressionConverter
    {
        public ExpressionConverter() { }

        public ExpressionNode Convert(Expression expression, FactorySettings factorySettings = null)
        {
            expression = ExpressionCompressor.Compress(expression);

            var factory = this.CreateFactory(expression, factorySettings);
            return factory.Create(expression);
        }

        protected virtual INodeFactory CreateFactory(Expression expression, FactorySettings factorySettings)
        {
            if (expression is LambdaExpression lambda)
                return new DefaultNodeFactory(lambda.Parameters.Select(p => p.Type), factorySettings);
            return new NodeFactory(factorySettings);
        }
    }
}
