using FubuMVC.Core;
using HelloWorld.Infrastructure;
using Swank;

namespace HelloWorld
{
    public class Conventions : FubuRegistry
    {
        public Conventions()
        {
            ApplyConvention(SwankConvention.Create(x => x
                .At("documentation")
                .AppliesTo<Conventions>()));

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