using System;
using System.Text.RegularExpressions;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace RedMonkey.Extensions
{
    public static class ConversionExtensions
    {
        public static bool ToBool(this object val, string trueRegex = "(?i)^ON|YES|Y|TRUE|T|1$", bool nullIsFalse = true, bool emptyStringIsFalse = true)
        {
            if (val is bool b)
                return b;
            if (val == null)
                return !nullIsFalse;
            var strVal = val.ToStringSafe(string.Empty);
            if (strVal == string.Empty) return !emptyStringIsFalse;
            var trueReg = new Regex(trueRegex);
            return trueReg.IsMatch(strVal);
        }

        public static double ToDouble(this object val, double defaultValue = default)
        {
            return GenericTo(val, defaultValue, double.TryParse);
        }

        public static decimal ToDecimal(this object val, decimal defaultValue = default)
        {
            return GenericTo(val, defaultValue, decimal.TryParse);
        }

        public static int ToInt(this object val, int defaultValue = default)
        {
            if (val is Enum)
                // ReSharper disable once PossibleInvalidCastException
                return (int) val;
            var dblVal = val.ToDouble();
            return GenericTo(Math.Round(dblVal, 0).ToStringSafe(string.Empty), defaultValue, int.TryParse);
        }

        public static long ToLong(this object val, long defaultValue = default)
        {
            var dblVal = val.ToDouble();
            return GenericTo(Math.Round(dblVal, 0).ToStringSafe(string.Empty), defaultValue, long.TryParse);
        }

        public static DateTime ToDateTime(this object val, DateTime defaultValue = default)
        {
            return GenericTo(val, defaultValue, DateTime.TryParse);
        }

        private static T GenericTo<T>(object input, T defaultValue, TryParseHandler<T> tryParseMethod)
        {
            if (input is T t)
                return t;
            if (input == null)
                return defaultValue;
            if (tryParseMethod(input.ToStringSafe(string.Empty), out var parsed))
                return parsed;
            return defaultValue;
        }

        public static byte ToByte(this object val, byte defaultValue = default)
        {
            if (val == null)
                return defaultValue;
            try
            {
                return Convert.ToByte(val);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static T ToEnum<T>(this string val, bool ignoreCase = true, T defaultValue = default) where T : Enum
        {
            return TryToEnum<T>(val, out var result, ignoreCase) ? result : defaultValue;
        }

        public static bool TryToEnum<T>(this string val, out T result, bool ignoreCase = true) where T : Enum
        {
            if (Enum.TryParse(typeof(T), val, ignoreCase, out var resultObject))
            {
                result = (T) resultObject;
                return true;
            }

            result = default;
            return false;
        }

        public static bool TryToString(this object input, out string output)
        {
            output = default;
            if (input == null)
                return false;
            output = input.ToString();
            return true;
        }

        public static string ToStringSafe(this object input, string defaultValue = null)
        {
            return TryToString(input, out var toStringResult) ? toStringResult : defaultValue;
        }

        private delegate bool TryParseHandler<T>(string value, out T result);
    }
}