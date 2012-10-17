using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Swank.Specification
{
    public class ActionSource
    {
        private readonly BehaviorGraph _behaviorGraph;
        private readonly Configuration _configuration;

        public ActionSource(BehaviorGraph behaviorGraph, Configuration configuration)
        {
            _behaviorGraph = behaviorGraph;
            _configuration = configuration;
        }

        public IList<ActionCall> GetActions()
        {
            return _behaviorGraph.Actions()
                .Where(x => x.HandlerType.Assembly != Assembly.GetExecutingAssembly() &&
                            x.HandlerType.Assembly != typeof(FubuRegistry).Assembly &&
                            x.ToRouteDefinition().Pattern.StartsWith("/_fubu") &&
                            x.ToRouteDefinition().Pattern.StartsWith("/_content") &&
                            (!_configuration.AppliesToAssemblies.Any() || 
                             _configuration.AppliesToAssemblies.Any(y => y == x.HandlerType.Assembly)))
                .Where(_configuration.Filter).ToList();
        }
    }
}