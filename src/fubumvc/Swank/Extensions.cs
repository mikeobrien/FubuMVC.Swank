using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FubuMVC.Core.Registration.Routes;

namespace Swank
{
    public static class Extensions
    {
        public static string GetRouteResource(this IRouteDefinition route)
        {
            return Regex.Replace(route.Pattern, "/*\\{.*?\\}", "").Trim('/');
        }

        public static string FirstPatternSegment(this IRouteDefinition route)
        {
            return route.Pattern.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        }

        public static string ReadToEnd(this Stream stream)
        {
            using (stream)
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}