using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore;
using MarkdownSharp;

namespace FubuMVC.Swank.Extensions.Compatibility
{
    public static class ReflectionExtensions
    {
        [Obsolete(".NET 4.5 Compatibility")]
        public static T GetCustomAttribute<T>(this MemberInfo memberInfo)
        {
            return memberInfo.GetCustomAttributes<T>().FirstOrDefault();
        }

        [Obsolete(".NET 4.5 Compatibility")]
        public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo memberInfo)
        {
            return memberInfo.GetCustomAttributes(true).OfType<T>();
        }
    }
}

namespace FubuMVC.Swank.Extensions
{
    public static class ReflectionExtensions
    {
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

        public static FieldInfo[] GetEnumOptions(this Type type)
        {
            type = type.IsNullable() ? Nullable.GetUnderlyingType(type) : type;
            return type.GetFields(BindingFlags.Public | BindingFlags.Static);
        }

        private readonly static Func<Assembly, string[]> GetEmbeddedResources =
            Func.Memoize<Assembly, string[]>(a => a.GetManifestResourceNames());

        public static string FindTextResourceNamed(this Assembly assembly, params string[] names)
        {
            return FindTextResourceNamed(assembly, names.AsEnumerable());
        }

        public static string FindTextResourceNamed(this Assembly assembly, IEnumerable<string> names)
        {
            var textResources = names
                .SelectMany(x => new[] { "", ".txt", ".html", ".md" }.Select(y => x + y)).ToList();
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

        public static T Cast<T>(this object instance)
        {
            return (T) instance;
        }

        public static MethodInfo GetMethod<T>(this Type type, Expression<Action<T>> method)
        {
            return type.GetMethod(((MethodCallExpression) method.Body).Method.Name);
        }

        public static Type GetInterface(this Type type, Type interfaceType)
        {
            return type.GetInterfaces()
                .FirstOrDefault(x => x == interfaceType || (x.IsGenericType && x.GetGenericTypeDefinition() == interfaceType));
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
            if (type == typeof(TimeSpan) || type == typeof(TimeSpan?)) return "duration";
            if (type == typeof(Guid) || type == typeof(Guid?)) return "guid";
            if (type == typeof(Char) || type == typeof(Char?)) return "char";
            if (type == typeof(byte[])) return "base64Binary";
            if (type.IsArray || type.IsList()) return "ArrayOf" + type.GetListElementType().GetXmlName().InitialCap();
            return type.IsNullable() ? Nullable.GetUnderlyingType(type).Name : type.Name;
        }

        public static bool IsString(this Type type) { return type == typeof(String); }
        public static bool IsBoolean(this Type type) { return type == typeof(Boolean) || type == typeof(Boolean?); }
        public static bool IsDecimal(this Type type) { return type == typeof(Decimal) || type == typeof(Decimal?); }
        public static bool IsDouble(this Type type) { return type == typeof(Double) || type == typeof(Double?); }
        public static bool IsSingle(this Type type) { return type == typeof(Single) || type == typeof(Single?); }
        public static bool IsByteArray(this Type type) { return type == typeof(byte[]); }
        public static bool IsByte(this Type type) { return type == typeof(Byte) || type == typeof(Byte?); }
        public static bool IsSByte(this Type type) { return type == typeof(SByte) || type == typeof(SByte?); }
        public static bool IsInt16(this Type type) { return type == typeof(Int16) || type == typeof(Int16?); }
        public static bool IsUInt16(this Type type) { return type == typeof(UInt16) || type == typeof(UInt16?); }
        public static bool IsInt32(this Type type) { return type == typeof(Int32) || type == typeof(Int32?); }
        public static bool IsUInt32(this Type type) { return type == typeof(UInt32) || type == typeof(UInt32?); }
        public static bool IsInt64(this Type type) { return type == typeof(Int64) || type == typeof(Int64?); }
        public static bool IsUInt64(this Type type) { return type == typeof(UInt64) || type == typeof(UInt64?); }
        public static bool IsDateTime(this Type type) { return type == typeof(DateTime) || type == typeof(DateTime?); }
        public static bool IsTimeSpan(this Type type) { return type == typeof(TimeSpan) || type == typeof(TimeSpan?); }
        public static bool IsGuid(this Type type) { return type == typeof(Guid) || type == typeof(Guid?); }
        public static bool IsChar(this Type type) { return type == typeof(Char) || type == typeof(Char?); }

        public static bool IsString(this PropertyInfo property) { return property.PropertyType.IsString(); }
        public static bool IsBoolean(this PropertyInfo property) { return property.PropertyType.IsBoolean(); }
        public static bool IsDecimal(this PropertyInfo property) { return property.PropertyType.IsDecimal(); }
        public static bool IsDouble(this PropertyInfo property) { return property.PropertyType.IsDouble(); }
        public static bool IsSingle(this PropertyInfo property) { return property.PropertyType.IsSingle(); }
        public static bool IsByteArray(this PropertyInfo property) { return property.PropertyType.IsByteArray(); }
        public static bool IsByte(this PropertyInfo property) { return property.PropertyType.IsByte(); }
        public static bool IsSByte(this PropertyInfo property) { return property.PropertyType.IsSByte(); }
        public static bool IsInt16(this PropertyInfo property) { return property.PropertyType.IsInt16(); }
        public static bool IsUInt16(this PropertyInfo property) { return property.PropertyType.IsUInt16(); }
        public static bool IsInt32(this PropertyInfo property) { return property.PropertyType.IsInt32(); }
        public static bool IsUInt32(this PropertyInfo property) { return property.PropertyType.IsUInt32(); }
        public static bool IsInt64(this PropertyInfo property) { return property.PropertyType.IsInt64(); }
        public static bool IsUInt64(this PropertyInfo property) { return property.PropertyType.IsUInt64(); }
        public static bool IsDateTime(this PropertyInfo property) { return property.PropertyType.IsDateTime(); }
        public static bool IsTimeSpan(this PropertyInfo property) { return property.PropertyType.IsTimeSpan(); }
        public static bool IsGuid(this PropertyInfo property) { return property.PropertyType.IsGuid(); }
        public static bool IsChar(this PropertyInfo property) { return property.PropertyType.IsChar(); }
    }
}