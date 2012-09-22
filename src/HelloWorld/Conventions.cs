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
            ApplyConvention(SwankConvention.Create(x => x
                .At("documentation")
                .AppliesTo<Conventions>()
                .Where(y => y.OutputType() != typeof(FubuContinuation))));

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