using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using FubuCore;

namespace FubuMVC.Swank.Extensions
{
    public static class StringExtensions
    {
        public static string InitialCap(this string value)
        {
            if (value.IsEmpty()) return value;
            return value[0].ToString().ToUpper() + (value.Length > 1 ? value.Substring(1) : "");
        }

        public static string Hash(this string value)
        {
            using (var hash = MD5.Create())
                return hash.ComputeHash(Encoding.Unicode.GetBytes(value)).ToHex().ToLower();
        }

        private static string ToHex(this IEnumerable<byte> bytes)
        {
            return bytes.Select(b => string.Format("{0:X2}", b)).Aggregate((a, i) => a + i);
        }

        public static string EnusureStartsWith(this string value, string prefix)
        {
            return value.StartsWith(prefix) ? value : prefix + value;
        }

        public static string Join<T>(this IEnumerable<T> items, Func<T, object> item, string initialValue, string seperator, string @default)
        {
            return items != null && items.Any() ? initialValue + items.Select(item).Aggregate((a, i) => a + seperator + i) : @default;
        }

        public static string Repeat(this string value, int count)
        {
            if (string.IsNullOrEmpty(value)) return value;
            var buffer = "";
            for (var i = 0; i < count; i++) buffer += value;
            return buffer;
        }

        public static string ConvertNbspHtmlEntityToSpaces(this string text)
        {
            return Regex.Replace(text, "&nbsp;", " ", RegexOptions.IgnoreCase);
        }

        public static string ConvertBrHtmlTagsToLineBreaks(this string text)
        {
            return Regex.Replace(text, "<br\\s?\\/?>", "\r\n", RegexOptions.IgnoreCase);
        }

        public static string Flatten(this string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .Aggregate((a, i) => a + i);
        }
    }
}