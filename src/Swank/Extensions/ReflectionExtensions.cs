using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
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

        public static string GetHash(this System.Type type)
        {
            return type.FullName.Hash();
        }

        public static string GetHash(this System.Type type, MethodInfo method)
        {
            return (method.DeclaringType.FullName + "." + method.Name + "." + type.FullName).Hash();
        }

        public static bool IsSystemType(this System.Type type)
        {
            return type.FullName.StartsWith("System.");
        }

        private static readonly System.Type[] ListTypes = new[] { typeof(IList<>), typeof(List<>) };

        public static bool IsListType(this System.Type type)
        {
            var genericTypeDef = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
            return ListTypes.Any(x => genericTypeDef == x);
        }

        public static bool ImplementsListType(this System.Type type)
        {
            return type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));
        }

        public static bool InheritsFromListType(this System.Type type)
        {
            return !type.IsListType() && type.ImplementsListType();
        }

        public static bool IsList(this System.Type type)
        {
            return type.IsListType() || type.ImplementsListType();
        }

        public static System.Type GetListInterface(this System.Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IList<>)) ? type :
                   type.GetInterfaces().First(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));
        }

        public static System.Type GetListElementType(this System.Type type)
        {
            return type.IsArray ? type.GetElementType() : type.IsList() ? type.GetListInterface().GetGenericArguments()[0] : null;
        }

        private static readonly Func<System.Type, FieldInfo[]> CachedEnumValues =
            Func.Memoize<System.Type, FieldInfo[]>(x => x.GetFields(BindingFlags.Public | BindingFlags.Static));

        public static FieldInfo[] GetCachedEnumValues(this System.Type type)
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

        private static string ReadToEnd(this Stream stream)
        {
            using (stream)
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static string ToFriendlyName(this System.Type type)
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
        
        private static string ToHex(this byte[] bytes)
        {
            return bytes.Select(b => string.Format("{0:X2}", b)).Aggregate((a, i) => a + i);
        }
    }
}