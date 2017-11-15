using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Extensions;
using Serialize.Linq.Nodes;

namespace Serialize.Linq.Universal.Tests.Issues
{
    // https://github.com/esskar/Serialize.Linq/issues/86
    [TestClass]
    public class Issue86
    {
        [TestMethod]
        public void Test1()
        {
            Expression<Predicate<SoftwareComponentFilter>> expressionIn = obj 
                => (obj.Category == Category.Application 
                || obj.Category == Category.Method) 
                    && obj.Criticality == Criticality.Urgent 
                    || obj.ReleaseId == "#########";
            var expressionNode = expressionIn.ToExpressionNode();

            var expressionOut = expressionNode.ToExpression<Predicate<SoftwareComponentFilter>>();
            var predicate = expressionOut.Compile();

            var res = predicate(new SoftwareComponentFilter("#############", ComponentType.Bundle, Criticality.Urgent, Category.Method));
            Assert.IsTrue(res);
        }

        [DataContract(Name = "Criticality")]
        public enum Criticality
        {
            [EnumMember(Value = "Undefined")] Undefined = 0,
            [EnumMember(Value = "Recommended")] Recommended,
            [EnumMember(Value = "Urgent")] Urgent,
            [EnumMember(Value = "Optional")] Optional
        }



        [DataContract(Name = "ComponentTypeValue")]
        public struct ComponentTypeValue
        {
            [DataMember(Name = "Value")] private readonly string _Value;

            public ComponentTypeValue(string value)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                if (!value.All(Char.IsUpper))
                {
                    throw new ArgumentException(
                        $@"Component type value '{value}' does not match expected pattern - upper case characters");
                }
                _Value = value;
            }

            public override int GetHashCode()
            {
                return _Value.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return obj is ComponentTypeValue categoryValue && _Value == categoryValue._Value;
            }

            public override string ToString()
            {
                return _Value;
            }

            public static implicit operator string(ComponentTypeValue categoryValue)
            {
                return categoryValue._Value;
            }

            public static explicit operator ComponentTypeValue(string value)
            {
                return new ComponentTypeValue(value);
            }

            public static bool operator ==(ComponentTypeValue left, ComponentTypeValue right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(ComponentTypeValue left, ComponentTypeValue right)
            {
                return !Equals(left, right);
            }
        }

        [DataContract(Name = "ComponentType")]
        public struct ComponentType
        {
            private static readonly Dictionary<ComponentTypeValue, ComponentType> _ComponentTypeValueToComponentTypeMap;

            public static readonly ComponentType Bundle = new ComponentType(new ComponentTypeValue("SBDL"), "Bundle");
            public static readonly ComponentType Single = new ComponentType(new ComponentTypeValue("SNGL"), "Single");

            private ComponentTypeValue _Value;
            private string _Name;

            [DataMember(Name = "Value")]
            public ComponentTypeValue Value
            {
                get => _Value;
                private set => _Value = value;
            }

            [DataMember(Name = "Name")]
            public string Name
            {
                get => _Name;
                private set => _Name = value;
            }

            static ComponentType()
            {
                Type componentTypeType = typeof(ComponentType);
                _ComponentTypeValueToComponentTypeMap = componentTypeType
                    .GetFields(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public)
                    .Where(item => item.FieldType == componentTypeType)
                    .Select(item => item.GetValue(null))
                    .Cast<ComponentType>()
                    .ToDictionary(item => item.Value, item => item);
            }

            private ComponentType(ComponentTypeValue value, string name)
            {
                _Value = value;
                _Name = name;
            }

            public override int GetHashCode()
            {
                return _Value.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return obj is ComponentType componentType && _Value == componentType._Value;
            }

            public override string ToString()
            {
                return _Value;
            }

