using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Extensions;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "L")]
#endif
    #endregion
    public class LambdaExpressionNode : ExpressionNode<LambdaExpression>
    {
        public LambdaExpressionNode(INodeFactory factory, LambdaExpression expression)
            : base(factory, expression) { }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "B")]
#endif
        #endregion
        public ExpressionNode Body { get; set; }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "P")]
#endif
        #endregion
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
            return Expression.Lambda(this.Type.ToType(), body, parameters);
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
