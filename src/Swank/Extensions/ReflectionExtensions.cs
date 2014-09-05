using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FubuCore;
using MarkdownSharp;

namespace FubuMVC.Swank.Extensions
{
    public static class ReflectionExtensions
    {
        public static bool IsSystemType(this Type type)
        {
            return type.FullName.StartsWith("System.");
        }

        public static bool IsSimpleType(this Type type)
        {
            Func<Type, bool> isSimpleType = x => x.IsPrimitive || x.IsEnum || x == typeof(string) || x == typeof(decimal) ||
                 x == typeof(DateTime) || x == typeof(TimeSpan) || x == typeof(Guid) || x == typeof(Uri);
            return isSimpleType(type) || (type.IsNullable() && isSimpleType(Nullable.GetUnderlyingType(type)));
        }

        public static bool Implements<T>(this Type type)
        {
            return type.Implements(typeof(T));
        }

        public static bool Implements(this Type type, Type @interface)
        {
            return type.GetInterfaces().Any(x => @interface ==
                (x.IsGenericType && @interface.IsGenericType &&
                 @interface.IsGenericTypeDefinition ? x.GetGenericTypeDefinition() : x));
        }

        public static Type UnwrapType(this Type type)
        {
            if (type.IsDictionary()) return type.GetGenericDictionaryTypes().Value.UnwrapType();
            if (type.IsArray || type.IsList()) return type.GetListElementType().UnwrapType();
            if (type.IsNullable()) return type.GetInnerTypeFromNullable().UnwrapType();
            return type;
        }

        public static Type GetNullableUnderlyingType(this Type type)
        {
            return type.IsNullable() ? type.GetInnerTypeFromNullable() : type;
        }

        // Lists

        private static readonly Type[] ListTypes = { typeof(IList<>), typeof(List<>) };

        public static bool IsListType(this Type type)
        {
            var genericTypeDef = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
            return ListTypes.Any(x => genericTypeDef == x);
        }

        public static bool ImplementsListType(this Type type)
        {
            return type.Implements(typeof(IList<>));
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

        // Dictionaries

        public static bool IsDictionary(this Type type)
        {
            return type.IsNonGenericDictionary() || type.IsGenericDictionary();
        }

        public static bool IsNonGenericDictionary(this Type type)
        {
            return type == typeof(IDictionary) || type.Implements<IDictionary>();
        }

        public static bool IsGenericDictionary(this Type type)
        {
            return type.GetInterfaces().Any(x => x.IsGenericDictionaryInterface()) || 
                type.IsGenericDictionaryInterface();
        }

        public static bool IsGenericDictionaryInterface(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>);
        }

        public static KeyValuePair<Type, Type> GetGenericDictionaryTypes(this Type type)
        {
            return (type.IsGenericDictionaryInterface() ? type
                : type.GetInterfaces().First(x => x.IsGenericDictionaryInterface()))
                .GetGenericArguments()
                .Map(x => new KeyValuePair<Type, Type>(x[0], x[1]));
        }

        public static Type MakeConcreteGenericDictionaryType(this Type type)
        {
            return type.GetGenericDictionaryTypes()
                .Map(x => typeof(Dictionary<,>).MakeGenericType(x.Key, x.Value));
        }

        // Enums

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

        public static string FindTextResourceNamed(this IEnumerable<Assembly> assemblies, params string[] names)
        {
            return assemblies
                .Select(x => x.FindTextResourceNamed(names))
                .FirstOrDefault(x => x != null);
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