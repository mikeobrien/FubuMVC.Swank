using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Specification
{
    public class SpecificationService : ISpecificationService
    {
        private class ActionMapping
        {
            public ActionCall Action { get; set; }
            public ModuleDescription Module { get; set; }
            public ResourceDescription Resource { get; set; }
        }

        private class TypeDef
        {
            public TypeDef(System.Type type, ActionCall action = null)
            {
                Type = type;
                Action = action;
            }

            public System.Type Type { get; private set; }
            public ActionCall Action { get; private set; }
        }

        private readonly Configuration _configuration;
        private readonly ActionSource _actions;
        private readonly ITypeDescriptorCache _typeCache;
        private readonly IDescriptionSource<ActionCall, ModuleDescription> _modules;
        private readonly IDescriptionSource<ActionCall, ResourceDescription> _resources;
        private readonly IDescriptionSource<ActionCall, EndpointDescription> _endpoints;
        private readonly IDescriptionSource<PropertyInfo, MemberDescription> _members;
        private readonly IDescriptionSource<FieldInfo, OptionDescription> _options;
        private readonly IDescriptionSource<ActionCall, List<ErrorDescription>> _errors;
        private readonly IDescriptionSource<System.Type, DataTypeDescription> _dataTypes;

        public SpecificationService(
            Configuration configuration, 
            ActionSource actions,
            ITypeDescriptorCache typeCache,
            IDescriptionSource<ActionCall, ModuleDescription> modules,
            IDescriptionSource<ActionCall, ResourceDescription> resources,
            IDescriptionSource<ActionCall, EndpointDescription> endpoints,
            IDescriptionSource<PropertyInfo, MemberDescription> members,
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
            _members = members;
            _options = options;
            _errors = errors;
            _dataTypes = dataTypes;
        }

        public Specification Generate()
        {
            var actionMapping = GetActionMapping(_actions.GetActions());
            CheckForOrphanedActions(actionMapping);
            var specification = new Specification {
                    types = GetTypes(actionMapping.Select(x => x.Action).ToList()),
                    modules = GetModules(actionMapping.Where(x => x.Module != null).ToList()),
                    resources = GetResources(actionMapping.Where(x => x.Module == null).ToList())
                };
            if (_configuration.MergeSpecificationPath.IsNotEmpty())
                MergeSepcification(specification, _configuration.MergeSpecificationPath);
            return specification;
        }

        private List<ActionMapping> GetActionMapping(IEnumerable<ActionCall> actions)
        {
            return actions
                .Where(x => !x.Method.HasAttribute<HideAttribute>() && !x.HandlerType.HasAttribute<HideAttribute>())
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
                .Where(x => x.HasInput && !x.ParentChain().Route.AllowsGet() && !x.ParentChain().Route.AllowsDelete())
                .Select(x => new TypeDef(x.InputType().GetListElementType() ?? x.InputType(), x))
                .Concat(actions.Where(x => x.HasOutput)
                               .SelectDistinct(x => x.OutputType())
                               .Select(x => new TypeDef(x.GetListElementType() ?? x)))
                .ToList();
            return rootTypes
                .Concat(rootTypes.SelectMany(GetTypes))
                .DistinctBy(x => x.Type, x => x.Action)
                .Select(x => {
                    var description = _dataTypes.GetDescription(x.Type);
                    var type = description.WhenNotNull(y => y.Type).Otherwise(x.Type);
                    return new Type {
                        id = x.Action != null ? type.GetHash(x.Action.Method) : type.GetHash(),
                        name = description.WhenNotNull(y => y.Name).OtherwiseDefault(),
                        comments = description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                        members = GetMembers(type, x.Action)
                    };
                })
                .OrderBy(x => x.name).ToList();
        }

        private List<TypeDef> GetTypes(TypeDef type)
        {
            var types = _typeCache.GetPropertiesFor(type.Type).Select(x => x.Value)
                .Where(x => !x.IsHidden() &&
                            !(x.PropertyType.GetListElementType() ?? x.PropertyType).IsSystemType() && 
                            !x.PropertyType.IsEnum &&
                            !x.IsAutoBound())
                .Select(x => x.PropertyType.GetListElementType() ?? x.PropertyType)
                .Distinct()
                .ToList();
            return types.Select(x => new TypeDef(x))
                        .Concat(types.Select(x => new TypeDef(x))
                        .SelectMany(GetTypes))
                        .Distinct()
                        .ToList();
        }

        private List<Member> GetMembers(System.Type type, ActionCall action)
        {
            return _typeCache.GetPropertiesFor(type).Select(x => x.Value)
                .Where(x => !x.IsHidden() &&
                            !x.IsAutoBound() &&
                            !x.IsQuerystring(action) &&
                            !x.IsUrlParameter(action))
                .Select(x => {
                        var description = _members.GetDescription(x);
                        var memberType = description.WhenNotNull(y => y.Type).Otherwise(x.PropertyType.GetListElementType(), x.PropertyType);
                        return new Member {
                            name = description.WhenNotNull(y => y.Name).OtherwiseDefault(),
                            comments = description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                            defaultValue = description.WhenNotNull(y => y.DefaultValue).WhenNotNull(z => z.ToString()).OtherwiseDefault(),
                            required = description.WhenNotNull(y => y.Required).OtherwiseDefault(),
                            type = memberType.IsSystemType() ? memberType.ToFriendlyName() : memberType.GetHash(),
                            collection = x.PropertyType.IsArray || x.PropertyType.IsList(),
                            options = GetOptions(x.PropertyType)
                        };
                    })
                .ToList();
        } 

        private List<Module> GetModules(IEnumerable<ActionMapping> actionMapping)
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

        private List<Resource> GetResources(IEnumerable<ActionMapping> actionMapping)
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
                .Select(x => {
                    var endpoint = _endpoints.GetDescription(x);
                    var route = x.ParentChain().Route;
                    return new Endpoint {
                        name = endpoint.WhenNotNull(y => y.Name).OtherwiseDefault(),
                        comments = endpoint.WhenNotNull(y => y.Comments).OtherwiseDefault(),
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
                    var description = _members.GetDescription(property);
                    return new UrlParameter {
                            name = description.WhenNotNull(y => y.Name).OtherwiseDefault(),
                            comments = description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
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
                            !x.Value.IsAutoBound())
                .Select(x => {
                    var description = _members.GetDescription(x.Value);
                    return new QuerystringParameter {
                        name = description.WhenNotNull(y => y.Name).OtherwiseDefault(),
                        comments = description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                        type = (x.Value.PropertyType.GetListElementType() ?? x.Value.PropertyType).ToFriendlyName(),
                        options = GetOptions(x.Value.PropertyType),
                        defaultValue = description.DefaultValue.WhenNotNull(y => y.ToString()).OtherwiseDefault(),
                        multipleAllowed = x.Value.PropertyType.IsArray || x.Value.PropertyType.IsList(),
                        required = description.Required
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
            var rootType = dataType.WhenNotNull(x => x.Type).Otherwise(type);
            return new Data
                {
                    name = dataType.WhenNotNull(y => y.Name).OtherwiseDefault(),
                    comments = dataType.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                    type = action.WhenNotNull(rootType.GetHash).Otherwise(rootType.GetHash()),
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
                            name = option.WhenNotNull(y => y.Name).OtherwiseDefault(),
                            comments = option.WhenNotNull(y => y.Comments).OtherwiseDefault(), 
                            value = x.Name
                        };
                    }).OrderBy(x => x.name ?? x.value).ToList();
        }

        private void MergeSepcification(Specification specification, string path)
        {
            if (!Path.IsPathRooted(path))
                path = HttpContext.Current.WhenNotNull(x => x.Server.MapPath(path)).Otherwise(Path.GetFullPath(path));

            var mergeSpecification = new JavaScriptSerializer().Deserialize<Specification>(File.ReadAllText(path));

            specification.types.AddRange(mergeSpecification.types.Where(x => specification.types.All(y => y.id != x.id)));
            
            specification.modules.SelectMany(x => x.resources.Select(y => new { Module = x, Resource = y }))
                .Join(mergeSpecification.modules.SelectMany(x => x.resources.Select(y => new { Module = x, Resource = y })),
                        x => x.Module.name + "." + x.Resource.name, x => x.Module.name + "." + x.Resource.name, 
                        (source, merge) => new { Source = source.Resource, Merge = merge.Resource })
                .ToList().ForEach(x => x.Source.endpoints.AddRange(x.Merge.endpoints.Where(y => x.Source.endpoints.All(z => y.url != z.url))));
            specification.modules.Join(mergeSpecification.modules, x => x.name, x => x.name, (source, merge) => new { Source = source, Merge = merge})
                .ToList().ForEach(x => x.Source.resources.AddRange(x.Merge.resources.Where(y => x.Source.resources.All(z => y.name != z.name))));
            specification.modules.AddRange(mergeSpecification.modules.Where(x => specification.modules.All(y => y.name != x.name)));
            
            specification.resources.Join(mergeSpecification.resources, x => x.name, x => x.name, (source, merge) => new { Source = source, Merge = merge })
                .ToList().ForEach(x => x.Source.endpoints.AddRange(x.Merge.endpoints.Where(y => x.Source.endpoints.All(z => y.url != z.url))));
            specification.resources.AddRange(mergeSpecification.resources.Where(x => specification.resources.All(y => y.name != x.name)));
        }
    }
}
