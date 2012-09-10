using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Factories;

namespace Serialize.Linq.Nodes
{
    [DataContract]
    public class ParameterExpressionNode : ExpressionNode<ParameterExpression>
    {
        private ParameterExpression _parameterExpression;
        private string _name;
        private bool _isByRef;

        public ParameterExpressionNode(ParameterExpression expression)
            : base(expression) { }

        public ParameterExpressionNode(IExpressionNodeFactory factory, ParameterExpression expression)
            : base(factory, expression) { }

        [DataMember]
        public bool IsByRef
        {
            get { return _isByRef; }
            set
            {
                if (_isByRef != value)
                {
                    _isByRef = value;
                    _parameterExpression = null;
                }
            }
        }

        [DataMember]
        public string Name 
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    _parameterExpression = null;
                }
            }
        }

        protected override void Initialize(ParameterExpression expression)
        {
            this.IsByRef = expression.IsByRef;
            this.Name = expression.Name;
            _parameterExpression = expression;
        }
        
        public override Expression ToExpression()
        {
            return _parameterExpression ?? (_parameterExpression = Expression.Parameter(this.Type, this.Name));
        }        
    }
}
