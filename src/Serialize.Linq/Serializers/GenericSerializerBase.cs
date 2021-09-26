﻿using Serialize.Linq.Extensions;
using Serialize.Linq.Factories;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Nodes;
using System;
using System.Linq.Expressions;

namespace Serialize.Linq.Serializers
{
    public abstract class GenericSerializerBase<TSerialize> : DataSerializer, IGenericSerializer<TSerialize>
    {
        protected GenericSerializerBase(FactorySettings factorySettings = null)
        {
            FactorySettings = factorySettings;
        }

        public abstract bool CanSerializeBinary { get; }

        public abstract bool CanSerializeText { get; }
        
        public FactorySettings FactorySettings { get; }

        public TSerialize SerializeGeneric(Expression expression, FactorySettings factorySettings = null)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            return Serialize(expression.ToExpressionNode(factorySettings ?? this.FactorySettings));
        }

        public Expression DeserializeGeneric(TSerialize data, IExpressionContext context = null)
        {
            return Deserialize<ExpressionNode>(data)?.ToExpression(context ?? new ExpressionContext(false));
        }

        public abstract TSerialize Serialize<TNode>(TNode obj) where TNode : Node;

        public abstract TNode Deserialize<TNode>(TSerialize data) where TNode : Node;
    }
}