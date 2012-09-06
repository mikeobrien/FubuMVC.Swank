using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Resources.Conneg;
using Swank.Models;

namespace Swank
{
    public class OrphanedActionException : Exception {
        public OrphanedActionException(IEnumerable<string> actions) 
            : base(string.Format("The following actions are not associated with a module. Either assocate them with a module or turn off orphaned action exceptions. {0}", 
                    string.Join(", ", actions))) { } }

    public class SpecificationBuilder
    {
        private readonly Configuration _configuration;
        private readonly BehaviorGraph _behaviorGraph;

        public SpecificationBuilder(Configuration configuration, BehaviorGraph behaviorGraph)
        {
            _configuration = configuration;
            _behaviorGraph = behaviorGraph;
        }

        public Specification Build()
        {
            var actions = _behaviorGraph.Actions().Where(x =>
                !_configuration.AppliesToAssemblies.Any() || _configuration.AppliesToAssemblies.Any(y => y == x.HandlerType.Assembly))
                .Where(_configuration.Filter).ToList();
            return new Specification
                {
                    dataTypes = new List<DataType>(),
                    modules = GetModules(actions)
                };
        }

        private List<Module> GetModules(IList<ActionCall> actions)
        {
            if (_configuration.OrphanedActions == OrphanedActionsBehavior.ThrowException)
            {
                var orphanedActions = actions.Where(x => !_configuration.Modules.HasDescription(x)).ToList();
                if (orphanedActions.Any()) throw new OrphanedActionException(orphanedActions.Select(x => x.HandlerType.FullName + "." + x.Method.Name));
            }

            var modules = actions
                .Where(x => (_configuration.OrphanedActions == OrphanedActionsBehavior.Ignore && 
                             _configuration.Modules.HasDescription(x)) || 
                            _configuration.OrphanedActions == OrphanedActionsBehavior.AddToDefaultModule)
                .GroupBy(x => _configuration.Modules.GetDescription(x) ?? _configuration.DefaultModule)
                .Select(x => new Module {
                    name = x.Key.Name,
                    description = x.Key.Comments,
                    resources = GetResources(x)
                })
                .OrderBy(x => x.name);
            return modules.ToList();
        }

        private List<Resource> GetResources(IEnumerable<ActionCall> actions)
        {
            return null;
        }
    }
}