            public static bool operator ==(ComponentType left, ComponentType right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(ComponentType left, ComponentType right)
            {
                return !Equals(left, right);
            }

            public static ComponentType Create(ComponentTypeValue componentTypeValue)
            {
                if (TryCreate(componentTypeValue, out ComponentType componentType))
                {
                    return componentType;
                }
                else
                {
                    throw new ArgumentException(
                        $@"Unable to map component type value '{componentTypeValue}' to a ComponentType");
                }
            }

            public static bool TryCreate(ComponentTypeValue componentTypeValue, out ComponentType componentType)
            {
                return _ComponentTypeValueToComponentTypeMap.TryGetValue(componentTypeValue, out componentType);
            }
        }

        [DataContract(Name = "Category")]
        public struct Category
        {
            private static readonly Dictionary<CategoryValue, Category> _CategoryValueToCategoryMap;

            public static readonly Category Application = new Category(new CategoryValue("AP"), "Application");
            public static readonly Category Method = new Category(new CategoryValue("ME"), "Method");

            private CategoryValue _Value;
            private string _Name;

            [DataMember(Name = "Value")]
            public CategoryValue Value
            {
                get => _Value;
                private set => _Value = value;
            }

            [DataMember(Name = "Name")]
            public string Name
            {
                get => _Name;
                private set => _Name = value;
            }

            static Category()
            {
                Type categoryType = typeof(Category);
                _CategoryValueToCategoryMap = categoryType
                    .GetFields(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public)
                    .Where(item => item.FieldType == categoryType)
                    .Select(item => item.GetValue(null))
                    .Cast<Category>()
                    .ToDictionary(item => item.Value, item => item);
            }

            private Category(CategoryValue value, string name)
            {
                _Value = value;
                _Name = name;
            }

            public override int GetHashCode()
            {
                return _Value.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return obj is Category category && _Value == category._Value;
            }

            public override string ToString()
            {
                return _Value;
            }

            public static bool operator ==(Category left, Category right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(Category left, Category right)
            {
                return !Equals(left, right);
            }

            public static Category Create(CategoryValue categoryValue)
            {
                if (TryCreate(categoryValue, out Category category))
                {
                    return category;
                }
                else
                {
                    throw new ArgumentException($@"Unable to map category value '{categoryValue}' to a Category");
                }
            }

            public static bool TryCreate(CategoryValue categoryValue, out Category category)
            {
                return _CategoryValueToCategoryMap.TryGetValue(categoryValue, out category);
            }

            [DataContract(Name = "CategoryValue")]
            public struct CategoryValue
            {
                [DataMember(Name = "Value")] private readonly string _Value;

                public CategoryValue(string value)
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException(nameof(value));
                    }
                    if (value.Length != 2 || !value.All(Char.IsUpper))
                    {
                        throw new ArgumentException(
                            $@"Category value '{value}' does not match expected pattern - 2 upper case characters");
                    }
                    _Value = value;
                }

                public override int GetHashCode()
                {
                    return _Value.GetHashCode();
                }

                public override bool Equals(object obj)
                {
                    return obj is CategoryValue categoryValue && _Value == categoryValue._Value;
                }

                public override string ToString()
                {
                    return _Value;
                }

                public static implicit operator string(CategoryValue categoryValue)
                {
                    return categoryValue._Value;
                }

                public static explicit operator CategoryValue(string value)
                {
                    return new CategoryValue(value);
                }

                public static bool operator ==(CategoryValue left, CategoryValue right)
                {
                    return Equals(left, right);
                }

                public static bool operator !=(CategoryValue left, CategoryValue right)
                {
                    return !Equals(left, right);
                }
            }
        }

        public sealed class SoftwareComponentFilter
        {
            public string ReleaseId { get; }
            public ComponentType ComponentType { get; }
            public Criticality Criticality { get; }
            public Category Category { get; }

            public SoftwareComponentFilter(string releaseId, ComponentType componentType, Criticality criticality,
                Category category)
            {
                ReleaseId = releaseId;
                ComponentType = componentType;
                Criticality = criticality;
                Category = category;
            }
        }
    }
}
