using System.Linq;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.Registration;

namespace Swank
{
    public class ResourcesHandler
    {
        private readonly BehaviorGraph _graph;
        private readonly Configuration _configuration;

        public ResourcesHandler(BehaviorGraph graph, Configuration configuration)
        {
            _graph = graph;
            _configuration = configuration;
        }

        public Resources Execute()
        {
            return new Resources {
                basePath = _configuration.Url,
                apiVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
                swaggerVersion = "1.0",
                apis = _graph.Actions()
                    .Where(x => x.OutputType() != typeof(Resources) && 
                                _configuration.AppliesToAssemblies.Any(y => y == x.HandlerType.Assembly))
                    .GroupBy(x => x.ParentChain().Route.GetRouteResourceId())
                    .Select(x => new Resource {
                            path = "/{0}.{{format}}".ToFormat(x.Key),
                            description = "This is the {0} resource yo!".ToFormat(
                                    x.First().ParentChain().Route.GetRouteResourceDescription()) })
                    .ToList()
                };
        }
    }
}