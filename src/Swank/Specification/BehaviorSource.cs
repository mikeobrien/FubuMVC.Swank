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

        public IList<ActionCall> GetChains()
        {
            return _behaviorGraph.Behaviors
                .Where(isNotContentChain)
                .Where(isNotDiagnosticChain)
                .Where(isNotSwankChain)
                .Where(isNotFubuChain)
                .Where(isNotConfigThing)
                .Where(_configuration.Filter)
                .SelectMany(b => b.Calls)
                .ToList();
        }

        bool isNotConfigThing(BehaviorChain chain)
        {
            return (!_configuration.AppliesToAssemblies.Any() ||
                    _configuration.AppliesToAssemblies.Any(y => y == chain.FirstCall().HandlerType.Assembly));
        }

        bool isNotSwankChain(BehaviorChain chain)
        {
            return chain.FirstCall().HandlerType.Assembly != Assembly.GetExecutingAssembly();
        }

        bool isNotFubuChain(BehaviorChain chain)
        {
            return chain.FirstCall().HandlerType.Assembly != typeof (FubuRegistry).Assembly;
        }

        bool isNotContentChain(BehaviorChain chain)
        {
            return !chain.GetRoutePattern().StartsWith("/_content");
        }

        bool isNotDiagnosticChain(BehaviorChain chain)
        {
            return !chain.GetRoutePattern().StartsWith("/_fubu");
        }
    }
}