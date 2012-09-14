using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace Tests
{
    public static class Behaviors
    {
        public static BehaviorGraph BuildGraph()
        {
            return new BehaviorGraph();
        }

        public static ActionCall GetAction<T>(this BehaviorGraph graph)
        {
            return graph.Actions().First(x => x.HandlerType == typeof(T));
        }

        public static BehaviorGraph AddAction<T>(this BehaviorGraph graph, string verb = null)
        {
            graph.AddActionFor(GetRoute(typeof(T), typeof(T).Assembly.FullName), typeof(T))
                .Route.AddHttpMethodConstraint(verb ?? GetVerb(typeof(T)));
            return graph;
        }

        public static BehaviorGraph AddActionsInThisNamespace(this BehaviorGraph graph)
        {
            var thisNamespace = new StackFrame(1).GetMethod().DeclaringType.Namespace;
            Assembly.GetCallingAssembly().GetTypes()
                .Where(x => x.Namespace.StartsWith(thisNamespace) && x.Name.EndsWith("Handler"))
                .ToList()
                .ForEach(x => {
                        var route = GetRoute(x, thisNamespace);
                        var method = GetVerb(x);
                        //Debug.WriteLine("{0} {1} ({2})", route, method, x.FullName);
                        graph.AddActionFor(route, x).Route.AddHttpMethodConstraint(method);
                    });
            return graph;
        }

        private static string GetRoute(Type handlerType, string thisNamespace)
        {
            var url = '/' + handlerType.Namespace.Replace(thisNamespace + ".", "").Replace('.', '/').ToLower();
            var handlerMethod = handlerType.GetMethods().First(x => x.Name.StartsWith("Execute"));
            var inputType = handlerMethod.GetParameters().Select(x => x.ParameterType).FirstOrDefault();
            return url + (inputType != null ? inputType
                .GetProperties()
                .Aggregate(handlerMethod.Name.Replace("Execute", "").Replace("_", "/").ToLower() + '/', 
                (a, i) => Regex.Replace(a, "/" + i.Name + "/", "/{" + i.Name + "}/", RegexOptions.IgnoreCase)).TrimEnd('/') : "");
        }

        private static string GetVerb(Type type)
        { 
            var typeName = type.Name;
            if (typeName.EndsWith("GetHandler")) return "GET";
            if (typeName.EndsWith("PostHandler")) return "POST";
            if (typeName.EndsWith("PutHandler")) return "PUT";
            if (typeName.EndsWith("DeleteHandler")) return "DELETE";
            throw new Exception("Could not determine verb from handler type name.");
        }
    }
}