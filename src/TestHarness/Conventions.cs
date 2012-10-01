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
                .MergeThisSpecification("~/spec.json"));

            Views.TryToAttachWithDefaultConventions();

            Media.ApplyContentNegotiationToActions(x => x.HandlerType.Assembly == GetType().Assembly);
        }
    }
}