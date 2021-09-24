using Serialize.Linq.Nodes;
using System;
using System.Collections.Generic;

namespace Serialize.Linq
{

    /// <summary>
    ///   ''' Improvement: weniger kostenintensives Dictionary(Of ParameterExpressionNode, ParameterExpression) statt ConcurrentDictionary(Of String, ParameterExpression) mit
    ///   ''' vereinfachter selbstverwaltender Struktur des ExpressionContext._expressions-Dictionarys.
    ///   ''' </summary>
    internal sealed class ParameterExpressionNodeComparer : EqualityComparer<ParameterExpressionNode>
    {
        public override bool Equals(ParameterExpressionNode x, ParameterExpressionNode y)
        {
            return x.Type.Name == y.Type.Name && x.Name == y.Name;
        }

        public override int GetHashCode(ParameterExpressionNode obj)
        {
            return (obj.Name + obj.Type.Name).GetHashCode();
        }
    }
}
