using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Routing;
using FubuMVC.Core.Registration.DSL;
using FubuMVC.Core.Registration.Nodes;

namespace HelloWorld.Infrastructure
{
    public static class Registration
    {
        public static ActionCallCandidateExpression IncludeTypeNamesSuffixed(this ActionCallCandidateExpression expression, params string[] suffix)
        {
            suffix.ToList().ForEach(x => expression.IncludeTypes(y => y.Name.EndsWith(x)));
            return expression;
        }

        public static ActionCallCandidateExpression IncludeMethodsPrefixed(this ActionCallCandidateExpression expression, params string[] prefix)
        {
            prefix.ToList().ForEach(x => expression.IncludeMethods(y => y.Name.StartsWith(x)));
            return expression;
        }

        public static bool IsInAssembly<T>(this ActionCall call)
        {
            return call.HandlerType.Assembly == typeof(T).Assembly;
        }

        public static RouteConventionExpression OverrideFolders(this RouteConventionExpression routeConvention)
        {
            RouteTable.Routes.Add(new IgnoreFilesRoute());
            RouteTable.Routes.RouteExistingFiles = true;
            return routeConvention;
        }

        private class IgnoreFilesRoute : Route
        {
            public IgnoreFilesRoute() : base(null, new StopRoutingHandler()) { }

            public override RouteData GetRouteData(HttpContextBase httpContext)
            {
                return HostingEnvironment.VirtualPathProvider.FileExists(httpContext.Request.AppRelativeCurrentExecutionFilePath) ?
                    new RouteData(this, RouteHandler) : null;
            }

            public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary routeValues)
            {
                return null;
            }
        }
    }
}