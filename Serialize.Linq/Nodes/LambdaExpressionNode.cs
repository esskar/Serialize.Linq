using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Extensions;
using Serialize.Linq.Factories;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    public class LambdaExpressionNode : ExpressionNode<LambdaExpression>
    {
        public LambdaExpressionNode(LambdaExpression expression)
            : base(expression) {}

        public LambdaExpressionNode(IExpressionNodeFactory factory, LambdaExpression expression)
            : base(factory, expression) { }

        [DataMember]
        public ExpressionNode Body { get; set; }

        [DataMember]
        public ExpressionNodeList Parameters { get; set; }

        protected override void Initialize(LambdaExpression expression)
        {
            this.Body = this.Factory.Create(expression.Body);
            this.Parameters = new ExpressionNodeList(this.Factory, expression.Parameters);
        }
        
        public override Expression ToExpression()
        {
            Expression body;
            ParameterExpression[] parameters;
            this.BuildExpression(out body, out parameters);
            return Expression.Lambda(this.Type, body, parameters);
        }

        public Expression<TDelegate> ToExpression<TDelegate>()
        {
            Expression body;
            ParameterExpression[] parameters;
            this.BuildExpression(out body, out parameters);            
            return Expression.Lambda<TDelegate>(body, parameters);
        }

        private void BuildExpression(out Expression bodyExpression, out ParameterExpression[] parameterExpressions)
        {
            bodyExpression = this.Body.ToExpression();

            var parameters = this.Parameters.GetParameterExpressions().ToArray();
            var bodyParameters = bodyExpression.GetNodes().OfType<ParameterExpression>().ToArray();
            for (var i = 0; i < parameters.Length; ++i)
            {
                var matchingParameter = bodyParameters.Where(p => p.Name == parameters[i].Name && p.Type == parameters[i].Type).ToArray();
                if (matchingParameter.Length == 1)
                    parameters[i] = matchingParameter.First();
            }
            
            parameterExpressions = parameters;
        }        
    }
}
