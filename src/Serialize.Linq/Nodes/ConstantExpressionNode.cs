#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;
#if NETSTANDARD || WINDOWS_UWP
using System.Reflection;
#endif
using Serialize.Linq.Exceptions;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Internals;

namespace Serialize.Linq.Nodes
{
    #region DataContract
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
    [DataContract]
#else
    [DataContract(Name = "C")]   
#endif
#if !WINDOWS_UWP
    [Serializable]
#endif
    #endregion
    public class ConstantExpressionNode : ExpressionNode<ConstantExpression>
    {
        private object _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantExpressionNode"/> class.
        /// </summary>
        public ConstantExpressionNode() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantExpressionNode"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="value">The value.</param>
        public ConstantExpressionNode(INodeFactory factory, object value)
            : this(factory, value, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantExpressionNode" /> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        public ConstantExpressionNode(INodeFactory factory, object value, Type type)
            : base(factory, ExpressionType.Constant)
        {
            Value = value;
            if (type != null)
                base.Type = factory.Create(type);

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantExpressionNode"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="expression">The expression.</param>
        public ConstantExpressionNode(INodeFactory factory, ConstantExpression expression)
            : base(factory, expression) { }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        /// <exception cref="InvalidTypeException"></exception>
        public override TypeNode Type
        {
            get { return base.Type; }
            set
            {
                if (Value != null)
                {
                    if (value == null)
                    {
                        value = Factory.Create(Value.GetType());
                    }
                    else
                    {
                        var context = new ExpressionContext();
                        if (!value.ToType(context).IsInstanceOfType(Value))
                            throw new InvalidTypeException(
                                string.Format("Type '{0}' is not an instance of the current value type '{1}'.",
                                    value.ToType(context), Value.GetType()));
                    }
                }
                base.Type = value;
            }
        }

        #region DataMember
#if !SERIALIZE_LINQ_OPTIMIZE_SIZE
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        /// <exception cref="System.ArgumentException">Expression not allowed.;value</exception>
        [DataMember(EmitDefaultValue = false)]
#else
        [DataMember(EmitDefaultValue = false, Name = "V")]
#endif
        #endregion
        public object Value
        {
            get { return _value; }
            set
            {
                if (value is Expression)
                    throw new ArgumentException("Expression not allowed.", "value");

                var valueType = value as Type;
                _value = valueType != null ? Factory.Create(valueType) : value;

                if (_value == null || _value is TypeNode) 
                    return;

                var type = base.Type != null ? base.Type.ToType(new ExpressionContext()) : null;
                if (type == null)
                {
                    if (Factory != null)
                        base.Type = Factory.Create(_value.GetType());
                    return;
                }
                _value = ValueConverter.Convert(_value, type);
            }
        }

        /// <summary>
        /// Initializes the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        protected override void Initialize(ConstantExpression expression)
        {
            Value = expression.Value;
        }

        /// <summary>
        /// Converts this instance to an expression.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override Expression ToExpression(IExpressionContext context)
        {
            var typeNode = Value as TypeNode;
            if (typeNode != null)
                return Expression.Constant(typeNode.ToType(context), Type.ToType(context));
            return Type != null 
                ? Expression.Constant(Value, Type.ToType(context)) 
                : Expression.Constant(Value);
        }
    }
}