using System;
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
        private static Specification _specification;

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
            return _specification ?? (_specification = BuildSpecification());
        }

        private Specification BuildSpecification()
        {
            var actionMapping = GetActionMapping(_actions.GetActions());
            CheckForOrphanedActions(actionMapping);
            var specification = new Specification {
                    Name = _configuration.Name,
                    Comments = actionMapping.Select(x => x.Action.HandlerType.Assembly).Distinct()
                        .Select(x => x.FindTextResourceNamed("*" + _configuration.Comments)).FirstOrDefault(x => x != null),
                    Copyright = _configuration.Copyright,
                    Types = GetTypes(actionMapping.Select(x => x.Action).ToList()),
                    Modules = GetModules(actionMapping.Where(x => x.Module != null).ToList()),
                    Resources = GetResources(actionMapping.Where(x => x.Module == null).ToList())
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
                        Id = x.Action != null ? type.GetHash(x.Action.Method) : type.GetHash(),
                        Name = description.WhenNotNull(y => y.Name).OtherwiseDefault(),
                        Comments = description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                        Members = GetMembers(type, x.Action)
                    };
                })
                .OrderBy(x => x.Name).ToList();
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
                            Name = description.WhenNotNull(y => y.Name).OtherwiseDefault(),
                            Comments = description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                            DefaultValue = description.WhenNotNull(y => y.DefaultValue).WhenNotNull(z => z.ToString()).OtherwiseDefault(),
                            Required = description.WhenNotNull(y => y.Required).OtherwiseDefault(),
                            Type = memberType.IsSystemType() ? memberType.ToFriendlyName() : memberType.GetHash(),
                            Collection = x.PropertyType.IsArray || x.PropertyType.IsList(),
                            Options = GetOptions(x.PropertyType)
                        };
                    })
                .ToList();
        } 

        private List<Module> GetModules(IEnumerable<ActionMapping> actionMapping)
        {
            return actionMapping
                .GroupBy(x => x.Module)
                .Select(x => new Module {
                    Name = x.Key.Name,
                    Comments = x.Key.Comments,
                    Resources = GetResources(x.Select(y => y).ToList())
                })
                .OrderBy(x => x.Name).ToList();
        }

        private List<Resource> GetResources(IEnumerable<ActionMapping> actionMapping)
        {
            return actionMapping
                .GroupBy(x => x.Resource)
                .Select(x => new Resource {
                    Name = x.Key.Name,
                    Comments = x.Key.Comments,
                    Endpoints = GetEndpoints(x.Select(y => y.Action))
                })
                .OrderBy(x => x.Name).ToList();
        }

        private List<Endpoint> GetEndpoints(IEnumerable<ActionCall> actions)
        {
            return actions
                .Select(x => {
                    var endpoint = _endpoints.GetDescription(x);
                    var route = x.ParentChain().Route;
                    return new Endpoint {
                        Name = endpoint.WhenNotNull(y => y.Name).OtherwiseDefault(),
                        Comments = endpoint.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                        Url = route.Pattern,
                        Method = route.AllowedHttpMethods.FirstOrDefault(),
                        UrlParameters = x.HasInput ? GetUrlParameters(x) : null,
                        QuerystringParameters = x.HasInput ? GetQuerystringParameters(x) : null,
                        Errors = GetErrors(x),
                        Request = x.HasInput && (route.AllowsPost() || route.AllowsPut()) ? GetData(x.InputType(), x.Method) : null,
                        Response = x.HasOutput ? GetData(x.OutputType()) : null
                    };
                }).OrderBy(x => x.Url).ThenBy(x => x.Method).ToList();
        }

        private List<UrlParameter> GetUrlParameters(ActionCall action)
        {
            var properties = _typeCache.GetPropertiesFor(action.InputType());
            return action.ParentChain().Route.Input.RouteParameters.Select(
                x => {
                    var property = properties[x.Name];
                    var description = _members.GetDescription(property);
                    return new UrlParameter {
                            Name = description.WhenNotNull(y => y.Name).OtherwiseDefault(),
                            Comments = description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                            Type = property.PropertyType.ToFriendlyName(),
                            Options = GetOptions(property.PropertyType)
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
                        Name = description.WhenNotNull(y => y.Name).OtherwiseDefault(),
                        Comments = description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                        Type = (x.Value.PropertyType.GetListElementType() ?? x.Value.PropertyType).ToFriendlyName(),
                        Options = GetOptions(x.Value.PropertyType),
                        DefaultValue = description.DefaultValue.WhenNotNull(y => y.ToString()).OtherwiseDefault(),
                        MultipleAllowed = x.Value.PropertyType.IsArray || x.Value.PropertyType.IsList(),
                        Required = description.Required
                    };
                }).OrderBy(x => x.Name).ToList();
        }

        private List<Error> GetErrors(ActionCall action)
        {
            return _errors.GetDescription(action)
                .Select(x => new Error {
                    Status = x.Status,
                    Name = x.Name,
                    Comments = x.Comments
                }).OrderBy(x => x.Status).ToList();
        }

        private Data GetData(System.Type type, MethodInfo action = null)
        {
            var dataType = _dataTypes.GetDescription(type);
            var rootType = dataType.WhenNotNull(x => x.Type).Otherwise(type);
            return new Data
                {
                    Name = dataType.WhenNotNull(y => y.Name).OtherwiseDefault(),
                    Comments = dataType.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                    Type = action.WhenNotNull(rootType.GetHash).Otherwise(rootType.GetHash()),
                    Collection = type.IsArray || type.IsList()
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
                            Name = option.WhenNotNull(y => y.Name).OtherwiseDefault(),
                            Comments = option.WhenNotNull(y => y.Comments).OtherwiseDefault(), 
                            Value = x.Name
                        };
                    }).OrderBy(x => x.Name ?? x.Value).ToList();
        }

        private void MergeSepcification(Specification specification, string path)
        {
            if (!Path.IsPathRooted(path))
                path = HttpContext.Current.WhenNotNull(x => x.Server.MapPath(path)).Otherwise(Path.GetFullPath(path));

            var mergeSpecification = new JavaScriptSerializer().Deserialize<Specification>(File.ReadAllText(path));

            specification.Types.AddRange(mergeSpecification.Types.Where(x => specification.Types.All(y => y.Id != x.Id)));
            
            specification.Modules.SelectMany(x => x.Resources.Select(y => new { Module = x, Resource = y }))
                .Join(mergeSpecification.Modules.SelectMany(x => x.Resources.Select(y => new { Module = x, Resource = y })),
                        x => x.Module.Name + "." + x.Resource.Name, x => x.Module.Name + "." + x.Resource.Name, 
                        (source, merge) => new { Source = source.Resource, Merge = merge.Resource })
                .ToList().ForEach(x => x.Source.Endpoints.AddRange(x.Merge.Endpoints.Where(y => x.Source.Endpoints.All(z => y.Url != z.Url))));
            specification.Modules.Join(mergeSpecification.Modules, x => x.Name, x => x.Name, (source, merge) => new { Source = source, Merge = merge})
                .ToList().ForEach(x => x.Source.Resources.AddRange(x.Merge.Resources.Where(y => x.Source.Resources.All(z => y.Name != z.Name))));
            specification.Modules.AddRange(mergeSpecification.Modules.Where(x => specification.Modules.All(y => y.Name != x.Name)));
            
            specification.Resources.Join(mergeSpecification.Resources, x => x.Name, x => x.Name, (source, merge) => new { Source = source, Merge = merge })
                .ToList().ForEach(x => x.Source.Endpoints.AddRange(x.Merge.Endpoints.Where(y => x.Source.Endpoints.All(z => y.Url != z.Url))));
            specification.Resources.AddRange(mergeSpecification.Resources.Where(x => specification.Resources.All(y => y.Name != x.Name)));
        }
    }
}
