using FubuMVC.Core;
using FubuMVC.Swank;

namespace TestHarness
{
    public class Conventions : FubuRegistry
    {
        public Conventions()
        {
            IncludeDiagnostics(true);

            Actions.IncludeTypesNamed(x => x.EndsWith("Handler"));
            
            Routes
                .HomeIs<IndexGetHandler>(x => x.Execute())
                .IgnoreNamespaceForUrlFrom<Conventions>()
                .IgnoreMethodSuffix("Execute")
                .IgnoreControllerNamesEntirely()
                .ConstrainToHttpMethod(action => action.HandlerType.Name.EndsWith("GetHandler"), "GET");

            Import<Swank>(x => x
                .AppliesToThisAssembly()
                .AtUrl("documentation")
                .Named("Test Harness API")
                .WithCopyright("Copyright &copy; {year} Test Harness")
                .WithStylesheets("~/styles/style.css")
                .WithScripts("~/scripts/script.js")
                .MergeThisSpecification("~/spec.json")
                .HideXml());

            Views.TryToAttachWithDefaultConventions();

            Media.ApplyContentNegotiationToActions(x => x.HandlerType.Assembly == GetType().Assembly);
        }
    }
}