using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using System;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.Issues
{
    // https://github.com/esskar/Serialize.Linq/issues/150
    [TestClass]
    public class Issue150
    {
        [TestMethod]
        public void InfiniteCycleCase()
        {
            JsonSerializer jsonSerializer = new(); jsonSerializer.AddKnownTypes(new Type[] { typeof(MyClass1) }); 
            ExpressionSerializer expressionSerializer = new(jsonSerializer); 
            
            var myClass = new MyClass1(); 
            var serializedText = expressionSerializer.SerializeText(() => myClass.DoSomething(6)); 
            expressionSerializer.DeserializeText(serializedText); 
        }

        [TestMethod]
        public void ArgumentExceptionCase()
        {
            JsonSerializer jsonSerializer = new(); jsonSerializer.AddKnownTypes(new Type[] { typeof(MyClass2) });
            ExpressionSerializer expressionSerializer = new(jsonSerializer);

            var myClass = new MyClass2();
            var serializedText = expressionSerializer.SerializeText(() => myClass.DoSomething(6));
            expressionSerializer.DeserializeText(serializedText);
        }

        public class MyClass1
        {
            public static MyClass1 MyClassInstance = new();

            public void DoSomething(int number)
            {
                //Console.WriteLine(number);
            }
        }

        public class MyClass2
        {
            public int MyInt;

            public void DoSomething(int number)
            {
                //Console.WriteLine(number);
            }
        }
    }
}
