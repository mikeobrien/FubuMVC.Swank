using FubuMVC.Core.Registration;

namespace Tests
{
    public enum HttpVerbs
    {
        Head, Get, Post, Put, Delete, Options, Trace, Connect, Patch
    }

    public static class Behaviors
    {

        public static BehaviorGraph BuildGraph()
        {
            return new BehaviorGraph();
        }

        public static BehaviorGraph AddAction<T>(this BehaviorGraph graph, string route, HttpVerbs verb)
        {
            var chain = graph.AddActionFor(route, typeof(T));
            chain.Route.AddHttpMethodConstraint("POST");
            return graph;
        }
    }
}