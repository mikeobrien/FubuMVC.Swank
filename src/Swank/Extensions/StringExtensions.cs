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
    }
}