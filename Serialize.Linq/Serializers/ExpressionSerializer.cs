using System;
using Serialize.Linq.Interfaces;

namespace Serialize.Linq.Serializers
{
    public class ExpressionSerializer
    {
        private readonly ISerializer _serializer;

        public ExpressionSerializer(ISerializer serializer)
        {
            if(serializer == null)
                throw new ArgumentNullException("serializer");
            _serializer = serializer;
        }


    }
}