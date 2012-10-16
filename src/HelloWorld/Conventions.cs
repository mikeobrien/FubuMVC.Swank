using FubuMVC.Core;
using FubuMVC.RegexUrlPolicy;
using FubuMVC.Swank;

namespace HelloWorld
{
    public class Conventions : FubuRegistry
    {
        public Conventions()
        {
            // --------Swank configuration -------

            Import<Swank>(x => x
                .AtUrl("documentation")
                .Named("Universal Exports API")
                .WithCopyright("Copyright &copy; {year} Universal Exports")
                .AppliesToThisAssembly()
                .WithComments("Comments")
                .WithStylesheets("~/styles/styles.css")
                .WithScripts("~/scripts/script.js")
                .MergeThisSpecification("~/spec.json"));

            // -----------------------------------

            IncludeDiagnostics(true);

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

            Media.ApplyContentNegotiationToActions(x =>
                x.IsInThisAssembly() && !x.HasAnyOutputBehavior());
        }
    }
}