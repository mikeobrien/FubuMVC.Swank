using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
                return hash.ComputeHash(Encoding.Unicode.GetBytes(value)).ToHex();
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
    }
}