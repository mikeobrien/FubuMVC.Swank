using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Registration.Routes;
using MarkdownSharp;

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

        public static bool IsUrlParameter(this PropertyInfo property, ActionCall action)
        {
            return action.ParentChain().Route.Input.RouteParameters.Any(x => x.Name == property.Name);
        }

        public static bool IsQuerystring(this PropertyInfo property, ActionCall action)
        {
            return !action.ParentChain().Route.Input.RouteParameters.Any(x => x.Name == property.Name) && 
                (property.GetCustomAttribute<QueryStringAttribute>() != null ||
                 !action.ParentChain().Route.AllowedHttpMethods.Any(x => 
                     x.Equals("post", StringComparison.OrdinalIgnoreCase) ||
                     x.Equals("put", StringComparison.OrdinalIgnoreCase)));
        }

        public static bool IsList(this Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>)) || 
                type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof (IList<>));
        }

        public static Type AsGenericList(this Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>)) ? type :
                   type.GetInterfaces().First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));
        }

        public static Type GetElementTypeOrDefault(this Type type)
        {
            return type.IsArray ? type.GetElementType() : type.IsList() ? type.AsGenericList().GetGenericArguments()[0] : type;
        }

        public static void AddService<T>(this ServiceGraph services, Type concreteType, params object[] dependencies)
        {
            var objectDef = new ObjectDef(concreteType);
            dependencies.Where(x => x != null).ToList().ForEach(objectDef.DependencyByValue);
            services.AddService(typeof(T), objectDef);
        }

        private static readonly Func<Type, FieldInfo[]> CachedEnumValues =
            Func.Memoize<Type, FieldInfo[]>(x => x.GetFields(BindingFlags.Public | BindingFlags.Static));

        public static FieldInfo[] GetCachedEnumValues(this Type type)
        {
            return CachedEnumValues(type);
        }

        private readonly static Func<Assembly, string[]> GetEmbeddedResources =
            Func.Memoize<Assembly, string[]>(a => a.GetManifestResourceNames());

        public static string FindTextResourceNamed(this Assembly assembly, params string[] names)
        {
            return FindTextResourceNamed(assembly, names.AsEnumerable());
        }

        public static string FindTextResourceNamed(this Assembly assembly, IEnumerable<string> names)
        {
            var textResources = names.Select(x => new[] { ".txt", ".html", ".md" }
                .Select(y => x + y)).SelectMany(x => x).ToList();
            var resourceName = GetEmbeddedResources(assembly)
                .FirstOrDefault(x => textResources.Any(y => y.Equals(x)));
            if (resourceName == null) return null;
            var text = assembly.GetManifestResourceStream(resourceName).ReadToEnd();
            return resourceName.EndsWith(".md") ? new Markdown().Transform(text).Trim() : text;
        }

        public static string ReadToEnd(this Stream stream)
        {
            using (stream)
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static string ToFriendlyName(this Type type)
        {
            if (type == typeof(Decimal)) return "decimal";
            if (type == typeof(Double)) return "double";
            if (type == typeof(Single)) return "float";
            if (type == typeof(Byte)) return "byte";
            if (type == typeof(SByte)) return "sbyte";
            if (type == typeof(Int16)) return "short";
            if (type == typeof(UInt16)) return "ushort";
            if (type == typeof(Int32)) return "int";
            if (type == typeof(UInt32)) return "uint";
            if (type == typeof(Int64)) return "long";
            if (type == typeof(UInt64)) return "ulong";
            if (type == typeof(String)) return "string";
            if (type == typeof(Boolean)) return "boolean";
            if (type == typeof(DateTime)) return "datetime";
            if (type == typeof(TimeSpan)) return "duration";
            if (type == typeof(Guid)) return "uuid";
            if (type == typeof(Char)) return "char";
            if (type == typeof(byte[])) return "binary";
            if (type.IsEnum) return "enum";
            return null;
        }

        public static string Hash(this string value)
        {
            using (var hash = MD5.Create())
                return hash.ComputeHash(Encoding.Unicode.GetBytes(value)).ToHex();
        }
        
        public static string ToHex(this byte[] bytes)
        {
            return bytes.Select(b => string.Format("{0:X2}", b)).Aggregate((a, i) => a + i);
        }
    }
}