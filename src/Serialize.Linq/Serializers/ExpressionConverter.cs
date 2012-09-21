using System.Linq;
using System.Linq.Expressions;
using Serialize.Linq.Factories;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Serializers
{
    public class ExpressionConverter
    {
        public ExpressionNode Convert(Expression expression)
        {
            var factory = this.CreateFactory(expression);
            return factory.Create(expression);
        }

        protected virtual INodeFactory CreateFactory(Expression expression)
        {
            var lambda = expression as LambdaExpression;
            if(lambda != null)
                return new DefaultNodeFactory(lambda.Parameters.Select(p => p.Type));
            return new NodeFactory();
        }        
    }
}
