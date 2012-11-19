using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Http.AspNet;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Swank.Description;

namespace FubuMVC.Swank.Extensions
{
    public static class FubuExtensions
    {
        public static bool IsSwank(this ActionCall action)
        {
            return action.HandlerType.Assembly == Assembly.GetExecutingAssembly();
        }

        public static string GetRouteResource(this IRouteDefinition route)
        {
            return "/" + Regex.Replace(route.Pattern, "/*\\{.*?\\}", "").Trim('/');
        }

        public static string FirstPatternSegment(this IRouteDefinition route)
        {
            return route.Pattern.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        }

        public static bool AllowsGet(this IRouteDefinition route) { return route.AllowsMethod("GET"); }
        public static bool AllowsPost(this IRouteDefinition route) { return route.AllowsMethod("POST"); }
        public static bool AllowsPut(this IRouteDefinition route) { return route.AllowsMethod("PUT"); }
        public static bool AllowsUpdate(this IRouteDefinition route) { return route.AllowsMethod("UPDATE"); }
        public static bool AllowsDelete(this IRouteDefinition route) { return route.AllowsMethod("DELETE"); }

        public static bool AllowsMethod(this IRouteDefinition route, string method)
        {
            return route.AllowedHttpMethods.Any(y => y.Equals(method, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsAutoBound(this PropertyInfo property)
        {
            return RequestPropertyValueSource.IsSystemProperty(property);
        }

        public static bool IsHidden(this PropertyInfo property)
        {
            return property.PropertyType.HasAttribute<HideAttribute>() || 
            property.HasAttribute<HideAttribute>() ||
            property.HasAttribute<XmlIgnoreAttribute>(); 
        }

        public static bool IsUrlParameter(this PropertyInfo property, ActionCall action)
        {
            return action != null && action.ParentChain().Route.Input.RouteParameters.Any(x => x.Name == property.Name);
        }

        public static bool IsQuerystring(this PropertyInfo property, ActionCall action)
        {
            return action != null && 
                action.ParentChain().Route.Input.RouteParameters.All(x => x.Name != property.Name) && 
                (property.HasAttribute<QueryStringAttribute>() ||
                 !(action.ParentChain().Route.AllowsPost() || 
                   action.ParentChain().Route.AllowsPut() || 
                   action.ParentChain().Route.AllowsUpdate()));
        }

        public static ServiceRegistry AddService<T>(this ServiceRegistry services, Type concreteType, params object[] dependencies)
        {
            var objectDef = new ObjectDef(concreteType);
            dependencies.Where(x => x != null).ToList().ForEach(objectDef.DependencyByValue);
            services.AddService(typeof(T), objectDef);
            return services;
        }
    }
}