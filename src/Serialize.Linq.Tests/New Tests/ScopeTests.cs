using System;
using System.IO;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Factories;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.NewTests
{
#pragma warning disable CA1822 // mark members as static
    /// <summary>
    /// https://github.com/esskar/Serialize.Linq/issues/105
    /// Test and fix provided by https://github.com/oahrens
    /// </summary>
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
            Expression<Func<int>> pubStaticFuncExpression = () => ScopeClass.PubStaticFunc();
            Expression<Func<int>> pubStaticPropExpression = () => ScopeClass.PubStaticProp;
            Expression<Func<int>> pubStaticFieldExpression = () => ScopeClass.PubStaticField;
            Expression<Func<int>> intStaticFuncExpression = () => ScopeClass.IntStaticFunc();
            Expression<Func<int>> intStaticPropExpression = () => ScopeClass.IntStaticProp;
            Expression<Func<int>> intStaticFieldExpression = () => ScopeClass.IntStaticField;

            var pubFuncValue = serializer.SerializeBinary(pubFuncExpression);
            var pubPropValue = serializer.SerializeBinary(pubPropExpression);
            var pubFieldValue = serializer.SerializeBinary(pubFieldExpression);
            var intFuncValue = serializer.SerializeBinary(intFuncExpression);
            var intPropValue = serializer.SerializeBinary(intPropExpression);
            var intFieldValue = serializer.SerializeBinary(intFieldExpression);
            var pubStaticFuncValue = serializer.SerializeBinary(pubStaticFuncExpression);
            var pubStaticPropValue = serializer.SerializeBinary(pubStaticPropExpression);
            var pubStaticFieldValue = serializer.SerializeBinary(pubStaticFieldExpression);
            var intStaticFuncValue = serializer.SerializeBinary(intStaticFuncExpression);
            var intStaticPropValue = serializer.SerializeBinary(intStaticPropExpression);
            var intStaticFieldValue = serializer.SerializeBinary(intStaticFieldExpression);

            var actualPubFuncExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeBinary(pubFuncValue);
            var actualPubPropExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeBinary(pubPropValue);
            var actualPubFieldExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeBinary(pubFieldValue);
            // the next line throws a MemberNotFoundException in version 2.0.0.0, since 'context' is not used
            var actualIntFuncExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeBinary(intFuncValue, context);
            var actualIntPropExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeBinary(intPropValue, context);
            var actualIntFieldExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeBinary(intFieldValue, context);
            var actualPubStaticFuncExpression = (Expression<Func<int>>)serializer.DeserializeBinary(pubStaticFuncValue);
            var actualPubStaticPropExpression = (Expression<Func<int>>)serializer.DeserializeBinary(pubStaticPropValue);
            var actualPubStaticFieldExpression = (Expression<Func<int>>)serializer.DeserializeBinary(pubStaticFieldValue);
            // the next line throws a MemberNotFoundException in version 2.0.0.0, since 'context' is not used
            var actualIntStaticFuncExpression = (Expression<Func<int>>)serializer.DeserializeBinary(intStaticFuncValue, context);
            var actualIntStaticPropExpression = (Expression<Func<int>>)serializer.DeserializeBinary(intStaticPropValue, context);
            var actualIntStaticFieldExpression = (Expression<Func<int>>)serializer.DeserializeBinary(intStaticFieldValue, context);

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
            Expression<Func<int>> pubStaticFuncExpression = () => ScopeClass.PubStaticFunc();
            Expression<Func<int>> pubStaticPropExpression = () => ScopeClass.PubStaticProp;
            Expression<Func<int>> pubStaticFieldExpression = () => ScopeClass.PubStaticField;
            Expression<Func<int>> intStaticFuncExpression = () => ScopeClass.IntStaticFunc();
            Expression<Func<int>> intStaticPropExpression = () => ScopeClass.IntStaticProp;
            Expression<Func<int>> intStaticFieldExpression = () => ScopeClass.IntStaticField;

            var pubFuncValue = serializer.SerializeText(pubFuncExpression);
            var pubPropValue = serializer.SerializeText(pubPropExpression);
            var pubFieldValue = serializer.SerializeText(pubFieldExpression);
            var intFuncValue = serializer.SerializeText(intFuncExpression);
            var intPropValue = serializer.SerializeText(intPropExpression);
            var intFieldValue = serializer.SerializeText(intFieldExpression);
            var pubStaticFuncValue = serializer.SerializeText(pubStaticFuncExpression);
            var pubStaticPropValue = serializer.SerializeText(pubStaticPropExpression);
            var pubStaticFieldValue = serializer.SerializeText(pubStaticFieldExpression);
            var intStaticFuncValue = serializer.SerializeText(intStaticFuncExpression);
            var intStaticPropValue = serializer.SerializeText(intStaticPropExpression);
            var intStaticFieldValue = serializer.SerializeText(intStaticFieldExpression);

            var actualPubFuncExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeText(pubFuncValue);
            var actualPubPropExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeText(pubPropValue);
            var actualPubFieldExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeText(pubFieldValue);
            // the next line throws a MemberNotFoundException in version 2.0.0.0, context is not used
            var actualIntFuncExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeText(intFuncValue, context);
            var actualIntPropExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeText(intPropValue, context);
            var actualIntFieldExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeText(intFieldValue, context);
            var actualPubStaticFuncExpression = (Expression<Func<int>>)serializer.DeserializeText(pubStaticFuncValue);
            var actualPubStaticPropExpression = (Expression<Func<int>>)serializer.DeserializeText(pubStaticPropValue);
            var actualPubStaticFieldExpression = (Expression<Func<int>>)serializer.DeserializeText(pubStaticFieldValue);
            // the next line throws a MemberNotFoundException in version 2.0.0.0, since 'context' is not used
            var actualIntStaticFuncExpression = (Expression<Func<int>>)serializer.DeserializeText(intStaticFuncValue, context);
            var actualIntStaticPropExpression = (Expression<Func<int>>)serializer.DeserializeText(intStaticPropValue, context);
            var actualIntStaticFieldExpression = (Expression<Func<int>>)serializer.DeserializeText(intStaticFieldValue, context);

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
            Expression<Func<int>> pubStaticFuncExpression = () => ScopeClass.PubStaticFunc();
            Expression<Func<int>> pubStaticPropExpression = () => ScopeClass.PubStaticProp;
            Expression<Func<int>> pubStaticFieldExpression = () => ScopeClass.PubStaticField;
            Expression<Func<int>> intStaticFuncExpression = () => ScopeClass.IntStaticFunc();
            Expression<Func<int>> intStaticPropExpression = () => ScopeClass.IntStaticProp;
            Expression<Func<int>> intStaticFieldExpression = () => ScopeClass.IntStaticField;

            var pubFuncValue = serializer.SerializeText(pubFuncExpression);
            var pubPropValue = serializer.SerializeText(pubPropExpression);
            var pubFieldValue = serializer.SerializeText(pubFieldExpression);
            var intFuncValue = serializer.SerializeText(intFuncExpression);
            var intPropValue = serializer.SerializeText(intPropExpression);
            var intFieldValue = serializer.SerializeText(intFieldExpression);
            var pubStaticFuncValue = serializer.SerializeText(pubStaticFuncExpression);
            var pubStaticPropValue = serializer.SerializeText(pubStaticPropExpression);
            var pubStaticFieldValue = serializer.SerializeText(pubStaticFieldExpression);
            var intStaticFuncValue = serializer.SerializeText(intStaticFuncExpression);
            var intStaticPropValue = serializer.SerializeText(intStaticPropExpression);
            var intStaticFieldValue = serializer.SerializeText(intStaticFieldExpression);

            var actualPubFuncExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeText(pubFuncValue);
            var actualPubPropExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeText(pubPropValue);
            var actualPubFieldExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeText(pubFieldValue);
            // the next line throws a MemberNotFoundException in version 2.0.0.0, context is not used
            var actualIntFuncExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeText(intFuncValue, context);
            var actualIntPropExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeText(intPropValue, context);
            var actualIntFieldExpression = (Expression<Func<ScopeClass, bool>>)serializer.DeserializeText(intFieldValue, context);
            var actualPubStaticFuncExpression = (Expression<Func<int>>)serializer.DeserializeText(pubStaticFuncValue);
            var actualPubStaticPropExpression = (Expression<Func<int>>)serializer.DeserializeText(pubStaticPropValue);
            var actualPubStaticFieldExpression = (Expression<Func<int>>)serializer.DeserializeText(pubStaticFieldValue);
            // the next line throws a MemberNotFoundException in version 2.0.0.0, since 'context' is not used
            var actualIntStaticFuncExpression = (Expression<Func<int>>)serializer.DeserializeText(intStaticFuncValue, context);
            var actualIntStaticPropExpression = (Expression<Func<int>>)serializer.DeserializeText(intStaticPropValue, context);
            var actualIntStaticFieldExpression = (Expression<Func<int>>)serializer.DeserializeText(intStaticFieldValue, context);

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