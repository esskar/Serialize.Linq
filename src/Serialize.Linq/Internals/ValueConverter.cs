#region Copyright
//  Copyright, Sascha Kiefer (esskar)
//  Released under LGPL License.
//  
//  License: https://raw.github.com/esskar/Serialize.Linq/master/LICENSE
//  Contributing: https://github.com/esskar/Serialize.Linq
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
#if !WINDOWS_PHONE
using System.Collections.Concurrent;
using System.Globalization;
using System.Xml;
#endif
#if NETSTANDARD || WINDOWS_UWP
using System.Reflection;
#endif
using System.Text.RegularExpressions;

namespace Serialize.Linq.Internals
{
    public static class ValueConverter
    {
        private static readonly ConcurrentDictionary<Type, Func<object, Type, object>> _userDefinedConverters;
        private static readonly Regex _dateRegex = new Regex(@"/Date\((?<date>-?\d+)((?<offsign>[-+])((?<offhours>\d{2})(?<offminutes>\d{2})))?\)/"
#if !NETSTANDARD
            , RegexOptions.Compiled | RegexOptions.ExplicitCapture
#else
            , RegexOptions.ExplicitCapture
#endif
            );
        private static readonly DateTime _utcEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime _localEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

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
                throw new ArgumentNullException(nameof(convertTo));

            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

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
                return convertTo.IsValueType() ? Activator.CreateInstance(convertTo) : null;

            if (convertTo.IsInstanceOfType(value))
                return value;

            if (TryCustomConvert(value, convertTo, out var retval))
                return retval;

            if (TryConvertToDateTime(value, convertTo, out var retDateTime))
                return retDateTime;

            if (TryConvertToTimeSpan(value, convertTo, out var retTimeSpan))
                return retTimeSpan;

            if (TryConvertToEnum(value, convertTo, out var retEnum))
                return Enum.ToObject(convertTo, retEnum);

            if (TryConvertToArray(value, convertTo, out var retArray))
                return retArray;

            if (convertTo.IsGenericType())
            {
                // convert nullable types
                var genericTypeDefinition = convertTo.GetGenericTypeDefinition();
                if (TryConvertToNullable(value, convertTo, out var retNullable))
                    return retNullable;

                if (TryConvertToList(value, convertTo, out var retList))
                    return retList;
            }

            if (TryConvertibleConversion(value, convertTo, out var retConvertible))
            {
                return retConvertible;
            }

