using System;

namespace Serialize.Linq.Internals
{
    public static class ValueConverter
    {
        public static object Convert(object value, Type type)
        {
            if (value == null)
                return type.IsValueType ? Activator.CreateInstance(type) : null;

            if (type.IsInstanceOfType(value))
                return value;

            if (type.IsArray && value.GetType().IsArray)
            {
                var valArray = (Array)value;
                var result = Array.CreateInstance(type.GetElementType(), valArray.Length);
                for (var i = 0; i < valArray.Length; ++i)
                    result.SetValue(valArray.GetValue(i), i);
                return result;
            }

            return Activator.CreateInstance(type, value);
        }
    }
}
