using FubuMVC.Core;
using FubuMVC.Core.Continuations;
using FubuMVC.Swank;
using HelloWorld.Infrastructure;

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
                .UrlPolicy(RegexUrlPolicy.Create()
                    .IgnoreAssemblyNamespace<Conventions>()
                    .IgnoreClassName()
                    .IgnoreMethodNames("Execute")
                    .ConstrainClassToHttpGetEndingWith("GetHandler")
                    .ConstrainClassToHttpPostEndingWith("PostHandler")
                    .ConstrainClassToHttpPutEndingWith("PutHandler")
                    .ConstrainClassToHttpDeleteEndingWith("DeleteHandler"));

            Media.ApplyContentNegotiationToActions(x =>
                x.IsInAssembly<Conventions>() && !x.HasAnyOutputBehavior());
        }
    }
}