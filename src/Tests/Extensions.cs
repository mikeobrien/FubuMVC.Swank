using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Bottles.Commands;
using Bottles.Creation;
using FubuCore;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank;
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

    public static class Bottles
    {
        public static bool Create(string source, string output, bool deleteExisting, bool includePdbs)
        {
            return new CreateBottleCommand().Execute(new CreateBottleInput
            {
                PackageFolder = source,
                ZipFileFlag = output,
                ForceFlag = deleteExisting,
                PdbFlag = includePdbs
            });
        }
    }

    public static class Paths
    {
        public static string Swank { get { return Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\Swank")); } }
        public static string TestHarness { get { return Path.GetFullPath(Environment.CurrentDirectory + @"\..\..\..\TestHarness");}}
    }

    public static class Extensions
    {
        public static ActionCall GetAction<T>(this BehaviorGraph graph)
        {
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
            var thisNamespace = new StackFrame(1).GetMethod().DeclaringType.Namespace;
            Assembly.GetCallingAssembly().GetTypes()
                .Where(x => x.Namespace.StartsWith(thisNamespace) && (x.Name.EndsWith("Handler") || x.Name.EndsWith("Controller")))
                .ToList()
                .ForEach(x => AddAction(graph, x, thisNamespace: thisNamespace));
            return graph;
        }

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
            if (typeName.EndsWith("UpdateHandler")) return "UPDATE";
            if (typeName.EndsWith("DeleteHandler")) return "DELETE";
            throw new Exception("Could not determine verb from handler type name.");
        }

        public static Endpoint GetEndpoint<T>(this Specification specification)
        {
            var url = typeof(T).GetHandlerUrl(new StackFrame(1).GetMethod().DeclaringType.Namespace);
            return specification.modules.SelectMany(x => x.resources).Concat(specification.resources)
                .SelectMany(x => x.endpoints).FirstOrDefault(x => x.url == url);
        }

        public static Resource GetResource<T>(this Specification specification)
        {
            var url = typeof(T).GetHandlerUrl(new StackFrame(1).GetMethod().DeclaringType.Namespace);
            return specification.modules.SelectMany(x => x.resources).Concat(specification.resources)
                .FirstOrDefault(x => x.endpoints.Any(y => y.url == url));
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

        public static FubuMVC.Swank.Type GetType<TType>(this List<FubuMVC.Swank.Type> types)
        {
            return types.Single(x => x.id == typeof(TType).GetHash());
        }

        public static FubuMVC.Swank.Type GetType<TType, THandler>(this List<FubuMVC.Swank.Type> types)
        {
            return types.Single(x => x.id == typeof(TType).GetHash(typeof(THandler).GetExecuteMethod()));
        }

        public static void ShouldContainOneOutputType<TType>(this List<FubuMVC.Swank.Type> types)
        {
            types.ShouldContainOneType<TType>();
        }

        public static void ShouldContainOneType<TType>(this List<FubuMVC.Swank.Type> types)
        {
            types.ShouldContainOneType<TType>(typeof(TType).GetHash());
        }

        public static void ShouldNotContainAnyOutputTypes<TType>(this List<FubuMVC.Swank.Type> types)
        {
            types.ShouldNotContainAnyType<TType>();
        }

        public static void ShouldNotContainAnyType<TType>(this List<FubuMVC.Swank.Type> types)
        {
            types.ShouldNotContainAnyType<TType>(typeof(TType).GetHash());
        }

        public static void ShouldContainOneInputType<TType, THandler>(this List<FubuMVC.Swank.Type> types)
        {
            types.ShouldContainOneType<TType>(typeof(TType).GetHash(typeof(THandler).GetExecuteMethod()));
        }

        public static void ShouldNotContainAnyInputType<TType, THandler>(this List<FubuMVC.Swank.Type> types)
        {
            types.ShouldNotContainAnyType<TType>(typeof(TType).GetHash(typeof(THandler).GetExecuteMethod()));
        }

        private static void ShouldNotContainAnyType<TType>(this List<FubuMVC.Swank.Type> types, string id)
        {
            types.Any(x => x.id == id).ShouldBeFalse("Specification should not contain type {0}.".ToFormat(typeof(TType).Name));
        }

        private static void ShouldContainOneType<TType>(this List<FubuMVC.Swank.Type> types, string id)
        {
            var count = types.Count(x => x.id == id);
            (count > 0).ShouldBeTrue("Specification does not contain any types {0}.".ToFormat(typeof(TType).Name));
            (count < 2).ShouldBeTrue("Specification contains more than one types {0}.".ToFormat(typeof(TType).Name));
        }

        public static void ShouldContainMember<TType>(this FubuMVC.Swank.Type type, Expression<Func<TType, object>> member)
        {
            type.ShouldContainMember(member.GetMemberName());
        }

        public static void ShouldContainMember(this FubuMVC.Swank.Type type, string name)
        {
            type.members.Count(x => x.name == name)
                .ShouldEqual(1, "Should contain member {0}.".ToFormat(name));
        }

        public static void ShouldNotContainMember<TType>(this FubuMVC.Swank.Type type, Expression<Func<TType, object>> member)
        {
            type.members.Any(x => x.name == member.GetMemberName())
                .ShouldBeFalse("Should not contain member {0}.".ToFormat(member.GetMemberName()));
        }

        public static bool HasUrlParameter<TType>(this Endpoint endpoint, Expression<Func<TType, object>> member)
        {
            return endpoint.urlParameters.Count(x => x.name == member.GetMemberName()) == 1;
        }

        public static UrlParameter GetUrlParameter<TType>(this Endpoint endpoint, Expression<Func<TType, object>> member)
        {
            return endpoint.urlParameters.Single(x => x.name == member.GetMemberName());
        }

        public static bool HasQuerystring<TType>(this Endpoint endpoint, Expression<Func<TType, object>> member)
        {
            return endpoint.querystringParameters.Count(x => x.name == member.GetMemberName()) == 1;
        }

        public static QuerystringParameter GetQuerystring<TType>(this Endpoint endpoint, Expression<Func<TType, object>> member)
        {
            return endpoint.querystringParameters.Single(x => x.name == member.GetMemberName());
        }

        public static Member GetMember<TType>(this FubuMVC.Swank.Type type, Expression<Func<TType, object>> member)
        {
            return type.members.Single(x => x.name == member.GetMemberName());
        }

        private static string GetMemberName<TType>(this Expression<Func<TType, object>> member)
        {
            return ((MemberExpression)(member.Body is UnaryExpression ? ((UnaryExpression)member.Body).Operand : member.Body)).Member.Name;
        }
    }
}