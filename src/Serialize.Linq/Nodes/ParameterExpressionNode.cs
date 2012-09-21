using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "P")]
#endif
    #endregion
    public class ParameterExpressionNode : ExpressionNode<ParameterExpression>
    {
        private ParameterExpression _parameterExpression;
        private string _name;
        private bool _isByRef;

        public ParameterExpressionNode(INodeFactory factory, ParameterExpression expression)
            : base(factory, expression) { }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "I")]
#endif
        #endregion
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

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "N")]
#endif
        #endregion
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
            return _parameterExpression ?? (_parameterExpression = Expression.Parameter(this.Type.ToType(), this.Name));
        }
    }
}
