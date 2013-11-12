using System;
using FubuMVC.Swank;
using FubuMVC.Swank.Extensions;
using NUnit.Framework;
using Should;
using Tests.Specification.SpecificationService.Tests;

namespace Tests.Specification.SpecificationService.TypeTests
{
    public class Tests : InteractionContext
    {
        [Test]
        public void should_not_include_input_types_from_module_excluded_endpoints()
        {
            var spec = BuildSpec<ExcludedHandlers.PostHandler>(x => x.OnOrphanedModuleAction(OrphanedActions.Exclude));
            spec.Types.ShouldNotContainAnyInputType<ExcludedHandlers.Request, ExcludedHandlers.PostHandler>();
            spec.Types.ShouldContainOneInputType<ExcludedHandlers.InModule.Request, ExcludedHandlers.InModule.PostHandler>();
        }

        [Test]
        public void should_not_include_input_types_from_resource_excluded_endpoints()
        {
            var spec = BuildSpec<ExcludedHandlers.PostHandler>(x => x.OnOrphanedResourceAction(OrphanedActions.Exclude));
            spec.Types.ShouldNotContainAnyInputType<ExcludedHandlers.Request, ExcludedHandlers.PostHandler>();
            spec.Types.ShouldContainOneInputType<ExcludedHandlers.InResource.Request, ExcludedHandlers.InResource.PostHandler>();
        }

        [Test]
        public void should_not_include_output_types_from_module_excluded_endpoints()
        {
            var spec = BuildSpec<ExcludedHandlers.PostHandler>(x => x.OnOrphanedModuleAction(OrphanedActions.Exclude));
            spec.Types.ShouldNotContainAnyOutputTypes<ExcludedHandlers.Response>();
            spec.Types.ShouldContainOneOutputType<ExcludedHandlers.InModule.Response>();
        }

        [Test]
        public void should_not_include_output_types_from_resource_excluded_endpoints()
        {
            var spec = BuildSpec<ExcludedHandlers.PostHandler>(x => x.OnOrphanedResourceAction(OrphanedActions.Exclude));
            spec.Types.ShouldNotContainAnyOutputTypes<ExcludedHandlers.Response>();
            spec.Types.ShouldContainOneOutputType<ExcludedHandlers.InResource.Response>();
        }

        [Test]
        public void should_not_include_input_types_from_hidden_endpoints()
        {
            var spec = BuildSpec<HiddenHandlers.PostHandler>();
            spec.Types.ShouldNotContainAnyInputType<HiddenHandlers.HiddenRequest, HiddenHandlers.HiddenPostHandler>();
            spec.Types.ShouldContainOneInputType<HiddenHandlers.Request, HiddenHandlers.PostHandler>();
        }

        [Test]
        public void should_not_include_output_types_from_hidden_endpoints()
        {
            var spec = BuildSpec<HiddenHandlers.PostHandler>();
            spec.Types.ShouldNotContainAnyOutputTypes<HiddenHandlers.HiddenResponse>();
            spec.Types.ShouldContainOneOutputType<HiddenHandlers.Response>();
        }

        [Test]
        public void should_not_include_input_types_for_get_and_delete_handlers()
        {
            var spec = BuildSpec<HandlerVerb.PostHandler>();
            spec.Types.ShouldNotContainAnyInputType<HandlerVerb.GetRequest, HandlerVerb.GetHandler>();
            spec.Types.ShouldNotContainAnyInputType<HandlerVerb.DeleteRequest, HandlerVerb.DeleteHandler>();
            spec.Types.ShouldContainOneInputType<HandlerVerb.PostRequest, HandlerVerb.PostHandler>();
            spec.Types.ShouldContainOneInputType<HandlerVerb.PutRequest, HandlerVerb.PutHandler>();
            spec.Types.ShouldContainOneInputType<HandlerVerb.UpdateRequest, HandlerVerb.UpdateHandler>();
        }

        [Test]
        public void should_define_unique_input_types()
        {
            var spec = BuildSpec<TypeEnumeration.PostHandler>();
            spec.Types.ShouldNotContainAnyType<TypeEnumeration.Request>();
            spec.Types.ShouldContainOneInputType<TypeEnumeration.Request, TypeEnumeration.PostHandler>();
            spec.Types.ShouldContainOneInputType<TypeEnumeration.Request, TypeEnumeration.PutHandler>();
        }

        [Test]
        public void should_define_shared_output_types()
        {
            var spec = BuildSpec<TypeEnumeration.PostHandler>();
            spec.Types.ShouldContainOneOutputType<TypeEnumeration.Response>();
        }

