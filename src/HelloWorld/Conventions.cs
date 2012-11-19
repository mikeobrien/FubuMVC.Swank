using FubuMVC.Core;
using FubuMVC.RegexUrlPolicy;
using FubuMVC.Swank;
using FubuMVC.Swank.Extensions;
using FubuMVC.Swank.Specification;

namespace HelloWorld
{
    public class Conventions : FubuRegistry
    {
        public Conventions()
        {
            // --------Swank configuration -------

            Import<Swank>(x => x
                // Url of the documentation.
                .AtUrl("documentation")

                // Name that shows up in the page title and title bar.
                .Named("Universal Exports API")

                // Footer copyright. {year} macro is replaced with the current year.
                .WithCopyright("Copyright &copy; {year} Universal Exports")

                // AppliesTo* indicates where to look for endpoints to include document.
                .AppliesToThisAssembly()

                // The name of an embedded comments file that will be displayed on the 
                // docs home page. The extension is not required.
                .WithComments("Comments")

                // Include your own stylesheets and scripts to override 
                // the styles and behavior of the docs.
                .WithStylesheets("~/styles/styles.css")
                .WithScripts("~/scripts/script.js")

                // Merge in a spec file. Useful if you want to consolidate 
                // specs from other platforms.
                .MergeThisSpecification("~/spec.json")

                // Why define these over and over when you can just override them in one place?
                .OverrideEndpoints((action, endpoint) => {
                    endpoint.StatusCodes.Add(new StatusCode { Code = 404, Name = "Not Found", Comments = "The item was not found!" });
                    endpoint.Headers.Add(new Header { Type = "Request", Name = "api-key", Comments = "The is the api key found under your account.", Optional = true });
                })

                .OverridePropertiesWhen((propertyinfo, property) => property.Comments = "This is the id of the user.", 
                    (propertyinfo, property) => propertyinfo.Name == "UserId" && propertyinfo.IsGuid()));

            // -----------------------------------

            Actions
                .IncludeTypeNamesSuffixed("Handler")
                .IncludeMethodsPrefixed("Execute");

            Routes
                .OverrideFolders()
                .UrlPolicy(RegexUrlPolicy.Create(x => x
                    .IgnoreAssemblyNamespace<Conventions>()
                    .IgnoreClassName()
                    .IgnoreMethodNames("Execute")
                    .ConstrainClassToGetEndingWith("GetHandler")
                    .ConstrainClassToPostEndingWith("PostHandler")
                    .ConstrainClassToPutEndingWith("PutHandler")
                    .ConstrainClassToDeleteEndingWith("DeleteHandler")));

            Policies.Add(x => x.Conneg.ApplyConneg());
        }
    }
}