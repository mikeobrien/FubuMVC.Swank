using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MarkdownSharp;

namespace FubuMVC.Swank.Extensions
{
    public static class ReflectionExtensions
    {
        public static T GetCustomAttribute<T>(this MemberInfo memberInfo)
        {
            return memberInfo.GetCustomAttributes<T>().FirstOrDefault();
        }

        public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo memberInfo)
        {
            return memberInfo.GetCustomAttributes(true).OfType<T>();
        }

        public static string GetHash(this Type type)
        {
            return type.FullName.Hash();
        }

        public static string GetHash(this Type type, MethodInfo method)
        {
            return (method.DeclaringType.FullName + "." + method.Name + "." + type.FullName).Hash();
        }

        public static bool IsSystemType(this Type type)
        {
            return type.FullName.StartsWith("System.");
        }

        private static readonly Type[] ListTypes = new[] { typeof(IList<>), typeof(List<>) };

        public static bool IsListType(this Type type)
        {
            var genericTypeDef = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
            return ListTypes.Any(x => genericTypeDef == x);
        }

        public static bool ImplementsListType(this Type type)
        {
            return type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));
        }

        public static bool InheritsFromListType(this Type type)
        {
            return !type.IsListType() && type.ImplementsListType();
        }

        public static bool IsList(this Type type)
        {
            return type.IsListType() || type.ImplementsListType();
        }

        public static Type GetListInterface(this Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>)) ? type :
                   type.GetInterfaces().First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));
        }

        public static Type GetListElementType(this Type type)
        {
            return type.IsArray ? type.GetElementType() : type.IsList() ? type.GetListInterface().GetGenericArguments()[0] : null;
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
                .FirstOrDefault(x => textResources.Any(y => y.StartsWith("*") 
                        ? x.EndsWith(y.Substring(1), StringComparison.OrdinalIgnoreCase) 
                        : y.Equals(x, StringComparison.OrdinalIgnoreCase)));
            if (resourceName == null) return null;
            var text = assembly.GetManifestResourceStream(resourceName).ReadToEnd();
            return resourceName.EndsWith(".md") ? new Markdown().Transform(text).Trim() : text;
        }

        private static string ReadToEnd(this Stream stream)
        {
            using (stream)
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static string GetXmlName(this Type type)
        {
            if (type == typeof(String)) return "string";
            if (type == typeof(Boolean) || type == typeof(Boolean?)) return "boolean";
            if (type == typeof(Decimal) || type == typeof(Decimal?)) return "decimal";
            if (type == typeof(Double) || type == typeof(Double?)) return "double";
            if (type == typeof(Single) || type == typeof(Single?)) return "float";
            if (type == typeof(Byte) || type == typeof(Byte?)) return "unsignedByte";
            if (type == typeof(SByte) || type == typeof(SByte?)) return "byte";
            if (type == typeof(Int16) || type == typeof(Int16?)) return "short";
            if (type == typeof(UInt16) || type == typeof(UInt16?)) return "unsignedShort";
            if (type == typeof(Int32) || type == typeof(Int32?)) return "int";
            if (type == typeof(UInt32) || type == typeof(UInt32?)) return "unsignedInt";
            if (type == typeof(Int64) || type == typeof(Int64?)) return "long";
            if (type == typeof(UInt64) || type == typeof(UInt64?)) return "unsignedLong";
            if (type == typeof(DateTime) || type == typeof(DateTime?)) return "dateTime";
            if (type == typeof(Guid) || type == typeof(Guid?)) return "guid";
            if (type == typeof(Char) || type == typeof(Char?)) return "char";
            if (type == typeof(byte[])) return "base64Binary";
            if (type.IsArray || type.IsList()) return 
                "ArrayOf" + type.GetListElementType().GetXmlName().InitialCap();
            return type.Name;
        }
    }
}