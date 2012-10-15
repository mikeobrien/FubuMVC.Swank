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
    }
}