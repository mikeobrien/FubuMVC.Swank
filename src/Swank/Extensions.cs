using System.Text.RegularExpressions;
using FubuMVC.Core.Registration.Routes;

namespace Swank
{
    public static class Extensions
    {
        public static string GetRouteResourceId(this IRouteDefinition route)
        {
            return route.Pattern.StripUrlParameters().Replace('/', '.');
        }

        public static string GetRouteResourceDescription(this IRouteDefinition route)
        {
            return route.Pattern.StripUrlParameters();
        }

        public static string StripUrlParameters(this string routePattern)
        {
            return Regex.Replace(routePattern, "/*\\{.*?\\}", "").Trim('/');
        }
    }
}