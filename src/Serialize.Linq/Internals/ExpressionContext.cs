using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Internals
{
    internal class ExpressionContext
    {
        private readonly ConcurrentDictionary<string, ParameterExpression> _parameterExpressions;

        public ExpressionContext()
        {
            _parameterExpressions = new ConcurrentDictionary<string, ParameterExpression>();
        }

        public ParameterExpression GetParameterExpression(ParameterExpressionNode node)
        {
            if(node == null)
                throw new ArgumentNullException("node");
            var key = node.Type.Name + Environment.NewLine + node.Name;
            return _parameterExpressions.GetOrAdd(key, k => Expression.Parameter(node.Type.ToType(), node.Name));
        }
    }
}
