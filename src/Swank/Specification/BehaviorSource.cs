using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                .Where(isNotContentChain)
                .Where(isNotDiagnosticChain)
                .Where(isNotSwankChain)
                .Where(isNotFubuChain)
                .Where(isNotConfigThing)
                .Where(_configuration.Filter)
                .ToList();
        }

        bool isNotConfigThing(BehaviorChain chain)
        {
            return (!_configuration.AppliesToAssemblies.Any() ||
                    _configuration.AppliesToAssemblies.Any(y => y == chain.FirstCall().HandlerType.Assembly));
        }

        static bool isNotSwankChain(BehaviorChain chain)
        {
            var call = chain.FirstCall();

            if (call == null)
            {
                return false;
            }

            return call.HandlerType.Assembly != Assembly.GetExecutingAssembly();
        }

        static bool isNotFubuChain(BehaviorChain chain)
        {
            var call = chain.FirstCall();

            if (call == null)
            {
                return false;
            }

            return call.HandlerType.Assembly != typeof (FubuRegistry).Assembly;
        }

        static bool isNotContentChain(BehaviorChain chain)
        {
            return !routePatternStartsWith(chain, "/_content");
        }

        static bool isNotDiagnosticChain(BehaviorChain chain)
        {
            return !routePatternStartsWith(chain, "/_fubu");
        }

        static bool routePatternStartsWith(BehaviorChain chain, string value)
        {
            var routePattern = chain.GetRoutePattern();
            return routePattern != null && routePattern.StartsWith(value);
        }
    }
}