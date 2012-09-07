using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using Swank.Models;

namespace Swank
{
    public class OrphanedModuleActionException : Exception {
        public OrphanedModuleActionException(IEnumerable<string> actions) 
            : base(string.Format("The following actions are not associated with a module. Either assocate them with a module or turn off orphaned action exceptions. {0}",
                    string.Join(", ", actions))) { }
    }

    public class OrphanedResourceActionException : Exception
    {
        public OrphanedResourceActionException(IEnumerable<string> actions)
            : base(string.Format("The following actions are not associated with a resource. Either assocate them with a resource or turn off orphaned action exceptions. {0}",
                    string.Join(", ", actions))) { }
    }

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
            if (_configuration.OrphanedModuleActions == OrphanedActions.Fail)
            {
                var orphanedActions = actions.Where(x => !_modules.HasModule(x)).ToList();
                if (orphanedActions.Any()) throw new OrphanedModuleActionException(orphanedActions.Select(x => x.HandlerType.FullName + "." + x.Method.Name));
            }

            var modules = actions
                .Where(x => (_configuration.OrphanedModuleActions == OrphanedActions.Exclude && 
                             _modules.HasModule(x)) || 
                            _configuration.OrphanedModuleActions == OrphanedActions.Default)
                .GroupBy(x => _modules.GetModule(x) ?? _configuration.DefaultModuleFactory(x))
                .Select(x => new Models.Module {
                    name = x.Key.Name,
                    comments = x.Key.Comments,
                    resources = GetResources(x.ToList())
                })
                .OrderBy(x => x.name);
            return modules.ToList();
        }

        private List<Models.Resource> GetResources(IList<ActionCall> actions)
        {
            if (_configuration.OrphanedResourceActions == OrphanedActions.Fail)
            {
                var orphanedActions = actions.Where(x => !_resources.HasResource(x)).ToList();
                if (orphanedActions.Any()) throw new OrphanedResourceActionException(orphanedActions.Select(x => x.HandlerType.FullName + "." + x.Method.Name));
            }

            var resources = actions
                .Where(x => (_configuration.OrphanedResourceActions == OrphanedActions.Exclude &&
                             _resources.HasResource(x)) ||
                            _configuration.OrphanedResourceActions == OrphanedActions.Default)
                .GroupBy(x => _resources.GetResource(x) ?? _configuration.DefaultResourceFactory(x))
                .Select(x => new Models.Resource {
                    name = x.Key.Name,
                    comments = x.Key.Comments,
                    endpoints = GetEndpoints(x)
                })
                .OrderBy(x => x.name);
            return resources.ToList();
        }

        private List<Endpoint> GetEndpoints(IEnumerable<ActionCall> actions)
        {
            return actions.Select(x => new Endpoint {
                    url = x.ParentChain().Route.Pattern,
                    method = x.ParentChain().Route.AllowedHttpMethods.FirstOrDefault(),
                    comments = "",
                    urlParameters = new List<UrlParameter>(),
                    querystringParameters = new List<QuerystringParameter>(),
                    errors = new List<Error>(),
                    request = new Data(),
                    response = new Data()
                }).ToList();
        }
    }
}
