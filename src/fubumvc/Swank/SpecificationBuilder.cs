using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using Swank.Description;
using Swank.Models;
using Module = Swank.Models.Module;

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
        private readonly ITypeDescriptorCache _typeCache;
        private readonly IDescriptionSource<ActionCall, ModuleDescription> _modules;
        private readonly IDescriptionSource<ActionCall, ResourceDescription> _resources;
        private readonly IDescriptionSource<ActionCall, EndpointDescription> _endpoints;
        private readonly IDescriptionSource<PropertyInfo, ParameterDescription> _parameters;
        private readonly IDescriptionSource<FieldInfo, OptionDescription> _options;
        private readonly IDescriptionSource<ActionCall, List<ErrorDescription>> _errors;

        public SpecificationBuilder(
            Configuration configuration, 
            ActionSource actions,
            ITypeDescriptorCache typeCache,
            IDescriptionSource<ActionCall, ModuleDescription> modules,
            IDescriptionSource<ActionCall, ResourceDescription> resources,
            IDescriptionSource<ActionCall, EndpointDescription> endpoints,
            IDescriptionSource<PropertyInfo, ParameterDescription> parameters,
            IDescriptionSource<FieldInfo, OptionDescription> options,
            IDescriptionSource<ActionCall, List<ErrorDescription>> errors)
        {
            _configuration = configuration;
            _actions = actions;
            _typeCache = typeCache;
            _modules = modules;
            _resources = resources;
            _endpoints = endpoints;
            _parameters = parameters;
            _options = options;
            _errors = errors;
        }

        public Specification Build()
        {
            var actions = _actions.GetActions();
            var modules = GetModules(actions);
            var hasModules = modules.Any(x => !string.IsNullOrEmpty(x.name));
            return new Specification {
                    dataTypes = new List<DataType>(),
                    modules = hasModules ? modules : new List<Module>(),
                    resources = !hasModules ? modules.SelectMany(x => x.resources).ToList() : new List<Resource>()
                };
        }

        private List<Module> GetModules(IList<ActionCall> actions)
        {
            if (_configuration.OrphanedModuleActions == OrphanedActions.Fail) {
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
            if (_configuration.OrphanedResourceActions == OrphanedActions.Fail) {
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
            return actions.Select(x => {
                    var endpoint = _endpoints.GetDescription(x);
                    return new Endpoint {
                        name = endpoint.GetNameOrDefault(),
                        comments = endpoint.GetCommentsOrDefault(),
                        url = x.ParentChain().Route.Pattern,
                        method = x.ParentChain().Route.AllowedHttpMethods.FirstOrDefault(),
                        urlParameters = x.HasInput ? GetUrlParameters(x) : null,
                        querystringParameters = x.HasInput ? GetQuerystringParameters(x) : null,
                        errors = GetErrors(x),
                        request = x.HasInput ? GetData(x.InputType()) : null,
                        response = x.HasOutput ? GetData(x.OutputType()) : null
                    };
                }).OrderBy(x => x.url).ThenBy(x => x.method).ToList();
        }

        private List<UrlParameter> GetUrlParameters(ActionCall action)
        {
            var properties = _typeCache.GetPropertiesFor(action.InputType());
            return action.ParentChain().Route.Input.RouteParameters.Select(
                x => {
                    var property = properties[x.Name];
                    var parameter = _parameters.GetDescription(property);
                    return new UrlParameter {
                            name = parameter.GetNameOrDefault(),
                            comments = parameter.GetCommentsOrDefault(),
                            dataType = property.PropertyType.ToFriendlyName(),
                            options = GetOptions(property.PropertyType)
                        };
                }).ToList();
        }

        private List<QuerystringParameter> GetQuerystringParameters(ActionCall action)
        {
            return _typeCache.GetPropertiesFor(action.InputType())
                .Where(x => x.Value.IsQuerystring(action))
                .Select(x => {
                    var parameter = _parameters.GetDescription(x.Value);
                    return new QuerystringParameter {
                        name = parameter.GetNameOrDefault(),
                        comments = parameter.GetCommentsOrDefault(),
                        dataType = (x.Value.PropertyType.IsArray ? x.Value.PropertyType.GetElementType() : 
                                   x.Value.PropertyType.IsList() ? x.Value.PropertyType.GetGenericArguments()[0] : x.Value.PropertyType)
                                   .ToFriendlyName(),
                        options = GetOptions(x.Value.PropertyType),
                        defaultValue = parameter.DefaultValue != null ? parameter.DefaultValue.ToString() : null,
                        multipleAllowed = x.Value.PropertyType.IsArray || x.Value.PropertyType.IsList()
                    };
                }).ToList();
        }

        private List<Error> GetErrors(ActionCall action)
        {
            return _errors.GetDescription(action)
                .Select(x => new Error {
                    status = x.Status,
                    name = x.Name,
                    comments = x.Comments
                }).OrderBy(x => x.status).ToList();
        }

        private Data GetData(Type type)
        {
            return new Data();
        } 

        private List<Option> GetOptions(Type type)
        {
            return !type.IsEnum ? new List<Option>() :
                type.GetCachedEnumValues()
                    .Select(x => {
                        var option = _options.GetDescription(x);
                        return new Option {
                            name = option.GetNameOrDefault(),
                            comments = option.GetCommentsOrDefault(), 
                            value = x.Name
                        };
                    }).OrderBy(x => x.name).ToList();
        }
    }
}
