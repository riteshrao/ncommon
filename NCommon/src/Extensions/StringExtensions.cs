using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Collections.Generic;

namespace NCommon.Extensions
{
    public static class StringExtension
    {
        private static readonly Regex WebUrlExpression = new Regex(@"(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex EmailExpression = new Regex(@"^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex StripHTMLExpression = new Regex("<\\S[^><]*>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        private static readonly char[] IllegalUrlCharacters = new[] { ';', '/', '\\', '?', ':', '@', '&', '=', '+', '$', ',', '<', '>', '#', '%', '.', '!', '*', '\'', '"', '(', ')', '[', ']', '{', '}', '|', '^', '`', '~', '–', '‘', '’', '“', '”', '»', '«' };

        [DebuggerStepThrough]
        public static bool IsWebUrl(this string target)
        {
            return !string.IsNullOrEmpty(target) && WebUrlExpression.IsMatch(target);
        }

        [DebuggerStepThrough]
        public static bool IsEmail(this string target)
        {
            return !string.IsNullOrEmpty(target) && EmailExpression.IsMatch(target);
        }

        [DebuggerStepThrough]
        public static string NullSafe(this string target)
        {
            return (target ?? string.Empty).Trim();
        }

        [DebuggerStepThrough]
        public static string FormatWith(this string target, params object[] args)
        {
            Guard.IsNotEmpty(target, "target");

            return string.Format(Constants.CurrentCulture, target, args);
        }

        [DebuggerStepThrough]
        public static string Hash(this string target)
        {
            Guard.IsNotEmpty(target, "target");

            using (MD5 md5 = MD5.Create())
            {
                byte[] data = Encoding.Unicode.GetBytes(target);
                byte[] hash = md5.ComputeHash(data);

                return Convert.ToBase64String(hash);
            }
        }

        [DebuggerStepThrough]
        public static string WrapAt(this string target, int index)
        {
            const int DotCount = 3;

            Guard.IsNotEmpty(target, "target");
            Guard.IsNotNegativeOrZero(index, "index");

            return (target.Length <= index) ? target : string.Concat(target.Substring(0, index - DotCount), new string('.', DotCount));
        }

        [DebuggerStepThrough]
        public static string StripHtml(this string target)
        {
            return StripHTMLExpression.Replace(target, string.Empty);
        }

        [DebuggerStepThrough]
        public static Guid ToGuid(this string target)
        {
            Guid result = Guid.Empty;

            if ((!string.IsNullOrEmpty(target)) && (target.Trim().Length == 22))
            {
                string encoded = string.Concat(target.Trim().Replace("-", "+").Replace("_", "/"), "==");

                try
                {
                    byte[] base64 = Convert.FromBase64String(encoded);

                    result = new Guid(base64);
                }
                catch (FormatException)
                {
                }
            }

            return result;
        }

        [DebuggerStepThrough]
        public static T ToEnum<T>(this string target, T defaultValue) where T : IComparable, IFormattable
        {
            T convertedValue = defaultValue;

            if (!string.IsNullOrEmpty(target))
            {
                try
                {
                    convertedValue = (T)Enum.Parse(typeof(T), target.Trim(), true);
                }
                catch (ArgumentException)
                {
                }
            }

            return convertedValue;
        }

        [DebuggerStepThrough]
        public static string ToLegalUrl(this string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                return target;
            }

            target = target.Trim();

            if (target.IndexOfAny(IllegalUrlCharacters) > -1)
            {
                foreach (char character in IllegalUrlCharacters)
                {
                    target = target.Replace(character.ToString(Constants.CurrentCulture), string.Empty);
                }
            }

            target = target.Replace(" ", "-");

            while (target.Contains("--"))
            {
                target = target.Replace("--", "-");
            }

            return target;
        }

        [DebuggerStepThrough]
        public static string UrlEncode(this string target)
        {
            return HttpUtility.UrlEncode(target);
        }

        [DebuggerStepThrough]
        public static string UrlDecode(this string target)
        {
            return HttpUtility.UrlDecode(target);
        }

        [DebuggerStepThrough]
        public static string AttributeEncode(this string target)
        {
            return HttpUtility.HtmlAttributeEncode(target);
        }

        [DebuggerStepThrough]
        public static string HtmlEncode(this string target)
        {
            return HttpUtility.HtmlEncode(target);
        }

        [DebuggerStepThrough]
        public static string HtmlDecode(this string target)
        {
            return HttpUtility.HtmlDecode(target);
        }

        public static string Replace(this string target, ICollection<string> oldValues, string newValue)
        {
            oldValues.ForEach(oldValue => target = target.Replace(oldValue, newValue));
            return target;
        }

        /// <summary>
        /// Trims the leading and trailing spaces from the input string, and converts an empty string to null.
        /// </summary>
        /// <param name="inputString">the input string to be converted</param>
        /// <returns>The converted string</returns>
        public static string ToNullIfEmptyOrBlank(this string inputString)
        {
            if (inputString == null || (inputString = inputString.Trim()).Length == 0) return null;

            return inputString;
        }

        /// <summary>
        ///  Tests if string is null, empty, or all blanks
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static bool IsNullOrEmptyOrBlank(this string inputString)
        {
            return (inputString == null || inputString.Trim().Length == 0);
        }


        /// <summary>
        ///  Tests if the inputString string is numeric
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if <paramref name="inputString"/> is null</exception>
        public static bool IsNumeric(this string inputString)
        {
            Guard.IsNotNull(inputString, "inputString");

            foreach (char c in inputString)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Converts a string to a byte array using ASCII encoding
        /// </summary>
        /// <param name="stringToConvert">The string to be converted</param>
        /// <returns>Byte Array containing data from input string</returns>
        /// <exception cref="ArgumentNullException">if <paramref name="input"/> is null</exception>
        public static byte[] ToByteArray(this string stringToConvert)
        {
            Guard.IsNotNull(stringToConvert, "stringToConvert");

            byte[] bytes = Encoding.ASCII.GetBytes(stringToConvert);
            return bytes;
        }


        /// <summary>
        /// Remove non-digit chars from inputString string
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if <paramref name="inputString"/> is null</exception>
        public static string ReturnNumericCharsOnly(this string inputString)
        {
            Guard.IsNotNull(inputString, "inputString");

            // use StringBuilder for efficient string concat
            StringBuilder sb = new StringBuilder(string.Empty);

            foreach (char c in inputString)
            {
                if (char.IsDigit(c))
                {
                    sb.Append(c);
                }
            }

            // return only numbers
            return sb.ToString();
        }


        /// <summary>
        /// Remove non-digit chars (including the comma separate) but keep 
        /// decimal point from inputString string
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if <paramref name="inputString"/> is null</exception>
        public static string ReturnDecimalCharsExcludingCommaSeparaters(this string inputString)
        {
            Guard.IsNotNull(inputString, "inputString");

            // use StringBuilder for efficient string concat
            StringBuilder sb = new StringBuilder(string.Empty);

            foreach (char c in inputString)
            {
                if (char.IsDigit(c) || c == '.' || c == '-')
                {
                    sb.Append(c);
                }
            }

            // Return only numbers, the negative sign and decimal point.
            return sb.ToString();
        }

        /// <summary>
        /// Remove non-alpha chars from inputString string
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if <paramref name="inputString"/> is null</exception>
        public static string ReturnAlphaCharsOnly(this string inputString)
        {
            Guard.IsNotNull(inputString, "inputString");

            // use StringBuilder for efficient string concat
            StringBuilder sb = new StringBuilder();

            foreach (char c in inputString)
            {
                if (char.IsLetter(c))
                {
                    sb.Append(c);
                }
            }

            // return only alpha chars
            return sb.ToString();
        }

        /// <summary>
        /// Remove non-alpha chars from inputString string
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if <paramref name="inputString"/> is null</exception>
        public static string ReturnAlphanumericCharsOnly(this string inputString)
        {
            Guard.IsNotNull(inputString, "inputString");

            // use StringBuilder for efficient string concat
            StringBuilder sb = new StringBuilder();

            foreach (char c in inputString)
            {
                if (char.IsLetter(c) || char.IsDigit(c))
                {
                    sb.Append(c);
                }
            }

            // return only alphanumeric chars
            return sb.ToString();
        }

        /// <summary>
        /// Pad with leading zero if needed.
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="maxLen"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if <paramref name="inputString"/> is null</exception>
        public static string PadWithLeadingZeros(this string inputString, int maxLen)
        {
            Guard.IsNotNull(inputString, "inputString");
            return inputString.PadLeft(maxLen, '0');
        }

		/// <summary>
		/// Allows for culture and case sensitive and insensitive string searching.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="value"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static bool Contains(this string target, string value, StringComparison comparer)
		{
			return target.IndexOf(value, 0, comparer) >= 0;
		}

		/// <summary>
		/// Allows for culture and case sensitive and insensitive string searching.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="value"></param>
		/// <param name="ignoreCase"></param>
		/// <returns></returns>
		public static bool Contains(this string target, string value, bool ignoreCase)
		{
			if (ignoreCase)
			{
				return Contains(target, value, StringComparison.InvariantCultureIgnoreCase);
			}
			else
			{
				return target.Contains(value); // It would be silly to use this method in this case, but we account for silliness.
			}
		}

    }
}
