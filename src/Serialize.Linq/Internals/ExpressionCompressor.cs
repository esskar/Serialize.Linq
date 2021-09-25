using System.Collections.Generic;
using System.Linq.Expressions;

namespace Serialize.Linq.Internals
{
    internal static class ExpressionCompressor
    {
        private static readonly ICollection<ExpressionType> _andOrExpressions = new HashSet<ExpressionType> { ExpressionType.Or, ExpressionType.And, ExpressionType.OrElse, ExpressionType.AndAlso };

        public static Expression Compress(Expression expression)
        {
            if (expression == null)
                return null;

            if (!_andOrExpressions.Contains(expression.NodeType)) 
                return expression;

            var stack = new Stack<Expression>();
            var items = new List<Expression>();
            var binary = (BinaryExpression)expression;

            stack.Push(binary.Right);
            stack.Push(binary.Left);
            while (stack.Count > 0)
            {
                var item = stack.Pop();
                if (item.NodeType == expression.NodeType)
                {
                    binary = (BinaryExpression)item;
                    stack.Push(binary.Right);
                    stack.Push(binary.Left);
                }
                else
                {
                    items.Add(item);
                }
            }

            if (items.Count <= 3)
                return expression;

            // having N items will lead to NxM recursive calls in expression visitors and
            // will result in stack overflow on relatively small numbers (~1000 items).
            // To fix it we will re-balance condition tree here which will result in
            // LOG2(N)*M recursive calls, or 10*M calls for 1000 items.
            //
            // E.g. we have condition A OR B OR C OR D OR E
            // as an expression tree it represented as tree with depth 5
            //   OR
            // A    OR
            //    B    OR
            //       C    OR
            //          D    E
            // for rebalanced tree it will have depth 4
            //                  OR
            //        OR
            //   OR        OR        OR
            // A    B    C    D    E    F
            // Not much on small numbers, but huge improvement on bigger numbers
            while (items.Count != 1)
                items = CompressTree(items, expression.NodeType);

            return items[0];
        }

        private static List<Expression> CompressTree(IList<Expression> items, ExpressionType nodeType)
        {
            var result = new List<Expression>();

            // traverse list from left to right to preserve calculation order
            for (var i = 0; i < items.Count; i += 2)
            {
                result.Add(i + 1 == items.Count
                    ? items[i] : Expression.MakeBinary(nodeType, items[i], items[i + 1]));
            }

            return result;
        }
    }
}