        [Test]
        public void should_recursively_include_member_types()
        {
            var spec = BuildSpec<RecursingTypes.PostHandler>();
            spec.Types.Count.ShouldEqual(8);
            spec.Types.ShouldContainOneInputType<RecursingTypes.Request, RecursingTypes.PostHandler>();
            spec.Types.ShouldContainOneOutputType<RecursingTypes.Response>();
            spec.Types.ShouldContainOneType<RecursingTypes.Device>();
            spec.Types.ShouldContainOneType<RecursingTypes.DeviceInfo>();
            spec.Types.ShouldContainOneType<RecursingTypes.DeviceClass>();
            spec.Types.ShouldContainOneType<RecursingTypes.DeviceId>();
            spec.Types.ShouldContainOneType<RecursingTypes.Printer>();
            spec.Types.ShouldContainOneType<RecursingTypes.PrinterInfo>();
        }

        [Test]
        public void should_not_recursively_follow_cyclic_references()
        {
            var spec = BuildSpec<CyclicReferences.PostHandler>();
            spec.Types.Count.ShouldEqual(3);
            spec.Types.ShouldContainOneInputType<CyclicReferences.Request, CyclicReferences.PostHandler>();
            spec.Types.ShouldContainOneOutputType<CyclicReferences.Response>();
            spec.Types.ShouldContainOneType<CyclicReferences.Node>();
        }

        [Test]
        public void should_exclude_member_types_marked_as_hidden()
        {
            var spec = BuildSpec<RecursingTypes.PostHandler>();
            spec.Types.ShouldContainOneInputType<RecursingTypes.Request, RecursingTypes.PostHandler>();
            spec.Types.ShouldNotContainAnyType<RecursingTypes.DeviceClassId>();
        }

        [Test]
        public void should_exclude_member_types_of_properties_marked_as_hidden()
        {
            var spec = BuildSpec<RecursingTypes.PostHandler>();
            spec.Types.ShouldContainOneInputType<RecursingTypes.Request, RecursingTypes.PostHandler>();
            spec.Types.ShouldNotContainAnyType<RecursingTypes.PrinterClass>();
            spec.Types.ShouldNotContainAnyType<RecursingTypes.PrinterClassId>();
        }

        [Test]
        public void should_exclude_member_types_of_properties_are_system_types()
        {
            var spec = BuildSpec<RecursingTypes.PostHandler>();
            spec.Types.ShouldContainOneInputType<RecursingTypes.Request, RecursingTypes.PostHandler>();
            spec.Types.ShouldNotContainAnyType<Uri>();
        }

        [Test]
        public void should_exclude_member_types_of_properties_are_enumerations()
        {
            var spec = BuildSpec<RecursingTypes.PostHandler>();
            spec.Types.ShouldContainOneInputType<RecursingTypes.Request, RecursingTypes.PostHandler>();
            spec.Types.ShouldNotContainAnyType<RecursingTypes.Options>();
        }

        [Test]
        public void should_order_types_by_type_name()
        {
            var spec = BuildSpec<TypeOrder.PostHandler>();
            spec.Types[0].Id.ShouldEqual(typeof(TypeOrder.AResponse).GetHash());
            spec.Types[1].Id.ShouldEqual(typeof(TypeOrder.MyRequest).GetHash(typeof(TypeOrder.PostHandler).GetExecuteMethod()));
            spec.Types[2].Id.ShouldEqual(typeof(TypeOrder.YourResponse).GetHash());
            spec.Types[3].Id.ShouldEqual(typeof(TypeOrder.ZeeRequest).GetHash(typeof(TypeOrder.UpdateHandler).GetExecuteMethod()));
        }

        [Test]
        public void should_define_unique_input_type_and_output_type_when_they_are_the_same_type()
        {
            var spec = BuildSpec<SameInputOutputType.PutHandler>();
            spec.Types.ShouldContainOneOutputType<SameInputOutputType.Data>();
            spec.Types.ShouldContainOneInputType<SameInputOutputType.Data, SameInputOutputType.PutHandler>();
        }

        [Test]
        public void should_set_type_description()
        {
            var spec = BuildSpec<TypeDescription.PostHandler>();
            spec.Types[0].Name.ShouldEqual("Request");
            spec.Types[0].Comments.ShouldBeNull();
            spec.Types[1].Name.ShouldEqual("Response");
            spec.Types[1].Comments.ShouldEqual("This is a nice response type.");
        }
    }
}