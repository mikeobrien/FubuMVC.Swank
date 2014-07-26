using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Swank.Specification
{
    public class BehaviorSource
    {
        private readonly BehaviorGraph _behaviorGraph;
        private readonly Configuration _configuration;

        public BehaviorSource(BehaviorGraph behaviorGraph, Configuration configuration)
        {
            _behaviorGraph = behaviorGraph;
            _configuration = configuration;
        }

        public IList<BehaviorChain> GetChains()
        {
            return _behaviorGraph.Behaviors
                .Where(IsNotContentChain)
                .Where(IsNotDiagnosticChain)
                .Where(IsNotSwankChain)
                .Where(IsNotFubuChain)
                .Where(IsNotConfigThing)
                .Where(_configuration.Filter)
                .ToList();
        }

        bool IsNotConfigThing(BehaviorChain chain)
        {
            return (!_configuration.AppliesToAssemblies.Any() ||
                    _configuration.AppliesToAssemblies.Any(y => y == chain.FirstCall().HandlerType.Assembly));
        }

        static bool IsNotSwankChain(BehaviorChain chain)
        {
            var call = chain.FirstCall();
            if (call == null) return false;
            return call.HandlerType.Assembly != typeof(BehaviorSource).Assembly;
        }

        static bool IsNotFubuChain(BehaviorChain chain)
        {
            var call = chain.FirstCall();
            if (call == null) return false;
            return call.HandlerType.Assembly != typeof (FubuRegistry).Assembly;
        }

        static bool IsNotContentChain(BehaviorChain chain)
        {
            return !RoutePatternStartsWith(chain, "/_content");
        }

        static bool IsNotDiagnosticChain(BehaviorChain chain)
        {
            return !RoutePatternStartsWith(chain, "/_fubu");
        }

        static bool RoutePatternStartsWith(BehaviorChain chain, string value)
        {
            var routePattern = chain.GetRoutePattern();
            return routePattern != null && routePattern.StartsWith(value);
        }
    }
}