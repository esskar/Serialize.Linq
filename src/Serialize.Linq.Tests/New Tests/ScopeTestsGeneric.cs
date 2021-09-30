using System;
using System.IO;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Factories;
using Serialize.Linq.Interfaces;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.NewTests
{
#pragma warning disable CA1822 // mark members as static
    /// <summary>
    /// https://github.com/esskar/Serialize.Linq/issues/105
    /// Test and fix provided by https://github.com/oahrens
    /// </summary>
    [TestClass]
    public class ScopeTestsGeneric
    {
        [TestMethod]
        public void SerializeDeserializeScope()
        {
            var factorySettings = new FactorySettings() { AllowPrivateFieldAccess = true };
            SerializeDeserializeScopeInternal(new BinarySerializer(factorySettings));
            SerializeDeserializeScopeInternal(new XmlSerializer(factorySettings));
            SerializeDeserializeScopeInternal(new JsonSerializer(factorySettings));
        }

        [TestMethod]
        public void SerializeDeserializeScopeStream()
        {
            var factorySettings = new FactorySettings() { AllowPrivateFieldAccess = true };
            SerializeDeserializeScopeStreamInternal(new BinarySerializer(factorySettings));
            SerializeDeserializeScopeStreamInternal(new XmlSerializer(factorySettings));
            SerializeDeserializeScopeStreamInternal(new JsonSerializer(factorySettings));
        }

        private static void SerializeDeserializeScopeInternal<T>(IGenericSerializer<T> serializer)
        {
            var testClass = new ScopeClass();
            // 'context' is no longer needed, because 'factorySettings' is forwarded
            // var context = new ExpressionContext() { AllowPrivateFieldAccess = true };

            Expression<Func<ScopeClass, bool>> pubFuncExpression = scope => scope.PubFunc() == testClass.PubFunc();
            Expression<Func<ScopeClass, bool>> pubPropExpression = scope => scope.PubProp == testClass.PubProp;
            Expression<Func<ScopeClass, bool>> pubFieldExpression = scope => scope.PubField == testClass.PubField;
            Expression<Func<ScopeClass, bool>> intFuncExpression = scope => scope.IntFunc() == testClass.IntFunc();
            Expression<Func<ScopeClass, bool>> intPropExpression = scope => scope.IntProp == testClass.IntProp;
            Expression<Func<ScopeClass, bool>> intFieldExpression = scope => scope.IntField == testClass.IntField;
            Expression<Func<int>> pubStaticFuncExpression = () => ScopeClass.PubStaticFunc();
            Expression<Func<int>> pubStaticPropExpression = () => ScopeClass.PubStaticProp;
            Expression<Func<int>> pubStaticFieldExpression = () => ScopeClass.PubStaticField;
            Expression<Func<int>> intStaticFuncExpression = () => ScopeClass.IntStaticFunc();
            Expression<Func<int>> intStaticPropExpression = () => ScopeClass.IntStaticProp;
            Expression<Func<int>> intStaticFieldExpression = () => ScopeClass.IntStaticField;

            var pubFuncValue = serializer.SerializeGeneric(pubFuncExpression);
            var pubPropValue = serializer.SerializeGeneric(pubPropExpression);
            var pubFieldValue = serializer.SerializeGeneric(pubFieldExpression);
            var intFuncValue = serializer.SerializeGeneric(intFuncExpression);
            var intPropValue = serializer.SerializeGeneric(intPropExpression);
            var intFieldValue = serializer.SerializeGeneric(intFieldExpression);
            var pubStaticFuncValue = serializer.SerializeGeneric(pubStaticFuncExpression);
            var pubStaticPropValue = serializer.SerializeGeneric(pubStaticPropExpression);
            var pubStaticFieldValue = serializer.SerializeGeneric(pubStaticFieldExpression);
            var intStaticFuncValue = serializer.SerializeGeneric(intStaticFuncExpression);
            var intStaticPropValue = serializer.SerializeGeneric(intStaticPropExpression);
            var intStaticFieldValue = serializer.SerializeGeneric(intStaticFieldExpression);

            var actualPubFuncExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeGeneric(pubFuncValue);
            var actualPubPropExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeGeneric(pubPropValue);
            var actualPubFieldExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeGeneric(pubFieldValue);
            // the next line throws a MemberNotFoundException in version 2.0.0.0 even if 'context' is specified
            var actualIntFuncExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeGeneric(intFuncValue);
            var actualIntPropExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeGeneric(intPropValue);
            var actualIntFieldExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeGeneric(intFieldValue);
            var actualPubStaticFuncExpression = (Expression<Func<int>>)serializer.DeserializeGeneric(pubStaticFuncValue);
            var actualPubStaticPropExpression = (Expression<Func<int>>)serializer.DeserializeGeneric(pubStaticPropValue);
            var actualPubStaticFieldExpression = (Expression<Func<int>>)serializer.DeserializeGeneric(pubStaticFieldValue);
            // the next line throws a MemberNotFoundException in version 2.0.0.0, since 'context' is not used
            var actualIntStaticFuncExpression = (Expression<Func<int>>)serializer.DeserializeGeneric(intStaticFuncValue);
            var actualIntStaticPropExpression = (Expression<Func<int>>)serializer.DeserializeGeneric(intStaticPropValue);
            var actualIntStaticFieldExpression = (Expression<Func<int>>)serializer.DeserializeGeneric(intStaticFieldValue);

            Assert.IsTrue(actualPubFuncExpression.Compile().Invoke(testClass), "public function failed.");
            Assert.IsTrue(actualPubPropExpression.Compile().Invoke(testClass), "public property failed.");
            Assert.IsTrue(actualPubFieldExpression.Compile().Invoke(testClass), "public field failed.");
            Assert.IsTrue(actualIntFuncExpression.Compile().Invoke(testClass), "internal function failed.");
            Assert.IsTrue(actualIntPropExpression.Compile().Invoke(testClass), "internal property failed.");
            Assert.IsTrue(actualIntFieldExpression.Compile().Invoke(testClass), "internal field failed.");
            Assert.IsTrue(actualPubStaticFuncExpression.Compile().Invoke() == ScopeClass.PubStaticFunc(), "public function failed.");
            Assert.IsTrue(actualPubStaticPropExpression.Compile().Invoke() == ScopeClass.PubStaticProp, "public property failed.");
            Assert.IsTrue(actualPubStaticFieldExpression.Compile().Invoke() == ScopeClass.PubStaticField, "public field failed.");
            Assert.IsTrue(actualIntStaticFuncExpression.Compile().Invoke() == ScopeClass.IntStaticFunc(), "internal function failed.");
            Assert.IsTrue(actualIntStaticPropExpression.Compile().Invoke() == ScopeClass.IntStaticProp, "internal property failed.");
            Assert.IsTrue(actualIntStaticFieldExpression.Compile().Invoke() == ScopeClass.IntStaticField, "internal field failed.");
        }

        private static void SerializeDeserializeScopeStreamInternal(IExpressionSerializer serializer)
        {
            var testClass = new ScopeClass();

            Expression<Func<ScopeClass, bool>> pubFuncExpression = scope => scope.PubFunc() == testClass.PubFunc();
            Expression<Func<ScopeClass, bool>> pubPropExpression = scope => scope.PubProp == testClass.PubProp;
            Expression<Func<ScopeClass, bool>> pubFieldExpression = scope => scope.PubField == testClass.PubField;
            Expression<Func<ScopeClass, bool>> intFuncExpression = scope => scope.IntFunc() == testClass.IntFunc();
            Expression<Func<ScopeClass, bool>> intPropExpression = scope => scope.IntProp == testClass.IntProp;
            Expression<Func<ScopeClass, bool>> intFieldExpression = scope => scope.IntField == testClass.IntField;
            Expression<Func<int>> pubStaticFuncExpression = () => ScopeClass.PubStaticFunc();
            Expression<Func<int>> pubStaticPropExpression = () => ScopeClass.PubStaticProp;
            Expression<Func<int>> pubStaticFieldExpression = () => ScopeClass.PubStaticField;
            Expression<Func<int>> intStaticFuncExpression = () => ScopeClass.IntStaticFunc();
            Expression<Func<int>> intStaticPropExpression = () => ScopeClass.IntStaticProp;
            Expression<Func<int>> intStaticFieldExpression = () => ScopeClass.IntStaticField;

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
            Expression<Func<int>> actualPubStaticFuncExpression;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, pubStaticFuncExpression);
                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
                stream.Position = 0;
                actualPubStaticFuncExpression = (Expression<Func<int>>)serializer.Deserialize(stream);
            }
            Expression<Func<int>> actualPubStaticPropExpression;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, pubStaticPropExpression);
                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
                stream.Position = 0;
                actualPubStaticPropExpression = (Expression<Func<int>>)serializer.Deserialize(stream);
            }
            Expression<Func<int>> actualPubStaticFieldExpression;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, pubStaticFieldExpression);
                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
                stream.Position = 0;
                actualPubStaticFieldExpression = (Expression<Func<int>>)serializer.Deserialize(stream);
            }
            Expression<Func<int>> actualIntStaticFuncExpression;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, intStaticFuncExpression);
                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
                stream.Position = 0;
                // the next line throws a MemberNotFoundException in version 2.0.0.0,
                // because BindingContext can neither be specified directly nor is it derived from the serializer's FactorySettings
                actualIntStaticFuncExpression = (Expression<Func<int>>)serializer.Deserialize(stream);
            }
            Expression<Func<int>> actualIntStaticPropExpression;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, intStaticPropExpression);
                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
                stream.Position = 0;
                // the next line throws a MemberNotFoundException in version 2.0.0.0,
                // because BindingContext can neither be specified directly nor is it derived from the serializer's FactorySettings
                actualIntStaticPropExpression = (Expression<Func<int>>)serializer.Deserialize(stream);
            }
            Expression<Func<int>> actualIntStaticFieldExpression;
            using (var stream = new MemoryStream())
            {
                serializer.Serialize(stream, intStaticFieldExpression);
                // System.ObjectDisposedException: "Cannot access a closed Stream." is thrown in version 2.0.0.0
                stream.Position = 0;
                // the next line throws a MemberNotFoundException in version 2.0.0.0,
                // because BindingContext can neither be specified directly nor is it derived from the serializer's FactorySettings
                actualIntStaticFieldExpression = (Expression<Func<int>>)serializer.Deserialize(stream);
            }

            Assert.IsTrue(actualPubFuncExpression.Compile().Invoke(testClass), "public function failed.");
            Assert.IsTrue(actualPubPropExpression.Compile().Invoke(testClass), "public property failed.");
            Assert.IsTrue(actualPubFieldExpression.Compile().Invoke(testClass), "public field failed.");
            Assert.IsTrue(actualIntFuncExpression.Compile().Invoke(testClass), "internal function failed.");
            Assert.IsTrue(actualIntPropExpression.Compile().Invoke(testClass), "internal property failed.");
            Assert.IsTrue(actualIntFieldExpression.Compile().Invoke(testClass), "internal field failed.");
            Assert.IsTrue(actualPubStaticFuncExpression.Compile().Invoke() == ScopeClass.PubStaticFunc(), "public function failed.");
            Assert.IsTrue(actualPubStaticPropExpression.Compile().Invoke() == ScopeClass.PubStaticProp, "public property failed.");
            Assert.IsTrue(actualPubStaticFieldExpression.Compile().Invoke() == ScopeClass.PubStaticField, "public field failed.");
            Assert.IsTrue(actualIntStaticFuncExpression.Compile().Invoke() == ScopeClass.IntStaticFunc(), "internal function failed.");
            Assert.IsTrue(actualIntStaticPropExpression.Compile().Invoke() == ScopeClass.IntStaticProp, "internal property failed.");
            Assert.IsTrue(actualIntStaticFieldExpression.Compile().Invoke() == ScopeClass.IntStaticField, "internal field failed.");
        }

        private class ScopeClass
        {
            public ScopeClass()
            {
                PubField = 3;
                IntField = 6;
                PubStaticField = 9;
                IntStaticField = 12;
            }

            public int PubFunc()
            {
                return 1;
            }

            public int PubProp => 2;

            public int PubField;

            internal int IntFunc()
            {
                return 4;
            }

            internal int IntProp => 5;

            internal int IntField;

            public static int PubStaticFunc()
            {
                return 7;
            }

            public static int PubStaticProp => 8;

            public static int PubStaticField;

            internal static int IntStaticFunc()
            {
                return 10;
            }

            internal static int IntStaticProp => 11;

            internal static int IntStaticField;
        }
    }
#pragma warning restore CA1822 // mark members as static
}