using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace Tests
{
    public enum HttpVerbs
    {
        Head,
        Get,
        Post,
        Put,
        Delete,
        Options,
        Trace,
        Connect,
        Patch
    }

    public static class Behaviors
    {
        public static BehaviorGraph BuildGraph()
        {
            return new BehaviorGraph();
        }

        public static BehaviorGraph AddAction<T>(this BehaviorGraph graph, string route, HttpVerbs verb)
        {
            var chain = graph.AddActionFor(route, typeof (T));
            chain.Route.AddHttpMethodConstraint(verb.ToString().ToUpper());
            return graph;
        }

        public static ActionCall CreateAction<T>(string url, HttpVerbs verb)
        {
            return BuildGraph().AddAction<T>(url, verb).Actions().First();
        }
    }
}