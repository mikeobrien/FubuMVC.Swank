using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

        public static IList<ActionCall> GetActions<T>(this BehaviorGraph graph)
        {
            return graph.Actions().Where(x => x.HandlerType == typeof(T)).ToList();
        }

        public static BehaviorGraph AddAction<T>(this BehaviorGraph graph, string verb = null)
        {
            return AddAction(graph, typeof (T), verb);
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
    }
}