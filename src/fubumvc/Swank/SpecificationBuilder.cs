using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using Swank.Description;
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
        private readonly IDescriptionSource<ActionCall, ModuleDescription> _modules;
        private readonly IDescriptionSource<ActionCall, ResourceDescription> _resources;
        private readonly IDescriptionSource<ActionCall, EndpointDescription> _endpoints;

        public SpecificationBuilder(
            Configuration configuration, 
            ActionSource actions,
            IDescriptionSource<ActionCall, ModuleDescription> modules,
            IDescriptionSource<ActionCall, ResourceDescription> resources,
            IDescriptionSource<ActionCall, EndpointDescription> endpoints)
        {
            _configuration = configuration;
            _actions = actions;
            _modules = modules;
            _resources = resources;
            _endpoints = endpoints;
        }

        public Specification Build()
        {
            var actions = _actions.GetActions();
            var modules = GetModules(actions);
            var hasModules = modules.Any(x => !string.IsNullOrEmpty(x.name));
            return new Specification
                {
                    dataTypes = new List<DataType>(),
                    modules = hasModules ? modules : new List<Module>(),
                    resources = !hasModules ? modules.SelectMany(x => x.resources).ToList() : new List<Resource>()
                };
        }

        private List<Module> GetModules(IList<ActionCall> actions)
        {
            if (_configuration.OrphanedModuleActions == OrphanedActions.Fail)
            {
                var orphanedActions = actions.Where(x => !_modules.HasDescription(x)).ToList();
                if (orphanedActions.Any()) throw new OrphanedModuleActionException(orphanedActions.Select(x => x.HandlerType.FullName + "." + x.Method.Name));
            }

            var modules = actions
                .Where(x => (_configuration.OrphanedModuleActions == OrphanedActions.Exclude && 
                             _modules.HasDescription(x)) || 
                            _configuration.OrphanedModuleActions == OrphanedActions.UseDefault)
                .GroupBy(x => _modules.GetDescription(x) ?? _configuration.DefaultModuleFactory(x))
                .Select(x => new Module {
                    name = x.Key.Name,
                    comments = x.Key.Comments,
                    resources = GetResources(x.ToList())
                })
                .OrderBy(x => x.name);
            return modules.ToList();
        }

        private List<Resource> GetResources(IList<ActionCall> actions)
        {
            if (_configuration.OrphanedResourceActions == OrphanedActions.Fail)
            {
                var orphanedActions = actions.Where(x => !_resources.HasDescription(x)).ToList();
                if (orphanedActions.Any()) throw new OrphanedResourceActionException(orphanedActions.Select(x => x.HandlerType.FullName + "." + x.Method.Name));
            }

            var resources = actions
                .Where(x => (_configuration.OrphanedResourceActions == OrphanedActions.Exclude &&
                             _resources.HasDescription(x)) ||
                            _configuration.OrphanedResourceActions == OrphanedActions.UseDefault)
                .GroupBy(x => _resources.GetDescription(x) ?? _configuration.DefaultResourceFactory(x))
                .Select(x => new Resource {
                    name = x.Key.Name,
                    comments = x.Key.Comments,
                    endpoints = GetEndpoints(x)
                })
                .OrderBy(x => x.name);
            return resources.ToList();
        }

        private List<Endpoint> GetEndpoints(IEnumerable<ActionCall> actions)
        {
            return actions.Select(x =>
                {
                    var endpoint = _endpoints.GetDescription(x);
                    return new Endpoint {
                        name = endpoint != null ? endpoint.Name : null,
                        comments = endpoint != null ? endpoint.Comments : null,
                        url = x.ParentChain().Route.Pattern,
                        method = x.ParentChain().Route.AllowedHttpMethods.FirstOrDefault(),
                        urlParameters = new List<UrlParameter>(),
                        querystringParameters = new List<QuerystringParameter>(),
                        errors = new List<Error>(),
                        request = new Data(),
                        response = new Data()
                    };
                }).ToList();
        }
    }
}
