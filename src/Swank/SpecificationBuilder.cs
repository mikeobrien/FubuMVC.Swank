using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Http.AspNet;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank.Description;

namespace FubuMVC.Swank
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

    public class ActionMapping
    {
        public ActionCall Action { get; set; }
        public ModuleDescription Module { get; set; }
        public ResourceDescription Resource { get; set; }
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
        private readonly IDescriptionSource<System.Type, DataTypeDescription> _dataTypes;

        public SpecificationBuilder(
            Configuration configuration, 
            ActionSource actions,
            ITypeDescriptorCache typeCache,
            IDescriptionSource<ActionCall, ModuleDescription> modules,
            IDescriptionSource<ActionCall, ResourceDescription> resources,
            IDescriptionSource<ActionCall, EndpointDescription> endpoints,
            IDescriptionSource<PropertyInfo, ParameterDescription> parameters,
            IDescriptionSource<FieldInfo, OptionDescription> options,
            IDescriptionSource<ActionCall, List<ErrorDescription>> errors,
            IDescriptionSource<System.Type, DataTypeDescription> dataTypes)
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
            _dataTypes = dataTypes;
        }

        public Specification Build()
        {
            var actionMapping = GetActionMapping(_actions.GetActions());
            CheckForOrphanedActions(actionMapping);
            return new Specification {
                    types = GetTypes(actionMapping.Select(x => x.Action).ToList()),
                    modules = GetModules(actionMapping.Where(x => x.Module != null).ToList()),
                    resources = GetResources(actionMapping.Where(x => x.Module == null).ToList())
                };
        }

        private List<ActionMapping> GetActionMapping(IList<ActionCall> actions)
        {
            return actions
                .Select(x => new { Action = x, Module = _modules.GetDescription(x), Resource = _resources.GetDescription(x) })
                .Where(x => ((_configuration.OrphanedModuleActions == OrphanedActions.Exclude && x.Module != null) ||
                              _configuration.OrphanedModuleActions != OrphanedActions.Exclude))
                .Where(x => ((_configuration.OrphanedResourceActions == OrphanedActions.Exclude && x.Resource != null) ||
                              _configuration.OrphanedResourceActions != OrphanedActions.Exclude))
                .Select(x => new ActionMapping {
                    Action = x.Action,
                    Module = _configuration.OrphanedModuleActions == OrphanedActions.UseDefault ?
                        x.Module ?? _configuration.DefaultModuleFactory(x.Action) : x.Module,
                    Resource = _configuration.OrphanedResourceActions == OrphanedActions.UseDefault ?
                        x.Resource ?? _configuration.DefaultResourceFactory(x.Action) : x.Resource
                }).ToList();
        }

        private void CheckForOrphanedActions(IList<ActionMapping> actionMapping)
        {
            if (_configuration.OrphanedModuleActions == OrphanedActions.Fail)
            {
                var orphanedModuleActions = actionMapping.Where(x => x.Module == null).ToList();
                if (orphanedModuleActions.Any()) throw new OrphanedModuleActionException(
                    orphanedModuleActions.Select(x => x.Action.HandlerType.FullName + "." + x.Action.Method.Name));
            }

            if (_configuration.OrphanedResourceActions == OrphanedActions.Fail)
            {
                var orphanedActions = actionMapping.Where(x => x.Resource == null).ToList();
                if (orphanedActions.Any()) throw new OrphanedResourceActionException(
                    orphanedActions.Select(x => x.Action.HandlerType.FullName + "." + x.Action.Method.Name));
            }
        }

        private List<Type> GetTypes(IList<ActionCall> actions)
        {
            var rootTypes = actions
                .Where(x => x.HasInput && (x.ParentChain().Route.AllowsPost() || x.ParentChain().Route.AllowsPut()))
                .Select(x => x.InputType().GetListElementType() ?? x.InputType())
                .Concat(actions.Where(x => x.HasOutput).Select(x => x.OutputType().GetListElementType() ?? x.OutputType()))
                .Distinct().ToList();
            return rootTypes
                .Concat(rootTypes.SelectMany(GetTypes))
                .Distinct()
                .Select(x => {
                    var dataType = _dataTypes.GetDescription(x);
                    return new Type {
                        id = dataType != null ? dataType.Type.GetHash() : x.GetHash(),
                        name = dataType.GetNameOrDefault(),
                        comments = dataType.GetCommentsOrDefault(),
                        members = null
                    };
                })
                .OrderBy(x => x.name).ToList();
        }

        private List<System.Type> GetTypes(System.Type type)
        {
            var types = _typeCache.GetPropertiesFor(type)
                .Select(x => x.Value.PropertyType)
                .Where(x => !(x.GetElementType() ?? x).IsSystemType() && !x.IsEnum).Distinct().ToList();
            return types.Concat(types.SelectMany(GetTypes)).Distinct().ToList();
        }

        private List<Module> GetModules(IList<ActionMapping> actionMapping)
        {
            return actionMapping
                .GroupBy(x => x.Module)
                .Select(x => new Module {
                    name = x.Key.Name,
                    comments = x.Key.Comments,
                    resources = GetResources(x.Select(y => y).ToList())
                })
                .OrderBy(x => x.name).ToList();
        }

        private List<Resource> GetResources(IList<ActionMapping> actionMapping)
        {
            return actionMapping
                .GroupBy(x => x.Resource)
                .Select(x => new Resource {
                    name = x.Key.Name,
                    comments = x.Key.Comments,
                    endpoints = GetEndpoints(x.Select(y => y.Action))
                })
                .OrderBy(x => x.name).ToList();
        }

        private List<Endpoint> GetEndpoints(IEnumerable<ActionCall> actions)
        {
            return actions
                .Where(x => !x.Method.HasAttribute<HideAttribute>() && !x.HandlerType.HasAttribute<HideAttribute>())
                .Select(x => {
                    var endpoint = _endpoints.GetDescription(x);
                    var route = x.ParentChain().Route;
                    return new Endpoint {
                        name = endpoint.GetNameOrDefault(),
                        comments = endpoint.GetCommentsOrDefault(),
                        url = route.Pattern,
                        method = route.AllowedHttpMethods.FirstOrDefault(),
                        urlParameters = x.HasInput ? GetUrlParameters(x) : null,
                        querystringParameters = x.HasInput ? GetQuerystringParameters(x) : null,
                        errors = GetErrors(x),
                        request = x.HasInput && (route.AllowsPost() || route.AllowsPut()) ? GetData(x.InputType(), x.Method) : null,
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
                            type = property.PropertyType.ToFriendlyName(),
                            options = GetOptions(property.PropertyType)
                        };
                }).ToList();
        }

        private List<QuerystringParameter> GetQuerystringParameters(ActionCall action)
        {
            return _typeCache.GetPropertiesFor(action.InputType())
                .Where(x => x.Value.IsQuerystring(action) && 
                            !x.Value.HasAttribute<HideAttribute>() && 
                            !RequestPropertyValueSource.IsSystemProperty(x.Value))
                .Select(x => {
                    var parameter = _parameters.GetDescription(x.Value);
                    return new QuerystringParameter {
                        name = parameter.GetNameOrDefault(),
                        comments = parameter.GetCommentsOrDefault(),
                        type = (x.Value.PropertyType.GetListElementType() ?? x.Value.PropertyType).ToFriendlyName(),
                        options = GetOptions(x.Value.PropertyType),
                        defaultValue = parameter.DefaultValue != null ? parameter.DefaultValue.ToString() : null,
                        multipleAllowed = x.Value.PropertyType.IsArray || x.Value.PropertyType.IsList()
                    };
                }).OrderBy(x => x.name).ToList();
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

        private Data GetData(System.Type type, MethodInfo action = null)
        {
            var dataType = _dataTypes.GetDescription(type);
            var rootType = dataType != null ? dataType.Type : type;
            return new Data
                {
                    name = dataType.GetNameOrDefault(),
                    comments = dataType.GetCommentsOrDefault(),
                    type = action == null ? rootType.GetHash() : rootType.GetHash(action),
                    collection = type.IsArray || type.IsList()
                };
        } 

        private List<Option> GetOptions(System.Type type)
        {
            return !type.IsEnum ? new List<Option>() :
                type.GetCachedEnumValues()
                    .Where(x => !x.HasAttribute<HideAttribute>())
                    .Select(x => {
                        var option = _options.GetDescription(x);
                        return new Option {
                            name = option.GetNameOrDefault(),
                            comments = option.GetCommentsOrDefault(), 
                            value = x.Name
                        };
                    }).OrderBy(x => x.name ?? x.value).ToList();
        }
    }
}
