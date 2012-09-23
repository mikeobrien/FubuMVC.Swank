using FubuMVC.Core;
using FubuMVC.Core.Continuations;
using FubuMVC.RegexUrlPolicy;
using FubuMVC.Swank;

namespace HelloWorld
{
    public class Conventions : FubuRegistry
    {
        public Conventions()
        {

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

            Import<Swank>(x => x
                .AtUrl("documentation")
                .AppliesTo<Conventions>());

            Media.ApplyContentNegotiationToActions(x =>
                x.IsInThisAssembly() && !x.HasAnyOutputBehavior());
        }
    }
}