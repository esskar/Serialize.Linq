using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Factories;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.NewTests
{
    [TestClass]
    public class ScopeTests
    {
        [TestMethod]
        public void SerializeDeserializeScopeBinary()
        {
            var testClass = new ScopeClass();
            var context = new ExpressionContext() { AllowPrivateFieldAccess = true };

#pragma warning disable CS0618 // type or member is obsolete
            var serializer = new ExpressionSerializer(new BinarySerializer(), new FactorySettings() { AllowPrivateFieldAccess = true });
#pragma warning restore CS0618 // type or member is obsolete

            Expression<Func<ScopeClass, bool>> pubFuncExpression = scope => scope.PubFunc() == testClass.PubFunc();
            Expression<Func<ScopeClass, bool>> pubPropExpression = scope => scope.PubProp == testClass.PubProp;
            Expression<Func<ScopeClass, bool>> pubFieldExpression = scope => scope.PubField == testClass.PubField;
            Expression<Func<ScopeClass, bool>> intFuncExpression = scope => scope.IntFunc() == testClass.IntFunc();
            Expression<Func<ScopeClass, bool>> intPropExpression = scope => scope.IntProp == testClass.IntProp;
            Expression<Func<ScopeClass, bool>> intFieldExpression = scope => scope.IntField == testClass.IntField;

            var pubFuncValue = serializer.SerializeBinary(pubFuncExpression);
            var pubPropValue = serializer.SerializeBinary(pubPropExpression);
            var pubFieldValue = serializer.SerializeBinary(pubFieldExpression);
            var intFuncValue = serializer.SerializeBinary(intFuncExpression);
            var intPropValue = serializer.SerializeBinary(intPropExpression);
            var intFieldValue = serializer.SerializeBinary(intFieldExpression);

            var actualPubFuncExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeBinary(pubFuncValue);
            var actualPubPropExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeBinary(pubPropValue);
            var actualPubFieldExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeBinary(pubFieldValue);
            // the next line throws a MemberNotFoundException in version 2.0.0.0, since 'context' is not used
            var actualIntFuncExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeBinary(intFuncValue, context);
            var actualIntPropExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeBinary(intPropValue, context);
            var actualIntFieldExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeBinary(intFieldValue, context);

            Assert.IsTrue(actualPubFuncExpression.Compile().Invoke(testClass), "public function failed.");
            Assert.IsTrue(actualPubPropExpression.Compile().Invoke(testClass), "public property failed.");
            Assert.IsTrue(actualPubFieldExpression.Compile().Invoke(testClass), "public field failed.");
            Assert.IsTrue(actualIntFuncExpression.Compile().Invoke(testClass), "internal function failed.");
            Assert.IsTrue(actualIntPropExpression.Compile().Invoke(testClass), "internal property failed.");
            Assert.IsTrue(actualIntFieldExpression.Compile().Invoke(testClass), "internal field failed.");
        }

        [TestMethod]
        public void SerializeDeserializeScopeXml()
        {
            var testClass = new ScopeClass();
            var context = new ExpressionContext() { AllowPrivateFieldAccess = true };

#pragma warning disable CS0618 // type or member is obsolete
            var serializer = new ExpressionSerializer(new XmlSerializer(), new FactorySettings() { AllowPrivateFieldAccess = true });
#pragma warning restore CS0618 // type or member is obsolete

            Expression<Func<ScopeClass, bool>> pubFuncExpression = scope => scope.PubFunc() == testClass.PubFunc();
            Expression<Func<ScopeClass, bool>> pubPropExpression = scope => scope.PubProp == testClass.PubProp;
            Expression<Func<ScopeClass, bool>> pubFieldExpression = scope => scope.PubField == testClass.PubField;
            Expression<Func<ScopeClass, bool>> intFuncExpression = scope => scope.IntFunc() == testClass.IntFunc();
            Expression<Func<ScopeClass, bool>> intPropExpression = scope => scope.IntProp == testClass.IntProp;
            Expression<Func<ScopeClass, bool>> intFieldExpression = scope => scope.IntField == testClass.IntField;

            var pubFuncValue = serializer.SerializeText(pubFuncExpression);
            var pubPropValue = serializer.SerializeText(pubPropExpression);
            var pubFieldValue = serializer.SerializeText(pubFieldExpression);
            var intFuncValue = serializer.SerializeText(intFuncExpression);
            var intPropValue = serializer.SerializeText(intPropExpression);
            var intFieldValue = serializer.SerializeText(intFieldExpression);

            var actualPubFuncExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeText(pubFuncValue);
            var actualPubPropExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeText(pubPropValue);
            var actualPubFieldExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeText(pubFieldValue);
            // the next line throws a MemberNotFoundException in version 2.0.0.0, context is not used
            var actualIntFuncExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeText(intFuncValue, context);
            var actualIntPropExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeText(intPropValue, context);
            var actualIntFieldExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeText(intFieldValue, context);

            Assert.IsTrue(actualPubFuncExpression.Compile().Invoke(testClass), "public function failed.");
            Assert.IsTrue(actualPubPropExpression.Compile().Invoke(testClass), "public property failed.");
            Assert.IsTrue(actualPubFieldExpression.Compile().Invoke(testClass), "public field failed.");
            Assert.IsTrue(actualIntFuncExpression.Compile().Invoke(testClass), "internal function failed.");
            Assert.IsTrue(actualIntPropExpression.Compile().Invoke(testClass), "internal property failed.");
            Assert.IsTrue(actualIntFieldExpression.Compile().Invoke(testClass), "internal field failed.");
        }

        [TestMethod]
        public void SerializeDeserializeScopeJson()
        {
            var testClass = new ScopeClass();
            var context = new ExpressionContext() { AllowPrivateFieldAccess = true };

#pragma warning disable CS0618 // type or member is obsolete
            var serializer = new ExpressionSerializer(new JsonSerializer(), new FactorySettings() { AllowPrivateFieldAccess = true });
#pragma warning restore CS0618 // type or member is obsolete

            Expression<Func<ScopeClass, bool>> pubFuncExpression = scope => scope.PubFunc() == testClass.PubFunc();
            Expression<Func<ScopeClass, bool>> pubPropExpression = scope => scope.PubProp == testClass.PubProp;
            Expression<Func<ScopeClass, bool>> pubFieldExpression = scope => scope.PubField == testClass.PubField;
            Expression<Func<ScopeClass, bool>> intFuncExpression = scope => scope.IntFunc() == testClass.IntFunc();
            Expression<Func<ScopeClass, bool>> intPropExpression = scope => scope.IntProp == testClass.IntProp;
            Expression<Func<ScopeClass, bool>> intFieldExpression = scope => scope.IntField == testClass.IntField;

            var pubFuncValue = serializer.SerializeText(pubFuncExpression);
            var pubPropValue = serializer.SerializeText(pubPropExpression);
            var pubFieldValue = serializer.SerializeText(pubFieldExpression);
            var intFuncValue = serializer.SerializeText(intFuncExpression);
            var intPropValue = serializer.SerializeText(intPropExpression);
            var intFieldValue = serializer.SerializeText(intFieldExpression);

            var actualPubFuncExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeText(pubFuncValue);
            var actualPubPropExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeText(pubPropValue);
            var actualPubFieldExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeText(pubFieldValue);
            // the next line throws a MemberNotFoundException in version 2.0.0.0, context is not used
            var actualIntFuncExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeText(intFuncValue, context);
            var actualIntPropExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeText(intPropValue, context);
            var actualIntFieldExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeText(intFieldValue, context);

            Assert.IsTrue(actualPubFuncExpression.Compile().Invoke(testClass), "public function failed.");
            Assert.IsTrue(actualPubPropExpression.Compile().Invoke(testClass), "public property failed.");
            Assert.IsTrue(actualPubFieldExpression.Compile().Invoke(testClass), "public field failed.");
            Assert.IsTrue(actualIntFuncExpression.Compile().Invoke(testClass), "internal function failed.");
            Assert.IsTrue(actualIntPropExpression.Compile().Invoke(testClass), "internal property failed.");
            Assert.IsTrue(actualIntFieldExpression.Compile().Invoke(testClass), "internal field failed.");
        }

        [TestMethod]
        public void SerializeDeserializeScopeStreamBinary()
        {
            var testClass = new ScopeClass();

#pragma warning disable CS0618 // type or member is obsolete
            var serializer = new ExpressionSerializer(new BinarySerializer(), new FactorySettings() { AllowPrivateFieldAccess = true });
#pragma warning restore CS0618 // type or member is obsolete

            Expression<Func<ScopeClass, bool>> pubFuncExpression = scope => scope.PubFunc() == testClass.PubFunc();
            Expression<Func<ScopeClass, bool>> pubPropExpression = scope => scope.PubProp == testClass.PubProp;
            Expression<Func<ScopeClass, bool>> pubFieldExpression = scope => scope.PubField == testClass.PubField;
            Expression<Func<ScopeClass, bool>> intFuncExpression = scope => scope.IntFunc() == testClass.IntFunc();
            Expression<Func<ScopeClass, bool>> intPropExpression = scope => scope.IntProp == testClass.IntProp;
            Expression<Func<ScopeClass, bool>> intFieldExpression = scope => scope.IntField == testClass.IntField;

            Expression<Func<ScopeClass, bool>> actualPubFuncExpression;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, pubFuncExpression);
                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
                stream.Position = 0;
                actualPubFuncExpression = (Expression<Func<ScopeClass, bool>>)serializer.Deserialize(stream);
            }
            Expression<Func<ScopeClass, bool>> actualPubPropExpression;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, pubPropExpression);
                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
                stream.Position = 0;
                actualPubPropExpression = (Expression<Func<ScopeClass, bool>>)serializer.Deserialize(stream);
            }
            Expression<Func<ScopeClass, bool>> actualPubFieldExpression;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, pubFieldExpression);
                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
                stream.Position = 0;
                actualPubFieldExpression = (Expression<Func<ScopeClass, bool>>)serializer.Deserialize(stream);
            }
            Expression<Func<ScopeClass, bool>> actualIntFuncExpression;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, intFuncExpression);
                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
                stream.Position = 0;
                // the next line throws a MemberNotFoundException in version 2.0.0.0,
                // because BindingContext can neither be specified directly nor is it derived from the serializer's FactorySettings
                actualIntFuncExpression = (Expression<Func<ScopeClass, bool>>)serializer.Deserialize(stream);
            }
            Expression<Func<ScopeClass, bool>> actualIntPropExpression;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, intPropExpression);
                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
                stream.Position = 0;
                // the next line throws a MemberNotFoundException in version 2.0.0.0,
                // because BindingContext can neither be specified directly nor is it derived from the serializer's FactorySettings
                actualIntPropExpression = (Expression<Func<ScopeClass, bool>>)serializer.Deserialize(stream);
            }
            Expression<Func<ScopeClass, bool>> actualIntFieldExpression;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, intFieldExpression);
                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
                stream.Position = 0;
                // the next line throws a MemberNotFoundException in version 2.0.0.0,
                // because BindingContext can neither be specified directly nor is it derived from the serializer's FactorySettings
                actualIntFieldExpression = (Expression<Func<ScopeClass, bool>>)serializer.Deserialize(stream);
            }

            Assert.IsTrue(actualPubFuncExpression.Compile().Invoke(testClass), "public function failed.");
            Assert.IsTrue(actualPubPropExpression.Compile().Invoke(testClass), "public property failed.");
            Assert.IsTrue(actualPubFieldExpression.Compile().Invoke(testClass), "public field failed.");
            Assert.IsTrue(actualIntFuncExpression.Compile().Invoke(testClass), "internal function failed.");
            Assert.IsTrue(actualIntPropExpression.Compile().Invoke(testClass), "internal property failed.");
            Assert.IsTrue(actualIntFieldExpression.Compile().Invoke(testClass), "internal field failed.");
        }

        [TestMethod]
        public void SerializeDeserializeScopeStreamXml()
        {
            var testClass = new ScopeClass();
            var factorySettings = new FactorySettings() { AllowPrivateFieldAccess = true };

#pragma warning disable CS0618 // type or member is obsolete
            var serializer = new ExpressionSerializer(new XmlSerializer(), factorySettings);
#pragma warning restore CS0618 // type or member is obsolete

            Expression<Func<ScopeClass, bool>> pubFuncExpression = scope => scope.PubFunc() == testClass.PubFunc();
            Expression<Func<ScopeClass, bool>> pubPropExpression = scope => scope.PubProp == testClass.PubProp;
            Expression<Func<ScopeClass, bool>> pubFieldExpression = scope => scope.PubField == testClass.PubField;
            Expression<Func<ScopeClass, bool>> intFuncExpression = scope => scope.IntFunc() == testClass.IntFunc();
            Expression<Func<ScopeClass, bool>> intPropExpression = scope => scope.IntProp == testClass.IntProp;
            Expression<Func<ScopeClass, bool>> intFieldExpression = scope => scope.IntField == testClass.IntField;

            Expression<Func<ScopeClass, bool>> actualPubFuncExpression;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, pubFuncExpression);
                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
                stream.Position = 0;
                actualPubFuncExpression = (Expression<Func<ScopeClass, bool>>)serializer.Deserialize(stream);
            }
            Expression<Func<ScopeClass, bool>> actualPubPropExpression;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, pubPropExpression);
                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
                stream.Position = 0;
                actualPubPropExpression = (Expression<Func<ScopeClass, bool>>)serializer.Deserialize(stream);
            }
            Expression<Func<ScopeClass, bool>> actualPubFieldExpression;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, pubFieldExpression);
                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
                stream.Position = 0;
                actualPubFieldExpression = (Expression<Func<ScopeClass, bool>>)serializer.Deserialize(stream);
            }
            Expression<Func<ScopeClass, bool>> actualIntFuncExpression;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, intFuncExpression);
                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
                stream.Position = 0;
                // the next line throws a MemberNotFoundException in version 2.0.0.0, because 'context' can neither be specified directly nor is it derived from 'factorySettings'
                actualIntFuncExpression = (Expression<Func<ScopeClass, bool>>)serializer.Deserialize(stream);
            }
            Expression<Func<ScopeClass, bool>> actualIntPropExpression;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, intPropExpression);
                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
                stream.Position = 0;
                // the next line throws a MemberNotFoundException in version 2.0.0.0, because 'context' can neither be specified directly nor is it derived from 'factorySettings'
                actualIntPropExpression = (Expression<Func<ScopeClass, bool>>)serializer.Deserialize(stream);
            }
            Expression<Func<ScopeClass, bool>> actualIntFieldExpression;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, intFieldExpression);
                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
                stream.Position = 0;
                // the next line throws a MemberNotFoundException in version 2.0.0.0, because 'context' can neither be specified directly nor is it derived from 'factorySettings'
                actualIntFieldExpression = (Expression<Func<ScopeClass, bool>>)serializer.Deserialize(stream);
            }

            Assert.IsTrue(actualPubFuncExpression.Compile().Invoke(testClass), "public function failed.");
            Assert.IsTrue(actualPubPropExpression.Compile().Invoke(testClass), "public property failed.");
            Assert.IsTrue(actualPubFieldExpression.Compile().Invoke(testClass), "public field failed.");
            Assert.IsTrue(actualIntFuncExpression.Compile().Invoke(testClass), "internal function failed.");
            Assert.IsTrue(actualIntPropExpression.Compile().Invoke(testClass), "internal property failed.");
            Assert.IsTrue(actualIntFieldExpression.Compile().Invoke(testClass), "internal field failed.");
        }

        [TestMethod]
        public void SerializeDeserializeScopeStreamJson()
        {
            var testClass = new ScopeClass();
            var factorySettings = new FactorySettings() { AllowPrivateFieldAccess = true };

#pragma warning disable CS0618 // type or member is obsolete
            var serializer = new ExpressionSerializer(new JsonSerializer(), factorySettings);
#pragma warning restore CS0618 // type or member is obsolete

            Expression<Func<ScopeClass, bool>> pubFuncExpression = scope => scope.PubFunc() == testClass.PubFunc();
            Expression<Func<ScopeClass, bool>> pubPropExpression = scope => scope.PubProp == testClass.PubProp;
            Expression<Func<ScopeClass, bool>> pubFieldExpression = scope => scope.PubField == testClass.PubField;
            Expression<Func<ScopeClass, bool>> intFuncExpression = scope => scope.IntFunc() == testClass.IntFunc();
            Expression<Func<ScopeClass, bool>> intPropExpression = scope => scope.IntProp == testClass.IntProp;
            Expression<Func<ScopeClass, bool>> intFieldExpression = scope => scope.IntField == testClass.IntField;

            Expression<Func<ScopeClass, bool>> actualPubFuncExpression;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, pubFuncExpression);
                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
                stream.Position = 0;
                actualPubFuncExpression = (Expression<Func<ScopeClass, bool>>)serializer.Deserialize(stream);
            }
            Expression<Func<ScopeClass, bool>> actualPubPropExpression;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, pubPropExpression);
                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
                stream.Position = 0;
                actualPubPropExpression = (Expression<Func<ScopeClass, bool>>)serializer.Deserialize(stream);
            }
            Expression<Func<ScopeClass, bool>> actualPubFieldExpression;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, pubFieldExpression);
                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
                stream.Position = 0;
                actualPubFieldExpression = (Expression<Func<ScopeClass, bool>>)serializer.Deserialize(stream);
            }
            Expression<Func<ScopeClass, bool>> actualIntFuncExpression;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, intFuncExpression);
                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
                stream.Position = 0;
                // the next line throws a MemberNotFoundException in version 2.0.0.0, because 'context' can neither be specified directly nor is it derived from 'factorySettings'
                actualIntFuncExpression = (Expression<Func<ScopeClass, bool>>)serializer.Deserialize(stream);
            }
            Expression<Func<ScopeClass, bool>> actualIntPropExpression;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, intPropExpression);
                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
                stream.Position = 0;
                // the next line throws a MemberNotFoundException in version 2.0.0.0, because 'context' can neither be specified directly nor is it derived from 'factorySettings'
                actualIntPropExpression = (Expression<Func<ScopeClass, bool>>)serializer.Deserialize(stream);
            }
            Expression<Func<ScopeClass, bool>> actualIntFieldExpression;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, intFieldExpression);
                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
                stream.Position = 0;
                // the next line throws a MemberNotFoundException in version 2.0.0.0, because 'context' can neither be specified directly nor is it derived from 'factorySettings'
                actualIntFieldExpression = (Expression<Func<ScopeClass, bool>>)serializer.Deserialize(stream);
            }

            Assert.IsTrue(actualPubFuncExpression.Compile().Invoke(testClass), "public function failed.");
            Assert.IsTrue(actualPubPropExpression.Compile().Invoke(testClass), "public property failed.");
            Assert.IsTrue(actualPubFieldExpression.Compile().Invoke(testClass), "public field failed.");
            Assert.IsTrue(actualIntFuncExpression.Compile().Invoke(testClass), "internal function failed.");
            Assert.IsTrue(actualIntPropExpression.Compile().Invoke(testClass), "internal property failed.");
            Assert.IsTrue(actualIntFieldExpression.Compile().Invoke(testClass), "internal field failed.");
        }

        private class ScopeClass
        {
            public ScopeClass()
            {
                IntField = true;
            }

            public bool PubFunc() => true;

            internal bool IntFunc() => true;

            public bool PubProp => true;

            internal bool IntProp => true;

            public bool PubField;

            internal bool IntField;
        }
    }
}