            // fallback
            return Activator.CreateInstance(convertTo, value);
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
            if (_userDefinedConverters.TryGetValue(convertTo, out var converter) || _userDefinedConverters.TryGetValue(typeof(void), out converter))
            {
                convertedValue = converter(value, convertTo);
                return true;
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
        private static bool TryConvertToDateTime(object sourceDate, Type targetType, out DateTime targetDate)
        {
            string sourceString;
            Match sourceMatch;
            long fromEpochMSec;
            int offHours;
            int offMinutes;
            int offSign;
            DateTime tempDate;

            if (targetType == typeof(DateTime))
            {
                sourceString = sourceDate.ToString();
                sourceMatch = _dateRegex.Match(sourceString);
                if (DateTime.TryParse(sourceString, CultureInfo.InvariantCulture, DateTimeStyles.None, out targetDate) || DateTime.TryParse(sourceString, CultureInfo.CurrentCulture, DateTimeStyles.None, out targetDate))
                    return true;
                else if (sourceMatch.Success && Int64.TryParse(sourceMatch.Groups["date"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out fromEpochMSec))
                {
                    // Improvement: conversion takes account of local and UTC time
                    if (sourceMatch.Groups["offsign"].Success)
                    {
                        offHours = Int32.Parse(sourceMatch.Groups["offhours"].Value, NumberStyles.None, CultureInfo.InvariantCulture);
                        offMinutes = Int32.Parse(sourceMatch.Groups["offminutes"].Value, NumberStyles.None, CultureInfo.InvariantCulture);
                        offSign = sourceMatch.Groups["offsign"].Value == "-" ? -1 : 1;
                        tempDate = _localEpoch.AddMilliseconds(fromEpochMSec).AddHours(offHours * offSign).AddMinutes(offMinutes * offSign);
                        targetDate = DateTime.Parse(String.Format(CultureInfo.InvariantCulture,
                                                                  "{0:yyyy-MM-ddTHH:mm:ss.fff}{1}{2:00}:{3:00}", 
                                                                  tempDate, sourceMatch.Groups["offsign"], offHours, offMinutes), 
                                                    CultureInfo.InvariantCulture, 
                                                    DateTimeStyles.AssumeLocal);
                    }
                    else
                    {
                        tempDate = _utcEpoch.AddMilliseconds(fromEpochMSec);
                        targetDate = DateTime.Parse(String.Format(CultureInfo.InvariantCulture, 
                                                                  "{0:yyyy-MM-ddTHH:mm:ss.fff}Z", 
                                                                  tempDate), 
                                                    CultureInfo.InvariantCulture, 
                                                    DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
                        // without DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal is targetDate.Kind = DateTimeKind.Local,
                        // see https://stackoverflow.com/questions/1756639/why-cant-datetime-parse-parse-utc-date
                    }
                    return true;
                }
                else
                {
                    targetDate = DateTime.MinValue;
                    return false;
                }
            }
            else
            {
                targetDate = DateTime.MinValue;
                return false;
            }
        }

        private static bool TryConvertToTimeSpan(object sourceSpan, Type targetType, out TimeSpan targetSpan)
        {
            string sourceSpanString;

            if (targetType == typeof(TimeSpan))
            {
                sourceSpanString = sourceSpan.ToString();
                if (String.IsNullOrEmpty(sourceSpanString))
                {
                    targetSpan = TimeSpan.Zero;

                    return true;
                }
                else if (TimeSpan.TryParse(sourceSpanString, CultureInfo.InvariantCulture, out targetSpan))
                    return true;
                else
                    try
                    {
                        targetSpan = XmlConvert.ToTimeSpan(sourceSpanString);

                        return true;
                    }
                    catch
                    {
                        targetSpan = TimeSpan.MinValue;
                        return false;
                    }
            }
            else
            {
                targetSpan = TimeSpan.MinValue;
                return false;
            }
        }

        private static bool TryConvertToEnum(object sourceEnum, Type targetType, out object targetEnum)
        {
            if (targetType.IsEnum())
            {
                try
                {
                    targetEnum = Enum.ToObject(targetType, sourceEnum);

                    return true;
                }
                catch
                {
                    targetEnum = null;
                    return false;
                }
            }
            else
            {
                targetEnum = null;
                return false;
            }
        }

        private static bool TryConvertToArray(object sourceArray, Type targetType, out Array targetArray)
        {
            Type targetItemType;
            Array tempSourceArray;

            if (targetType.IsArray && sourceArray.GetType().IsArray)
            {
                targetItemType = targetType.GetElementType();
                if (targetItemType == null)
                    throw new InvalidOperationException("Cannot build array with an unkown element type.");
                tempSourceArray = (Array)sourceArray;
                targetArray = Array.CreateInstance(targetItemType, tempSourceArray.Length);

                for (int tmpCtr = 0; tmpCtr <= tempSourceArray.Length - 1; tmpCtr++)
                    targetArray.SetValue(Convert(tempSourceArray.GetValue(tmpCtr), targetItemType), tmpCtr);

                return true;
            }
            else
            {
                targetArray = null;
                return false;
            }
        }

        private static bool TryConvertToList(object sourceList, Type targetType, out IList targetList)
        {
            Type targetItemType;
            IEnumerable tempSourceList;

            tempSourceList = sourceList as IEnumerable;
            if (tempSourceList != null && targetType.GetGenericTypeDefinition() == typeof(List<>))
            {
                targetList = (IList)Activator.CreateInstance(targetType);
                targetItemType = targetType.GetGenericArguments()[0];

                foreach (object tmpSourceItem in tempSourceList)
                    targetList.Add(Convert(tmpSourceItem, targetItemType));

                return true;
            }
            else
            {
                targetList = null;
                return false;
            }
        }

        private static bool TryConvertToNullable(object sourceNullable, Type targetType, out object targetNullable)
        {
            Type[] argumentTypes;

            if (targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                argumentTypes = targetType.GetGenericArguments();
                if (argumentTypes.Length == 1)
                {
                    targetNullable = Convert(sourceNullable, argumentTypes[0]);

                    return true;
                }
            }
            targetNullable = null;
            return false;
        }

        private static bool TryConvertibleConversion(object sourceConvertible, Type targetType, out IConvertible targetConvertible)
        {
            if (sourceConvertible is IConvertible)
            {
                try
                {
                    targetConvertible = (IConvertible)System.Convert.ChangeType(sourceConvertible, targetType, CultureInfo.InvariantCulture);

                    return true;
                }
                catch
                {
                }
            }
            targetConvertible = null;
            return false;
        }
    }
}
