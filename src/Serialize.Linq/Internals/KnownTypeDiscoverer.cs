using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Internals
{
    /// <summary>
    /// Walks a serialized <see cref="Node"/> tree and collects the runtime types of constant values so
    /// they can be registered as known types with the underlying data contract serializer. This is what
    /// allows expressions that capture custom types or enum values to be serialized without the caller
    /// having to call <c>AddKnownType</c> manually for every such type.
    /// </summary>
    internal static class KnownTypeDiscoverer
    {
        public static IEnumerable<Type> Discover(Node root)
        {
            var result = new HashSet<Type>();
            if (root == null)
                return result;

            VisitNode(root, result, new HashSet<Node>(ReferenceComparer.Instance), new HashSet<Type>());
            return result;
        }

        private static void VisitNode(Node node, HashSet<Type> result, HashSet<Node> visitedNodes, HashSet<Type> visitedTypes)
        {
            if (node == null || !visitedNodes.Add(node))
                return;

            // A constant's value is the only member declared as object, so it is the one place the data
            // contract serializer needs a polymorphic type registered. We read its runtime type but never
            // enumerate the value itself, which could execute an IQueryable or other lazy sequence.
            if (node is ConstantExpressionNode constant)
                CollectType(constant.Value?.GetType(), result, visitedTypes);

            foreach (var property in node.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.GetIndexParameters().Length > 0)
                    continue;

                // Only follow members that are structurally part of the node tree. Reading other members
                // (e.g. the constant value) is intentionally avoided here to prevent side effects.
                if (typeof(Node).IsAssignableFrom(property.PropertyType))
                {
                    VisitNode((Node)GetValue(property, node), result, visitedNodes, visitedTypes);
                }
                else
                {
                    var elementType = GetNodeElementType(property.PropertyType);
                    if (elementType == null)
                        continue;

                    if (GetValue(property, node) is IEnumerable sequence)
                    {
                        foreach (var item in sequence)
                            VisitNode(item as Node, result, visitedNodes, visitedTypes);
                    }
                }
            }
        }

        private static object GetValue(PropertyInfo property, Node node)
        {
            try
            {
                return property.GetValue(node);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the element type of a property that is an array or generic sequence of nodes, or null
        /// when the property is not a node collection.
        /// </summary>
        private static Type GetNodeElementType(Type type)
        {
            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                return elementType != null && typeof(Node).IsAssignableFrom(elementType) ? elementType : null;
            }

            foreach (var candidate in new[] { type }.Concat(type.GetInterfaces()))
            {
                if (!candidate.IsGenericType || candidate.GetGenericTypeDefinition() != typeof(IEnumerable<>))
                    continue;

                var elementType = candidate.GetGenericArguments()[0];
                if (typeof(Node).IsAssignableFrom(elementType))
                    return elementType;
            }

            return null;
        }

        /// <summary>
        /// Adds a type and its array element / generic argument types to the result set, skipping the
        /// built-in known types. The type structure is inspected statically; the value is never enumerated.
        /// </summary>
        private static void CollectType(Type type, HashSet<Type> result, HashSet<Type> visitedTypes)
        {
            if (type == null || !visitedTypes.Add(type))
                return;

            if (!KnownTypes.IsBuiltIn(type))
                result.Add(type);

            if (type.IsArray)
            {
                CollectType(type.GetElementType(), result, visitedTypes);
            }
            else if (type.IsGenericType)
            {
                foreach (var argument in type.GetGenericArguments())
                    CollectType(argument, result, visitedTypes);
            }
        }

        private sealed class ReferenceComparer : IEqualityComparer<Node>
        {
            public static readonly ReferenceComparer Instance = new ReferenceComparer();

            public bool Equals(Node x, Node y) => ReferenceEquals(x, y);

            public int GetHashCode(Node obj) => RuntimeHelpers.GetHashCode(obj);
        }
    }
}
