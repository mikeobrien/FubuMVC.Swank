using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Specification
{
    public class SpecificationService
    {
        private class BehaviorMapping
        {
            public ActionCall FirstAction { get; set; }
            public ModuleDescription Module { get; set; }
            public ResourceDescription Resource { get; set; }

            public BehaviorChain Chain { get; set; }
        }

        public static readonly Func<string, int> HttpVerbRank = x => { switch (x.IsEmpty() ? null : x.ToLower()) 
            { case "get": return 0; case "post": return 1; case "put": return 2; 
              case "update": return 3; case "delete": return 5; default: return 4; } };

        private readonly Configuration _configuration;
        private readonly BehaviorSource _behaviors;
        private readonly ITypeDescriptorCache _typeCache;
        private readonly IDescriptionConvention<BehaviorChain, ModuleDescription> _moduleConvention;
        private readonly IDescriptionConvention<BehaviorChain, ResourceDescription> _resourceConvention;
        private readonly IDescriptionConvention<BehaviorChain, EndpointDescription> _endpointConvention;
        private readonly IDescriptionConvention<PropertyInfo, MemberDescription> _memberConvention;
        private readonly IDescriptionConvention<BehaviorChain, List<StatusCodeDescription>> _statusCodeConvention;
        private readonly IDescriptionConvention<BehaviorChain, List<HeaderDescription>> _headerConvention;
        private readonly TypeGraphFactory _typeGraphFactory;
        private readonly DataDescriptionFactory _dataDescriptionFactory;
        private readonly OptionFactory _optionFactory;

        public SpecificationService(
            Configuration configuration, 
            BehaviorSource behaviors,
            ITypeDescriptorCache typeCache,
            IDescriptionConvention<BehaviorChain, ModuleDescription> moduleConvention,
            IDescriptionConvention<BehaviorChain, ResourceDescription> resourceConvention,
            IDescriptionConvention<BehaviorChain, EndpointDescription> endpointConvention,
            IDescriptionConvention<PropertyInfo, MemberDescription> memberConvention,
            IDescriptionConvention<BehaviorChain, List<StatusCodeDescription>> statusCodeConvention,
            IDescriptionConvention<BehaviorChain, List<HeaderDescription>> headerConvention,
            TypeGraphFactory typeGraphFactory,
            DataDescriptionFactory dataDescriptionFactory,
            OptionFactory optionFactory)
        {
            _configuration = configuration;
            _behaviors = behaviors;
            _typeCache = typeCache;
            _moduleConvention = moduleConvention;
            _resourceConvention = resourceConvention;
            _endpointConvention = endpointConvention;
            _memberConvention = memberConvention;
            _statusCodeConvention = statusCodeConvention;
            _typeGraphFactory = typeGraphFactory;
            _dataDescriptionFactory = dataDescriptionFactory;
            _optionFactory = optionFactory;
            _headerConvention = headerConvention;
        }

        public Specification Generate()
        {
            var behaviorMappings = GetBehaviorMapping(_behaviors.GetChains());

            CheckForOrphanedChains(behaviorMappings);

            var modules = GetModules(behaviorMappings.Where(x => x.Module != null).ToList());
            var resources = GetResources(behaviorMappings.Where(x => x.Module == null).ToList());
            if (resources.Any()) modules.Add(new Module
            {
                Name = Module.DefaultName,
                Resources = resources
            });

            var specification = new Specification {
                    Name = _configuration.Name,
                    Comments = _configuration.AppliesToAssemblies
                        .Select(x => x.FindTextResourceNamed("*" + _configuration.Comments))
                        .FirstOrDefault(x => x != null),
                    Modules = modules
                };
            return specification;
        }

        private List<BehaviorMapping> GetBehaviorMapping(IEnumerable<BehaviorChain> chains)
        {
            return chains
                .Where(x => !x.Calls.Any(c => c.Method.HasAttribute<HideAttribute>() || c.HandlerType.HasAttribute<HideAttribute>()))
                .Select(c => new { Chain = c, Module = _moduleConvention.GetDescription(c), Resource = _resourceConvention.GetDescription(c) })
                .Where(x => ((_configuration.OrphanedModuleActions == OrphanedActions.Exclude && x.Module != null) ||
                              _configuration.OrphanedModuleActions != OrphanedActions.Exclude))
                .Where(x => ((_configuration.OrphanedResourceActions == OrphanedActions.Exclude && x.Resource != null) ||
                              _configuration.OrphanedResourceActions != OrphanedActions.Exclude))
                .Select(x => new BehaviorMapping {
                    Chain = x.Chain,
                    FirstAction = x.Chain.FirstCall(),
                    Module = _configuration.OrphanedModuleActions == OrphanedActions.UseDefault
                        ? x.Module ?? _configuration.DefaultModuleFactory(x.Chain)
                        : x.Module,
                    Resource = _configuration.OrphanedResourceActions == OrphanedActions.UseDefault
                        ? x.Resource ?? _configuration.DefaultResourceFactory(x.Chain)
                        : x.Resource
                }).ToList();
        }

        private void CheckForOrphanedChains(IList<BehaviorMapping> behaviorMappings)
        {
            if (_configuration.OrphanedModuleActions == OrphanedActions.Fail)
            {
                var orphanedModuleActions = behaviorMappings.Where(x => x.Module == null).ToList();
                if (orphanedModuleActions.Any()) throw new OrphanedModuleActionException(
                    orphanedModuleActions.Select(x => x.FirstAction.HandlerType.FullName + "." + x.FirstAction.Method.Name));
            }

            if (_configuration.OrphanedResourceActions == OrphanedActions.Fail)
            {
                var orphanedActions = behaviorMappings.Where(x => x.Resource == null).ToList();
                if (orphanedActions.Any()) throw new OrphanedResourceActionException(
                    orphanedActions.Select(x => x.FirstAction.HandlerType.FullName + "." + x.FirstAction.Method.Name));
            }
        } 

        private List<Module> GetModules(IEnumerable<BehaviorMapping> behaviorMappings)
        {
            return behaviorMappings
                .GroupBy(x => x.Module)
                .Select(x => _configuration.ModuleOverrides.Apply(new Module {
                    Name = x.Key.Name,
                    Comments = x.Key.Comments,
                    Resources = GetResources(x.Select(y => y).ToList())
                }))
                .OrderBy(x => x.Name).ToList();
        }

        private List<Resource> GetResources(IEnumerable<BehaviorMapping> behaviorMappings)
        {
            return behaviorMappings
                .GroupBy(x => x.Resource)
                .Select(x => _configuration.ResourceOverrides.Apply(new Resource {
                    Name = x.Key.Name,
                    Comments = x.Key.Comments ?? x.First().FirstAction.HandlerType.Assembly
                        .FindTextResourceNamed(x.First().FirstAction.HandlerType.Namespace + ".resource"),
                    Endpoints = GetEndpoints(x.Select(y => y.Chain))
                }))
                .OrderBy(x => x.Name).ToList();
        }

        private List<Endpoint> GetEndpoints(IEnumerable<BehaviorChain> chains)
        {
            return chains
                .Select(chain => {
                    var endpoint = _endpointConvention.GetDescription(chain);
                    var route = chain.Route;
                    var querystring = chain.FirstCall().HasInput ? GetQuerystringParameters(chain) : null;
                    return _configuration.EndpointOverrides.Apply(chain, new Endpoint {
                        Name = endpoint.WhenNotNull(y => y.Name).OtherwiseDefault(),
                        Comments = endpoint.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                        Url = route.Pattern.EnusureStartsWith("/") + querystring.Join(y => "{0}={{{0}}}".ToFormat(y.Name), "?", "&", ""),
                        Method = route.AllowedHttpMethods.FirstOrDefault(),
                        UrlParameters = chain.FirstCall().HasInput ? GetUrlParameters(chain) : null,
                        QuerystringParameters = querystring,
                        StatusCodes = GetStatusCodes(chain),
                        Headers = GetHeaders(chain),
                        Request = GetRequestData(chain, endpoint),
                        Response = GetResponseData(chain, endpoint)
                    });
                }).OrderBy(x => x.Url.Split('?').First()).ThenBy(x => HttpVerbRank(x.Method)).ToList();
        }

        private Data GetRequestData(BehaviorChain chain, EndpointDescription endpoint)
        {
            var firstCall = chain.FirstCall();

            if (!firstCall.HasInput ||
                chain.Route.AllowsGet() ||
                chain.Route.AllowsDelete()) return null;

            return _configuration.RequestOverrides.Apply(chain, new Data
            {
                Comments = endpoint.RequestComments,
                Description = _dataDescriptionFactory.Create(_typeGraphFactory.BuildGraph(firstCall.InputType(), chain.FirstCall()))
            });
        }

        private Data GetResponseData(BehaviorChain chain, EndpointDescription endpoint)
        {
            var lastCall = chain.LastCall();
            if (!lastCall.HasOutput) return null;
            return _configuration.ResponseOverrides.Apply(chain, new Data
            {
                Comments = endpoint.ResponseComments,
                Description = _dataDescriptionFactory.Create(_typeGraphFactory.BuildGraph(lastCall.OutputType()))
            });
        }

        private List<UrlParameter> GetUrlParameters(BehaviorChain chain)
        {
            var action = chain.FirstCall();

            var properties = _typeCache.GetPropertiesFor(action.InputType());
            return action.ParentChain().Route.Input.RouteParameters.Select(
                x => {
                    var property = properties[x.Name];
                    var description = _memberConvention.GetDescription(property);
                    return _configuration.UrlParameterOverrides.Apply(chain, property, new UrlParameter {
                            Name = description.WhenNotNull(y => y.Name).OtherwiseDefault(),
                            Comments = description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                            Type = property.PropertyType.GetXmlName(_configuration.EnumValue == EnumValue.AsString),
                            Options = _optionFactory.BuildOptions(property.PropertyType)
                        });
                }).ToList();
        }

        private List<QuerystringParameter> GetQuerystringParameters(BehaviorChain chain)
        {
            var action = chain.FirstCall();

            return _typeCache.GetPropertiesFor(action.InputType())
                .Where(x => x.Value.IsQuerystring(action) && 
                            !x.Value.HasAttribute<HideAttribute>() && 
                            !x.Value.IsAutoBound())
                .Select(x => {
                    var description = _memberConvention.GetDescription(x.Value);
                    return _configuration.QuerystringOverrides.Apply(chain, x.Value, new QuerystringParameter {
                        Name = description.WhenNotNull(y => y.Name).OtherwiseDefault(),
                        Comments = description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                        Type = (x.Value.PropertyType.GetListElementType() ?? x.Value.PropertyType).GetXmlName(_configuration.EnumValue == EnumValue.AsString),
                        Options = _optionFactory.BuildOptions(x.Value.PropertyType),
                        DefaultValue = description.DefaultValue.WhenNotNull(y => y.ToDefaultValueString(_configuration)).OtherwiseDefault(),
                        MultipleAllowed = x.Value.PropertyType.IsArray || x.Value.PropertyType.IsList(),
                        Required = !description.Optional
                    });
                }).OrderBy(x => x.Name).ToList();
        }

        private List<StatusCode> GetStatusCodes(BehaviorChain chain)
        {
            return _statusCodeConvention.GetDescription(chain)
                .Select(x => _configuration.StatusCodeOverrides.Apply(chain, new StatusCode {
                    Code = x.Code,
                    Name = x.Name,
                    Comments = x.Comments
                })).OrderBy(x => x.Code).ToList();
        }

        private List<Header> GetHeaders(BehaviorChain chain)
        {
            return _headerConvention.GetDescription(chain)
                .Select(x => _configuration.HeaderOverrides.Apply(chain, new Header {
                    Type = x.Type.ToString(),
                    Name = x.Name,
                    Comments = x.Comments,
                    Optional = x.Optional
                })).OrderBy(x => x.Type).ThenBy(x => x.Name).ToList();
        }
    }
}
