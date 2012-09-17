using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Swank.Models;

namespace Tests
{
    public static class Extensions
    {
        public static MethodInfo GetExecuteMethod(this Type type)
        {
            return type.GetMethods().First(x => x.Name.StartsWith("Execute"));
        }

        public static string GetHandlerUrl(this Type handlerType, string rootNamespace)
        {
            var url = '/' + handlerType.Namespace.Replace(rootNamespace + ".", "").Replace('.', '/').ToLower();
            var handlerMethod = handlerType.GetExecuteMethod();
            var inputType = handlerMethod.GetParameters().Select(x => x.ParameterType).FirstOrDefault();
            return url + (inputType != null ? inputType
                .GetProperties()
                .Aggregate(handlerMethod.Name.Replace("Execute", "").Replace("_", "/").ToLower() + '/',
                (a, i) => Regex.Replace(a, "/" + i.Name + "/", "/{" + i.Name + "}/", RegexOptions.IgnoreCase)).TrimEnd('/') : "");
        }

        public static string GetHandlerVerb(this Type handlerType)
        {
            var typeName = handlerType.Name;
            if (typeName.EndsWith("GetHandler")) return "GET";
            if (typeName.EndsWith("PostHandler")) return "POST";
            if (typeName.EndsWith("PutHandler")) return "PUT";
            if (typeName.EndsWith("DeleteHandler")) return "DELETE";
            throw new Exception("Could not determine verb from handler type name.");
        }

        public static Endpoint GetEndpoint<T>(this Specification specification)
        {
            var url = typeof(T).GetHandlerUrl(new StackFrame(1).GetMethod().DeclaringType.Namespace);
            return specification.modules.SelectMany(x => x.resources).Concat(specification.resources)
                .SelectMany(x => x.endpoints).FirstOrDefault(x => x.url == url);
        }
    }
}