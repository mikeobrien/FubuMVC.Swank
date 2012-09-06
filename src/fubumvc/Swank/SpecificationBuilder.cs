using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
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
        private readonly ActionSource _actions;
        private readonly IModuleSource _modules;
        private readonly IResourceSource _resources;

        public SpecificationBuilder(
            Configuration configuration, 
            ActionSource actions, 
            IModuleSource modules, 
            IResourceSource resources)
        {
            _configuration = configuration;
            _actions = actions;
            _modules = modules;
            _resources = resources;
        }

        public Specification Build()
        {
            var actions = _actions.GetActions();
            return new Specification
                {
                    dataTypes = new List<DataType>(),
                    modules = GetModules(actions)
                };
        }

        private List<Models.Module> GetModules(IList<ActionCall> actions)
        {
            if (_configuration.OrphanedActions == OrphanedActionsBehavior.ThrowException)
            {
                var orphanedActions = actions.Where(x => !_modules.HasModule(x)).ToList();
                if (orphanedActions.Any()) throw new OrphanedActionException(orphanedActions.Select(x => x.HandlerType.FullName + "." + x.Method.Name));
            }

            var modules = actions
                .Where(x => (_configuration.OrphanedActions == OrphanedActionsBehavior.Ignore && 
                             _modules.HasModule(x)) || 
                            _configuration.OrphanedActions == OrphanedActionsBehavior.AddToDefaultModule)
                .GroupBy(x => _modules.GetModule(x) ?? _configuration.DefaultModule)
                .Select(x => new Models.Module {
                    name = x.Key.Name,
                    comments = x.Key.Comments,
                    resources = GetResources(x)
                })
                .OrderBy(x => x.name);
            return modules.ToList();
        }

        private List<Models.Resource> GetResources(IEnumerable<ActionCall> actions)
        {
            return null;
        }
    }
}
