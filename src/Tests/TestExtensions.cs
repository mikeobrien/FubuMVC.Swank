using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank.Extensions;
using FubuMVC.Swank.Specification;
using Should;

namespace Tests
{
    public static class Behavior
    {
        public static BehaviorGraph BuildGraph()
        {
            return new BehaviorGraph();
        }
    }

    public static class Paths
    {
        public static string TestHarness => Path.GetFullPath(Environment
            .CurrentDirectory + @"\..\..\..\TestHarness").NormalizeToUnc();
    }

    public static class TestExtensions
    {
        [DllImport("mpr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int WNetGetConnection(
            [MarshalAs(UnmanagedType.LPTStr)] string localName,
            [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName, ref int length);

        public static string NormalizeToUnc(this string path)
        {
            if (path.StartsWith(@"\\")) return path;
            var share = new StringBuilder(512);
            var length = share.Capacity;
            var result = WNetGetConnection(path.Substring(0, 2), share, ref length);
            var normalizedPath = result == 0 ? share.ToString().TrimEnd() + path.Substring(2) : path;
            Console.WriteLine($"Normalizing '{path}' to '{normalizedPath}'");
            return normalizedPath;
        }

        public static IEnumerable<T> ShouldTotal<T>(this IEnumerable<T> source, int total)
        {
            source.Count().ShouldEqual(total);
            return source;
        }

        public static ActionCall GetAction<T>(this BehaviorGraph graph)
        {
            if (graph.Actions().All(x => x.HandlerType != typeof(T)))
                throw new Exception("Could not find handler of type {0} in behaviour graph with {1} actions."
                    .ToFormat(typeof(T).Name, graph.Actions().Count()));
            return graph.Actions().First(x => x.HandlerType == typeof(T));
        }

        public static IList<ActionCall> GetActions<T>(this BehaviorGraph graph)
        {
            return graph.Actions().Where(x => x.HandlerType == typeof(T)).ToList();
        }

        public static ActionCall AddAndGetAction<T>(this BehaviorGraph graph, string verb = null)
        {
            return AddAction(graph, typeof(T), verb).ActionsForHandler<T>().First();
        }

        public static BehaviorGraph AddAction<T>(this BehaviorGraph graph, string verb = null)
        {
            return AddAction(graph, typeof(T), verb);
        }

        private static BehaviorGraph AddAction(BehaviorGraph graph, Type type, string verb = null, string thisNamespace = null)
        {
            var route = type.GetHandlerUrl(thisNamespace ?? type.Assembly.FullName);
            var chain = graph.AddActionFor(route, type);
            var method = verb ?? (type.Name.EndsWith("Handler") ? type.GetHandlerVerb() : null);
            if (method != null) chain.Route.AddHttpMethodConstraint(method);
            return graph;
        }

        public static BehaviorGraph AddActionsInThisNamespace(this BehaviorGraph graph)
        {
            return graph.AddActionsInNamespace(MethodBase.GetCurrentMethod().GetCallingType());
        }

        public static Type GetCallingType(this MethodBase method)
        {
            return new StackTrace().GetFrames().Select(x => x.GetMethod().DeclaringType)
                .First(x => x != method.DeclaringType);
        }

        public static BehaviorGraph AddActionsInNamespace<T>(this BehaviorGraph graph)
        {
            return graph.AddActionsInNamespace(typeof (T));
        }

        public static BehaviorGraph AddActionsInNamespace(this BehaviorGraph graph, Type type)
        {
            var rootNamespace = type.Namespace;
            Assembly.GetCallingAssembly().GetTypes()
                .Where(x => !x.Namespace.IsEmpty() && x.Namespace.StartsWith(rootNamespace) && (x.Name.EndsWith("Handler") || x.Name.EndsWith("Controller")))
                .ToList().ForEach(x => AddAction(graph, x, thisNamespace: rootNamespace));
            return graph;
        }

        public static MethodInfo GetExecuteMethod(this Type type)
        {
            return type.GetMethods().First(x => x.Name.StartsWith("Execute"));
        }

        public static string GetHandlerUrl(this Type handlerType, string rootNamespace)
        {
            var url = handlerType.Namespace == rootNamespace ? "/" : 
                '/' + handlerType.Namespace.Replace(rootNamespace + ".", "").Replace('.', '/').ToLower();
            var handlerMethod = handlerType.GetExecuteMethod();
            var inputType = handlerMethod.GetParameters().Select(x => x.ParameterType).FirstOrDefault();
            url = url + (inputType != null ? inputType
                .GetProperties()
                .Aggregate(handlerMethod.Name.Replace("Execute", "").Replace("_", "/").ToLower() + '/',
                (a, i) => Regex.Replace(a, "/" + i.Name + "/", "/{" + i.Name + "}/", RegexOptions.IgnoreCase)).TrimEnd('/') : "");
            return "/" + url.Split(new [] {"/"}, StringSplitOptions.RemoveEmptyEntries).Join("/");
        }

        public static string GetHandlerVerb(this Type handlerType)
        {
            var typeName = handlerType.Name;
            if (typeName.EndsWith("GetHandler")) return "GET";
            if (typeName.EndsWith("PostHandler")) return "POST";
            if (typeName.EndsWith("PutHandler")) return "PUT";
            if (typeName.EndsWith("UpdateHandler")) return "UPDATE";
            if (typeName.EndsWith("DeleteHandler")) return "DELETE";
            throw new Exception("Could not determine verb from handler type name.");
        }

        public static Endpoint GetEndpoint<T>(this FubuMVC.Swank.Specification.Specification specification)
        {
            var url = typeof(T).GetHandlerUrl(new StackFrame(1).GetMethod().DeclaringType.Namespace);
            return specification.Modules.SelectMany(x => x.Resources)
                .SelectMany(x => x.Endpoints).FirstOrDefault(x => x.Url.Split('?')[0] == url);
        }

        public static Resource GetResource<T>(this FubuMVC.Swank.Specification.Specification specification)
        {
            var url = typeof(T).GetHandlerUrl(new StackFrame(1).GetMethod().DeclaringType.Namespace);
            return specification.Modules.SelectMany(x => x.Resources)
                .FirstOrDefault(x => x.Endpoints.Any(y => y.Url == url));
        }

        public static bool HasDefaultModule(this IEnumerable<FubuMVC.Swank.Specification.Module> modules)
        {
            return modules.Any(x => x.Name == FubuMVC.Swank.Specification.Module.DefaultName);
        }

        public static FubuMVC.Swank.Specification.Module GetDefaultModule(this IEnumerable<FubuMVC.Swank.Specification.Module> modules)
        {
            return modules.SingleOrDefault(x => x.Name == FubuMVC.Swank.Specification.Module.DefaultName);
        }

        public static FubuMVC.Swank.Specification.Module GetFirstNonDefaultModule(this IEnumerable<FubuMVC.Swank.Specification.Module> modules)
        {
            return modules.FirstOrDefault(x => x.Name != FubuMVC.Swank.Specification.Module.DefaultName);
        }

        public static bool InNamespace<T>(this Type type)
        {
            return type.Namespace == typeof (T).Namespace || 
                type.Namespace.StartsWith(typeof (T).Namespace + ".");
        }

        public static string FindTextResourceNamed<T>(this Assembly assembly)
        {
            return assembly.FindTextResourceNamed(typeof(T).FullName);
        }

        public static void ShouldContainMember<TType>(this DataType type, Expression<Func<TType, object>> member)
        {
            type.ShouldContainMember(member.GetMemberName());
        }

        public static void ShouldContainMember(this DataType type, string name)
        {
            type.Members.Count(x => x.Name == name)
                .ShouldEqual(1, "Should contain member {0}.".ToFormat(name));
        }

        public static void ShouldNotContainMember<TType>(this DataType type, Expression<Func<TType, object>> member)
        {
            type.Members.Any(x => x.Name == member.GetMemberName())
                .ShouldBeFalse("Should not contain member {0}.".ToFormat(member.GetMemberName()));
        }

        public static bool HasUrlParameter<TType>(this Endpoint endpoint, Expression<Func<TType, object>> member)
        {
            return endpoint.UrlParameters.Count(x => x.Name == member.GetMemberName()) == 1;
        }

        public static UrlParameter GetUrlParameter<TType>(this Endpoint endpoint, Expression<Func<TType, object>> member)
        {
            return endpoint.UrlParameters.Single(x => x.Name == member.GetMemberName());
        }

        public static bool HasQuerystring<TType>(this Endpoint endpoint, Expression<Func<TType, object>> member)
        {
            return endpoint.QuerystringParameters.Count(x => x.Name == member.GetMemberName()) == 1;
        }

        public static QuerystringParameter GetQuerystring<TType>(this Endpoint endpoint, Expression<Func<TType, object>> member)
        {
            return endpoint.QuerystringParameters.Single(x => x.Name == member.GetMemberName());
        }

        public static Member GetMember<TType>(this DataType type, Expression<Func<TType, object>> member)
        {
            return type.Members.Single(x => x.Name == member.GetMemberName());
        }

        private static string GetMemberName<TType>(this Expression<Func<TType, object>> member)
        {
            return ((MemberExpression)(member.Body is UnaryExpression ? ((UnaryExpression)member.Body).Operand : member.Body)).Member.Name;
        }
    }
}