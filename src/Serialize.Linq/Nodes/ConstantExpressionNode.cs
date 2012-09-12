using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Serialize.Linq.Exceptions;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Nodes
{
    [DataContract]   
    public class ConstantExpressionNode : ExpressionNode<ConstantExpression>
    {
        private object _value;

        public ConstantExpressionNode(object value)
            : base(ExpressionType.Constant)
        {
            this.Value = value;
        }

        public ConstantExpressionNode(ConstantExpression expression)
            : base(expression) {}

        public ConstantExpressionNode(INodeFactory factory, ConstantExpression expression) 
            : base(factory, expression) { }
        
        public override TypeNode Type
        {
            get { return base.Type; }
            set
            {
                if(this.Value != null)
                {
                    if (value == null)
                        value = this.Factory.Create(this.Value.GetType());
                    else if (!value.ToType().IsInstanceOfType(this.Value))
                        throw new InvalidTypeException(string.Format("Type '{0}' is not an instance of the current value type '{1}'.", value.ToType(), this.Value.GetType()));
                }
                base.Type = value;                
            }
        }

        [DataMember]
        public object Value 
        {
            get { return _value; }
            set
            {
                if(value is Expression)
                    throw new ArgumentException("Expression not allowed.", "value");            
                _value = value;
                if (_value != null)
                {
                    var type = base.Type != null ? base.Type.ToType() : null;
                    if (type == null || !type.IsInstanceOfType(_value))
                        base.Type = this.Factory.Create(_value.GetType());
                }
            }
        }

        protected override void Initialize(ConstantExpression expression)
        {
            this.Value = expression.Value;
        }
        
        public override Expression ToExpression()
        {
            return this.Type != null ? Expression.Constant(this.Value, this.Type.ToType()) : Expression.Constant(this.Value);
        }        
    }
}
