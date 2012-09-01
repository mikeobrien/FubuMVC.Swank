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
    }
}