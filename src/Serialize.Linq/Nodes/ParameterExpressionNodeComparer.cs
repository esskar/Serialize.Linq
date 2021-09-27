using System.Collections.Generic;

namespace Serialize.Linq.Nodes
{
    public sealed class ParameterExpressionNodeComparer : EqualityComparer<ParameterExpressionNode>
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
