﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.IssuesGeneric
{
    /// <summary>
    /// https://github.com/esskar/Serialize.Linq/issues/56
    /// </summary>
    [TestClass]
    public class Issue56
    {
        [TestMethod]
        public void SerializeContainsWithNullablesAndWithoutNullableKey()
        {
            var enterpriseKeys = new List<long?> {1, 2, 3, 4};

            var predicatePart =
                (Expression<Func<GroupEntityWithoutNullable, bool>>)
                    (groupType =>
                        enterpriseKeys.Contains(groupType.GroupEnterpriseKey));

            var serializeTo = new XmlSerializer()
            {
                AutoAddKnownTypesCollectionType = AutoAddCollectionTypes.AsList
            };
            var predicatePartSerializedToString = serializeTo.SerializeGeneric(predicatePart);

            Assert.IsNotNull(predicatePartSerializedToString);
        }

        [TestMethod]
        public void SerializeContainsWithNullablesAndWithNullableKey()
        {
            var enterpriseKeys = new List<long?> { 1, 2, 3, 4 };

            var predicatePart =
                (Expression<Func<GroupEntityWithNullable, bool>>)
                    (groupType =>
                        enterpriseKeys.Contains(groupType.GroupEnterpriseKey));

            var serializeTo = new XmlSerializer()
            {
                AutoAddKnownTypesCollectionType = AutoAddCollectionTypes.AsList
            };
            var predicatePartSerializedToString = serializeTo.SerializeGeneric(predicatePart);

            Assert.IsNotNull(predicatePartSerializedToString);
        }

        class GroupEntityWithoutNullable
        {
            public long GroupEnterpriseKey { get; set; }
        }

        class GroupEntityWithNullable
        {
            public long? GroupEnterpriseKey { get; set; }
        }
    }
}
