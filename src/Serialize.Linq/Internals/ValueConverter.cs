#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
#if !WINDOWS_PHONE
using System.Collections.Concurrent;
#endif
using System.Text.RegularExpressions;

namespace Serialize.Linq.Internals
{
    public static class ValueConverter
    {
        private static readonly ConcurrentDictionary<Type, Func<object, Type, object>> _userDefinedConverters;
        private static readonly Regex _dateRegex = new Regex(@"/Date\((?<date>\d+)((?<offsign>[-+])((?<offhours>\d{2})(?<offminutes>\d{2})))?\)/"
#if !SILVERLIGHT
            ,RegexOptions.Compiled
#endif
            );
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Initializes the <see cref="ValueConverter"/> class.
        /// </summary>
        static ValueConverter()
        {
            _userDefinedConverters = new ConcurrentDictionary<Type, Func<object, Type, object>>();
        }

        /// <summary>
        /// Adds the custom converter.
        /// </summary>
        /// <param name="convertTo">The convert to.</param>
        /// <param name="converter">The converter.</param>
        public static void AddCustomConverter(Type convertTo, Func<object, object> converter)
        {
            AddCustomConverter(convertTo, (v, t) => converter(v));
        }

        /// <summary>
        /// Adds the custom converter.
        /// </summary>
        /// <param name="converter">The converter.</param>
        public static void AddCustomConverter(Func<object, Type, object> converter)
        {
            AddCustomConverter(typeof(void), converter);
        }

        /// <summary>
        /// Adds the custom converter.
        /// </summary>
        /// <param name="convertTo">The convert to.</param>
        /// <param name="converter">The converter.</param>
        /// <exception cref="System.ArgumentNullException">
        /// convertTo
        /// or
        /// converter
        /// </exception>
        /// <exception cref="System.ApplicationException">Failed to add converter.</exception>
        public static void AddCustomConverter(Type convertTo, Func<object, Type, object> converter)
        {
            if (convertTo == null)
                throw new ArgumentNullException("convertTo");

            if (converter == null)
                throw new ArgumentNullException("converter");

            if (!_userDefinedConverters.TryAdd(convertTo, converter))
                throw new Exception("Failed to add converter.");
        }

        /// <summary>
        /// Clears the custom converters.
        /// </summary>
        public static void ClearCustomConverters()
        {
            _userDefinedConverters.Clear();
        }

        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="convertTo">The convert to.</param>
        /// <returns></returns>
        public static object Convert(object value, Type convertTo)
        {
            if (value == null)
                return convertTo.IsValueType ? Activator.CreateInstance(convertTo) : null;

            if (convertTo.IsInstanceOfType(value))
                return value;

            object retval;
            if (TryCustomConvert(value, convertTo, out retval))
                return retval;

            if (convertTo.IsEnum)
                return Enum.ToObject(convertTo, value);            

            // convert array types
            if (convertTo.IsArray && value.GetType().IsArray)
            {
                var elementType = convertTo.GetElementType();

                var valArray = (Array)value;
                var result = Array.CreateInstance(elementType, valArray.Length);
                for (var i = 0; i < valArray.Length; ++i)
                    result.SetValue(Convert(valArray.GetValue(i), elementType), i);
                return result;
            }

            // convert nullable types
            if (convertTo.IsGenericType && convertTo.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var argumentTypes = convertTo.GetGenericArguments();
                if (argumentTypes.Length == 1)                
                    value = Convert(value, argumentTypes[0]);                
            }
            
            // TODO: think about a better way; exception could may have an critical impact on performance
            try
            {
#if SILVERLIGHT
                return System.Convert.ChangeType(value, convertTo, System.Threading.Thread.CurrentThread.CurrentCulture);
#else
                return System.Convert.ChangeType(value, convertTo);
#endif
            }
            catch (Exception)
            {
                return Activator.CreateInstance(convertTo, value);
            }            
        }

        /// <summary>
        /// Tries the custom convert.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="convertTo">The convert to.</param>
        /// <param name="convertedValue">The converted value.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Tries the convert to date time.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        private static bool TryConvertToDateTime(object value, out DateTime dateTime)
        {
            var stringValue = value.ToString();
            if (DateTime.TryParse(stringValue, out dateTime))
                return true;

            var match = _dateRegex.Match(stringValue);
            if (!match.Success)
                return false;

            // try to parse the string into a long. then create a datetime.
            long msFromEpoch;
            if (!long.TryParse(match.Groups["date"].Value, out msFromEpoch))
                return false;

            var fromEpoch = TimeSpan.FromMilliseconds(msFromEpoch);
            dateTime = _epoch.Add(fromEpoch);
            return true;
        }
    }
}
