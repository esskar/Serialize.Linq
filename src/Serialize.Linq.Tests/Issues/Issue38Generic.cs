﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.Issues
{
    [TestClass]
    public class Issue38Generic
    {
        [TestMethod]
        public void SerializeAsQueryableWithPredicateTest()
        {
            Expression<Func<Order, bool>> predicate = x => x.Id > 0 && x.Id < 5;

            Expression<Func<Document, bool>> pred = x => x.Orders.AsQueryable().Any(predicate);

            var serializer = new BinarySerializer();
            var value = serializer.SerializeGeneric(pred);

            Assert.IsNotNull(value);
        }

        [TestMethod]
        public void SerializeAndDeserializeAsQueryableWithPredicateTest()
        {
            Expression<Func<Order, bool>> predicate = x => x.Id > 0 && x.Id < 5;

            Expression<Func<Document, bool>> pred = x => x.Orders.AsQueryable().Any(predicate);

            var serializer = new BinarySerializer();
            var value = serializer.SerializeGeneric(pred);

            var expression = serializer.DeserializeGeneric(value);
            Assert.IsNotNull(expression);
        }

        public class Order
        {
            public virtual int Id { get; set; }
            public virtual int Qty { get; set; }
        }
        
        public class Document
        {
            public virtual int Id { get; set; }
            public virtual ICollection<Order> Orders { get; set; }
        }

    }
}
