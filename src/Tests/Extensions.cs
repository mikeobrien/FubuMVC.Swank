using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank.Extensions;
using FubuMVC.Swank.Specification;
using Should;
using Type = System.Type;

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
        public static string TestHarness { get { return Path.GetFullPath(Environment.CurrentDirectory + @"\..\..\..\TestHarness");}}
    }

    public static class Extensions
    {
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
                .Where(x => x.Namespace.StartsWith(rootNamespace) && (x.Name.EndsWith("Handler") || x.Name.EndsWith("Controller")))
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
            return specification.Modules.SelectMany(x => x.Resources).Concat(specification.Resources)
                .SelectMany(x => x.Endpoints).FirstOrDefault(x => x.Url == url);
        }

        public static Resource GetResource<T>(this FubuMVC.Swank.Specification.Specification specification)
        {
            var url = typeof(T).GetHandlerUrl(new StackFrame(1).GetMethod().DeclaringType.Namespace);
            return specification.Modules.SelectMany(x => x.Resources).Concat(specification.Resources)
                .FirstOrDefault(x => x.Endpoints.Any(y => y.Url == url));
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

        public static FubuMVC.Swank.Specification.Type GetType<TType>(this List<FubuMVC.Swank.Specification.Type> types)
        {
            return types.Single(x => x.Id == typeof(TType).GetHash());
        }

        public static FubuMVC.Swank.Specification.Type GetType<TType, THandler>(this List<FubuMVC.Swank.Specification.Type> types)
        {
            return types.Single(x => x.Id == typeof(TType).GetHash(typeof(THandler).GetExecuteMethod()));
        }

        public static void ShouldContainOneOutputType<TType>(this List<FubuMVC.Swank.Specification.Type> types)
        {
            types.ShouldContainOneType<TType>();
        }

        public static void ShouldContainOneType<TType>(this List<FubuMVC.Swank.Specification.Type> types)
        {
            types.ShouldContainOneType<TType>(typeof(TType).GetHash());
        }

        public static void ShouldNotContainAnyOutputTypes<TType>(this List<FubuMVC.Swank.Specification.Type> types)
        {
            types.ShouldNotContainAnyType<TType>();
        }

        public static void ShouldNotContainAnyType<TType>(this List<FubuMVC.Swank.Specification.Type> types)
        {
            types.ShouldNotContainAnyType<TType>(typeof(TType).GetHash());
        }

        public static void ShouldContainOneInputType<TType, THandler>(this List<FubuMVC.Swank.Specification.Type> types)
        {
            types.ShouldContainOneType<TType>(typeof(TType).GetHash(typeof(THandler).GetExecuteMethod()));
        }

        public static void ShouldNotContainAnyInputType<TType, THandler>(this List<FubuMVC.Swank.Specification.Type> types)
        {
            types.ShouldNotContainAnyType<TType>(typeof(TType).GetHash(typeof(THandler).GetExecuteMethod()));
        }

        private static void ShouldNotContainAnyType<TType>(this List<FubuMVC.Swank.Specification.Type> types, string id)
        {
            types.Any(x => x.Id == id).ShouldBeFalse("Specification should not contain type {0}.".ToFormat(typeof(TType).Name));
        }

        private static void ShouldContainOneType<TType>(this List<FubuMVC.Swank.Specification.Type> types, string id)
        {
            var count = types.Count(x => x.Id == id);
            (count > 0).ShouldBeTrue("Specification does not contain any types {0}.".ToFormat(typeof(TType).Name));
            (count < 2).ShouldBeTrue("Specification contains more than one types {0}.".ToFormat(typeof(TType).Name));
        }

        public static void ShouldContainMember<TType>(this FubuMVC.Swank.Specification.Type type, Expression<Func<TType, object>> member)
        {
            type.ShouldContainMember(member.GetMemberName());
        }

        public static void ShouldContainMember(this FubuMVC.Swank.Specification.Type type, string name)
        {
            type.Members.Count(x => x.Name == name)
                .ShouldEqual(1, "Should contain member {0}.".ToFormat(name));
        }

        public static void ShouldNotContainMember<TType>(this FubuMVC.Swank.Specification.Type type, Expression<Func<TType, object>> member)
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

        public static Member GetMember<TType>(this FubuMVC.Swank.Specification.Type type, Expression<Func<TType, object>> member)
        {
            return type.Members.Single(x => x.Name == member.GetMemberName());
        }

        private static string GetMemberName<TType>(this Expression<Func<TType, object>> member)
        {
            return ((MemberExpression)(member.Body is UnaryExpression ? ((UnaryExpression)member.Body).Operand : member.Body)).Member.Name;
        }
    }
}