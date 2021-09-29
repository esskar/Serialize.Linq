//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq.Expressions;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Serialize.Linq.Factories;
//using Serialize.Linq.Interfaces;
//using Serialize.Linq.Serializers;

//namespace Serialize.Linq.Tests.NewTests
//{
//    [TestClass]
//    public class ScopeTestsGeneric
//    {
//        [TestMethod]
//        public void SerializeDeserializeScope()
//        {
//            var factorySettings = new FactorySettings() { AllowPrivateFieldAccess = true };
//            SerializeDeserializeScopeInternal(new BinarySerializer(factorySettings));
//            SerializeDeserializeScopeInternal(new XmlSerializer(factorySettings));
//            SerializeDeserializeScopeInternal(new JsonSerializer(factorySettings));
//        }

//        [TestMethod]
//        public void SerializeDeserializeScopeStream()
//        {
//            var factorySettings = new FactorySettings() { AllowPrivateFieldAccess = true };
//            SerializeDeserializeScopeStreamInternal(new BinarySerializer(factorySettings));
//            SerializeDeserializeScopeStreamInternal(new XmlSerializer(factorySettings));
//            SerializeDeserializeScopeStreamInternal(new JsonSerializer(factorySettings));
//        }

//        private static void SerializeDeserializeScopeInternal<T>(IGenericSerializer<T> serializer)
//        {
//            var testClass = new ScopeClass();
//            // 'context' is no longer needed, because 'factorySettings' is forwarded
//            // var context = new ExpressionContext() { AllowPrivateFieldAccess = true };

//            Expression<Func<ScopeClass, bool>> pubFuncExpression = scope => scope.PubFunc() == testClass.PubFunc();
//            Expression<Func<ScopeClass, bool>> pubPropExpression = scope => scope.PubProp == testClass.PubProp;
//            Expression<Func<ScopeClass, bool>> pubFieldExpression = scope => scope.PubField == testClass.PubField;
//            Expression<Func<ScopeClass, bool>> intFuncExpression = scope => scope.IntFunc() == testClass.IntFunc();
//            Expression<Func<ScopeClass, bool>> intPropExpression = scope => scope.IntProp == testClass.IntProp;
//            Expression<Func<ScopeClass, bool>> intFieldExpression = scope => scope.IntField == testClass.IntField;

//            var pubFuncValue = serializer.SerializeGeneric(pubFuncExpression);
//            var pubPropValue = serializer.SerializeGeneric(pubPropExpression);
//            var pubFieldValue = serializer.SerializeGeneric(pubFieldExpression);
//            var intFuncValue = serializer.SerializeGeneric(intFuncExpression);
//            var intPropValue = serializer.SerializeGeneric(intPropExpression);
//            var intFieldValue = serializer.SerializeGeneric(intFieldExpression);

//            var actualPubFuncExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeGeneric(pubFuncValue);
//            var actualPubPropExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeGeneric(pubPropValue);
//            var actualPubFieldExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeGeneric(pubFieldValue);
//            // the next line throws a MemberNotFoundException in version 2.0.0.0 even if 'context' is specified
//            var actualIntFuncExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeGeneric(intFuncValue);
//            var actualIntPropExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeGeneric(intPropValue);
//            var actualIntFieldExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeGeneric(intFieldValue);

//            Assert.IsTrue(actualPubFuncExpression.Compile().Invoke(testClass), "public function failed.");
//            Assert.IsTrue(actualPubPropExpression.Compile().Invoke(testClass), "public property failed.");
//            Assert.IsTrue(actualPubFieldExpression.Compile().Invoke(testClass), "public field failed.");
//            Assert.IsTrue(actualIntFuncExpression.Compile().Invoke(testClass), "internal function failed.");
//            Assert.IsTrue(actualIntPropExpression.Compile().Invoke(testClass), "internal property failed.");
//            Assert.IsTrue(actualIntFieldExpression.Compile().Invoke(testClass), "internal field failed.");
//        }

//        private static void SerializeDeserializeScopeStreamInternal<T>(IGenericSerializer<T> serializer)
//        {
//            var testClass = new ScopeClass();

