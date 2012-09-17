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

        public static BehaviorGraph AddAction<T>(this BehaviorGraph graph, string verb = null)
        {
            graph.AddActionFor(typeof(T).GetHandlerUrl(typeof(T).Assembly.FullName), typeof(T))
                .Route.AddHttpMethodConstraint(verb ?? typeof(T).GetHandlerVerb());
            return graph;
        }

        public static BehaviorGraph AddActionsInThisNamespace(this BehaviorGraph graph)
        {
            var thisNamespace = new StackFrame(1).GetMethod().DeclaringType.Namespace;
            Assembly.GetCallingAssembly().GetTypes()
                .Where(x => x.Namespace.StartsWith(thisNamespace) && x.Name.EndsWith("Handler"))
                .ToList()
                .ForEach(x => {
                        var route = x.GetHandlerUrl(thisNamespace);
                        var method = x.GetHandlerVerb();
                        //Debug.WriteLine("{0} {1} ({2})", route, method, x.FullName);
                        graph.AddActionFor(route, x).Route.AddHttpMethodConstraint(method);
                    });
            return graph;
        }
    }
}