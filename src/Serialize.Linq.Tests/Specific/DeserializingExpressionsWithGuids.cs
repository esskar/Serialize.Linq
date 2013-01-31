using System;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serialize.Linq.Serializers;

namespace Serialize.Linq.Tests.Specific
{
	[TestClass]
	public class DeserializingExpressionsWithGuids
	{
		[TestMethod]
		public void SerializeAndDeserializeSimpleGuidExpression()
		{
			var serializer = new ExpressionSerializer(new JsonSerializer());

			Expression<Func<Guid, bool>> exp = g => Guid.NewGuid() != g;

			var result = serializer.SerializeText(exp);
			Assert.IsNotNull(result);

			var deserialized = serializer.DeserializeText(result);
			Assert.IsNotNull(deserialized);
		}

		[TestMethod]
		public void SerializeAndDeserializeAMoreComplexExpression()
		{
			Expression<Func<Model, bool>> exp = model => model.Id != Guid.Empty;

			var serializer = new ExpressionSerializer(new JsonSerializer());
			var json = serializer.SerializeText(exp);

			var deserializedExp = serializer.DeserializeText(json);
			Assert.IsNotNull(deserializedExp);
		}

		class Model
		{
			public Guid Id { get; set; }
		}
	}
}