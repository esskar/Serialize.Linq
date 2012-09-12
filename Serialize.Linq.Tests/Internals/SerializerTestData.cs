using System;
using System.Linq.Expressions;

namespace Serialize.Linq.Tests.Internals
{
    internal static class SerializerTestData
    {
        public static readonly SerializerTestItem[] TestItems = new [] {
            new SerializerTestItem { 
                Expression = null, 
                Json = "null", 
                Xml = "<ExpressionNode i:nil=\"true\" xmlns=\"http://schemas.datacontract.org/2004/07/Serialize.Linq.Nodes\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"/>" },
            new SerializerTestItem { 
                Expression = (Expression<Func<bool, bool>>)(b => b), 
                Json = "{\"__type\":\"LambdaExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":18,\"TypeName\":\"System.Func`2[[System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\",\"Body\":{\"__type\":\"ParameterExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":38,\"TypeName\":\"System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\",\"IsByRef\":false,\"Name\":\"b\"},\"Parameters\":[{\"__type\":\"ParameterExpressionNode:#Serialize.Linq.Nodes\",\"NodeType\":38,\"TypeName\":\"System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\",\"IsByRef\":false,\"Name\":\"b\"}]}", 
                Xml = "<ExpressionNode i:type=\"LambdaExpressionNode\" xmlns=\"http://schemas.datacontract.org/2004/07/Serialize.Linq.Nodes\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><NodeType>Lambda</NodeType><TypeName>System.Func`2[[System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</TypeName><Body i:type=\"ParameterExpressionNode\"><NodeType>Parameter</NodeType><TypeName>System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</TypeName><IsByRef>false</IsByRef><Name>b</Name></Body><Parameters><ExpressionNode i:type=\"ParameterExpressionNode\"><NodeType>Parameter</NodeType><TypeName>System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</TypeName><IsByRef>false</IsByRef><Name>b</Name></ExpressionNode></Parameters></ExpressionNode>" }
                                                                       };
    }
}