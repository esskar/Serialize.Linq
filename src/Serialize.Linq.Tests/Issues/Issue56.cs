﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.Issues
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

#pragma warning disable CS0618 // type or member is obsolete
            var serializeTo = new ExpressionSerializer(new XmlSerializer())
#pragma warning restore CS0618 // type or member is obsolete
            {
                AutoAddKnownTypesAsListTypes = true
            };
            var predicatePartSerializedToString = serializeTo.SerializeText(predicatePart);

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

#pragma warning disable CS0618 // type or member is obsolete
            var serializeTo = new ExpressionSerializer(new XmlSerializer())
#pragma warning restore CS0618 // type or member is obsolete
            {
                AutoAddKnownTypesAsListTypes = true
            };
            var predicatePartSerializedToString = serializeTo.SerializeText(predicatePart);

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
