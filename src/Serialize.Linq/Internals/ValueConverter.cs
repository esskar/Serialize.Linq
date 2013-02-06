using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Serialize.Linq.Internals
{
    public static class ValueConverter
    {
        private static readonly ConcurrentDictionary<Type, Func<object, Type, object>> _userDefinedConverters;
        private static readonly Regex _dateRegex = new Regex(@"/Date\((\d+)([-+])(\d+)\)/", RegexOptions.Compiled);
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        static ValueConverter()
        {
            _userDefinedConverters = new ConcurrentDictionary<Type, Func<object, Type, object>>();
        }

        public static void AddCustomConverter(Type convertTo, Func<object, object> converter)
        {
            AddCustomConverter(convertTo, (v, t) => converter(v));
        }

        public static void AddCustomConverter(Func<object, Type, object> converter)
        {
            AddCustomConverter(typeof(void), converter);
        }

        public static void AddCustomConverter(Type convertTo, Func<object, Type, object> converter)
        {
            if (convertTo == null)
                throw new ArgumentNullException("convertTo");

            if (converter == null)
                throw new ArgumentNullException("converter");

            if (!_userDefinedConverters.TryAdd(convertTo, converter))
                throw new ApplicationException("Failed to add converter.");
        }

        public static void ClearCustomConverters()
        {
            _userDefinedConverters.Clear();
        }

        public static object Convert(object value, Type convertTo)
        {
            if (value == null)
                return convertTo.IsValueType ? Activator.CreateInstance(convertTo) : null;

            if (convertTo.IsInstanceOfType(value))
                return value;

            object retval;
            if (TryCustomConvert(value, convertTo, out retval))
                return retval;

            if (convertTo.IsArray && value.GetType().IsArray)
            {
                var valArray = (Array)value;
                var result = Array.CreateInstance(convertTo.GetElementType(), valArray.Length);
                for (var i = 0; i < valArray.Length; ++i)
                    result.SetValue(valArray.GetValue(i), i);
                return result;
            }

            return Activator.CreateInstance(convertTo, value);
        }
        
        private static bool TryCustomConvert(object value, Type convertTo, out object convertedValue)
        {
            Func<object, Type, object> converter;
            if (_userDefinedConverters.TryGetValue(convertTo, out converter) || _userDefinedConverters.TryGetValue(typeof(void), out converter))
            {
                convertedValue = converter(value, convertTo);
                return true;
            }

            if (convertTo == typeof(DateTime))
            {
                DateTime dateTime;
                if (TryConvertToDateTime(value, out dateTime))
                {
                    convertedValue = dateTime;
                    return true;
                }
            }

            convertedValue = null;
            return false;
        }

        private static bool TryConvertToDateTime(object value, out DateTime dateTime)
        {
            var stringValue = value.ToString();
            if (DateTime.TryParse(stringValue, out dateTime))
                return true;

            var match = _dateRegex.Match(stringValue);
            if (!match.Success)
                return false;

            // try to parse the string into a long. then create a datetime and convert to local time.
            long msFromEpoch;
            if (!long.TryParse(match.Groups[1].Value, out msFromEpoch))
                return false;

            var fromEpoch = TimeSpan.FromMilliseconds(msFromEpoch);

            dateTime = _epoch.Add(fromEpoch);
            return true;
        }
    }
}
