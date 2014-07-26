using System;
using System.IO;
using System.Reflection;

namespace FubuMVC.Swank.Extensions
{

    public static class AssemblyExtensions
    {
        public static string GetResourceString(this Assembly assembly, string resourceName)
        {
            return GetResourceReader(assembly, resourceName).ReadToEnd();
        }

        public static StreamReader GetResourceReader(this Assembly assembly, string resourceName)
        {
            return new StreamReader(assembly.GetManifestResourceStream(
                    ResolveResourceNamespace(assembly, resourceName)));
        }

        private static string ResolveResourceNamespace(this Assembly assembly, string resourceName)
        {
            var resourceNames = assembly.GetManifestResourceNames();
            var currentResourceName = string.Empty;

            foreach (string resource in resourceNames)
            {
                if (resource.ToLower().EndsWith(resourceName.ToLower()) &&
                    ((currentResourceName == string.Empty) |
                     (resource.Length < currentResourceName.Length)))
                    currentResourceName = resource;
            }

            return currentResourceName;
        }
    }
}