//            Expression<Func<ScopeClass, bool>> pubFuncExpression = scope => scope.PubFunc() == testClass.PubFunc();
//            Expression<Func<ScopeClass, bool>> pubPropExpression = scope => scope.PubProp == testClass.PubProp;
//            Expression<Func<ScopeClass, bool>> pubFieldExpression = scope => scope.PubField == testClass.PubField;
//            Expression<Func<ScopeClass, bool>> intFuncExpression = scope => scope.IntFunc() == testClass.IntFunc();
//            Expression<Func<ScopeClass, bool>> intPropExpression = scope => scope.IntProp == testClass.IntProp;
//            Expression<Func<ScopeClass, bool>> intFieldExpression = scope => scope.IntField == testClass.IntField;

//            Expression<Func<ScopeClass, bool>> actualPubFuncExpression;
//            using (var stream = new MemoryStream())
//            {
//                serializer.Serialize(stream, pubFuncExpression);
//                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
//                stream.Position = 0;
//                actualPubFuncExpression = (Expression<Func<ScopeClass, bool>>)serializer.Deserialize(stream);
//            }
//            Expression<Func<ScopeClass, bool>> actualPubPropExpression;
//            using (var stream = new MemoryStream())
//            {
//                serializer.Serialize(stream, pubPropExpression);
//                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
//                stream.Position = 0;
//                actualPubPropExpression = (Expression<Func<ScopeClass, bool>>)serializer.Deserialize(stream);
//            }
//            Expression<Func<ScopeClass, bool>> actualPubFieldExpression;
//            using (var stream = new MemoryStream())
//            {
//                serializer.Serialize(stream, pubFieldExpression);
//                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
//                stream.Position = 0;
//                actualPubFieldExpression = (Expression<Func<ScopeClass, bool>>)serializer.Deserialize(stream);
//            }
//            Expression<Func<ScopeClass, bool>> actualIntFuncExpression;
//            using (var stream = new MemoryStream())
//            {
//                serializer.Serialize(stream, intFuncExpression);
//                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
//                stream.Position = 0;
//                // the next line throws a MemberNotFoundException in version 2.0.0.0,
//                // because BindingContext can neither be specified directly nor is it derived from the serializer's FactorySettings
//                actualIntFuncExpression = (Expression<Func<ScopeClass, bool>>)serializer.Deserialize(stream);
//            }
//            Expression<Func<ScopeClass, bool>> actualIntPropExpression;
//            using (var stream = new MemoryStream())
//            {
//                serializer.Serialize(stream, intPropExpression);
//                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
//                stream.Position = 0;
//                // the next line throws a MemberNotFoundException in version 2.0.0.0,
//                // because BindingContext can neither be specified directly nor is it derived from the serializer's FactorySettings
//                actualIntPropExpression = (Expression<Func<ScopeClass, bool>>)serializer.Deserialize(stream);
//            }
//            Expression<Func<ScopeClass, bool>> actualIntFieldExpression;
//            using (var stream = new MemoryStream())
//            {
//                serializer.Serialize(stream, intFieldExpression);
//                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
//                stream.Position = 0;
//                // the next line throws a MemberNotFoundException in version 2.0.0.0,
//                // because BindingContext can neither be specified directly nor is it derived from the serializer's FactorySettings
//                actualIntFieldExpression = (Expression<Func<ScopeClass, bool>>)serializer.Deserialize(stream);
//            }

//            Assert.IsTrue(actualPubFuncExpression.Compile().Invoke(testClass), "public function failed.");
//            Assert.IsTrue(actualPubPropExpression.Compile().Invoke(testClass), "public property failed.");
//            Assert.IsTrue(actualPubFieldExpression.Compile().Invoke(testClass), "public field failed.");
//            Assert.IsTrue(actualIntFuncExpression.Compile().Invoke(testClass), "internal function failed.");
//            Assert.IsTrue(actualIntPropExpression.Compile().Invoke(testClass), "internal property failed.");
//            Assert.IsTrue(actualIntFieldExpression.Compile().Invoke(testClass), "internal field failed.");
//        }

//        private class ScopeClass
//        {
//            public ScopeClass()
//            {
//                IntField = true;
//            }

//            public bool PubFunc() => true;

//            internal bool IntFunc() => true;

//            public bool PubProp => true;

//            internal bool IntProp => true;

//            public bool PubField;

//            internal bool IntField;
//        }
//    }
//}