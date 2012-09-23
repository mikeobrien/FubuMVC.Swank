using System;
using FubuMVC.Swank;
using FubuMVC.Swank.Extensions;
using NUnit.Framework;
using Should;

namespace Tests.Specification.SpecificationBuilderTypeTests
{
    public class TypeTests : TestBase
    {
        [Test]
        public void should_not_include_input_types_from_module_excluded_endpoints()
        {
            var spec = BuildSpec<ExcludedHandlers.PostHandler>(x => x.OnOrphanedModuleAction(OrphanedActions.Exclude));
            spec.types.ShouldNotContainAnyInputType<ExcludedHandlers.Request, ExcludedHandlers.PostHandler>();
            spec.types.ShouldContainOneInputType<ExcludedHandlers.InModule.Request, ExcludedHandlers.InModule.PostHandler>();
        }

        [Test]
        public void should_not_include_input_types_from_resource_excluded_endpoints()
        {
            var spec = BuildSpec<ExcludedHandlers.PostHandler>(x => x.OnOrphanedResourceAction(OrphanedActions.Exclude));
            spec.types.ShouldNotContainAnyInputType<ExcludedHandlers.Request, ExcludedHandlers.PostHandler>();
            spec.types.ShouldContainOneInputType<ExcludedHandlers.InResource.Request, ExcludedHandlers.InResource.PostHandler>();
        }

        [Test]
        public void should_not_include_output_types_from_module_excluded_endpoints()
        {
            var spec = BuildSpec<ExcludedHandlers.PostHandler>(x => x.OnOrphanedModuleAction(OrphanedActions.Exclude));
            spec.types.ShouldNotContainAnyOutputTypes<ExcludedHandlers.Response>();
            spec.types.ShouldContainOneOutputType<ExcludedHandlers.InModule.Response>();
        }

        [Test]
        public void should_not_include_output_types_from_resource_excluded_endpoints()
        {
            var spec = BuildSpec<ExcludedHandlers.PostHandler>(x => x.OnOrphanedResourceAction(OrphanedActions.Exclude));
            spec.types.ShouldNotContainAnyOutputTypes<ExcludedHandlers.Response>();
            spec.types.ShouldContainOneOutputType<ExcludedHandlers.InResource.Response>();
        }

        [Test]
        public void should_not_include_input_types_from_hidden_endpoints()
        {
            var spec = BuildSpec<HiddenHandlers.PostHandler>();
            spec.types.ShouldNotContainAnyInputType<HiddenHandlers.HiddenRequest, HiddenHandlers.HiddenPostHandler>();
            spec.types.ShouldContainOneInputType<HiddenHandlers.Request, HiddenHandlers.PostHandler>();
        }

        [Test]
        public void should_not_include_output_types_from_hidden_endpoints()
        {
            var spec = BuildSpec<HiddenHandlers.PostHandler>();
            spec.types.ShouldNotContainAnyOutputTypes<HiddenHandlers.HiddenResponse>();
            spec.types.ShouldContainOneOutputType<HiddenHandlers.Response>();
        }

        [Test]
        public void should_not_include_input_types_for_get_and_delete_handlers()
        {
            var spec = BuildSpec<HandlerVerb.PostHandler>();
            spec.types.ShouldNotContainAnyInputType<HandlerVerb.GetRequest, HandlerVerb.GetHandler>();
            spec.types.ShouldNotContainAnyInputType<HandlerVerb.DeleteRequest, HandlerVerb.DeleteHandler>();
            spec.types.ShouldContainOneInputType<HandlerVerb.PostRequest, HandlerVerb.PostHandler>();
            spec.types.ShouldContainOneInputType<HandlerVerb.PutRequest, HandlerVerb.PutHandler>();
            spec.types.ShouldContainOneInputType<HandlerVerb.UpdateRequest, HandlerVerb.UpdateHandler>();
        }

        [Test]
        public void should_define_unique_input_types()
        {
            var spec = BuildSpec<TypeEnumeration.PostHandler>();
            spec.types.ShouldNotContainAnyType<TypeEnumeration.Request>();
            spec.types.ShouldContainOneInputType<TypeEnumeration.Request, TypeEnumeration.PostHandler>();
            spec.types.ShouldContainOneInputType<TypeEnumeration.Request, TypeEnumeration.PutHandler>();
        }

        [Test]
        public void should_define_shared_output_types()
        {
            BuildSpec<TypeEnumeration.PostHandler>().types.ShouldContainOneOutputType<TypeEnumeration.Response>();
        }

        [Test]
        public void should_recursively_include_member_types()
        {
            var spec = BuildSpec<RecursingTypes.PostHandler>();
            spec.types.Count.ShouldEqual(8);
            spec.types.ShouldContainOneInputType<RecursingTypes.Request, RecursingTypes.PostHandler>();
            spec.types.ShouldContainOneOutputType<RecursingTypes.Response>();
            spec.types.ShouldContainOneType<RecursingTypes.Device>();
            spec.types.ShouldContainOneType<RecursingTypes.DeviceInfo>();
            spec.types.ShouldContainOneType<RecursingTypes.DeviceClass>();
            spec.types.ShouldContainOneType<RecursingTypes.DeviceId>();
            spec.types.ShouldContainOneType<RecursingTypes.Printer>();
            spec.types.ShouldContainOneType<RecursingTypes.PrinterInfo>();
        }

        [Test]
        public void should_exclude_member_types_marked_as_hidden()
        {
            var spec = BuildSpec<RecursingTypes.PostHandler>();
            spec.types.ShouldContainOneInputType<RecursingTypes.Request, RecursingTypes.PostHandler>();
            spec.types.ShouldNotContainAnyType<RecursingTypes.DeviceClassId>();
        }

        [Test]
        public void should_exclude_member_types_of_properties_marked_as_hidden()
        {
            var spec = BuildSpec<RecursingTypes.PostHandler>();
            spec.types.ShouldContainOneInputType<RecursingTypes.Request, RecursingTypes.PostHandler>();
            spec.types.ShouldNotContainAnyType<RecursingTypes.PrinterClass>();
            spec.types.ShouldNotContainAnyType<RecursingTypes.PrinterClassId>();
        }

        [Test]
        public void should_exclude_member_types_of_properties_are_system_types()
        {
            var spec = BuildSpec<RecursingTypes.PostHandler>();
            spec.types.ShouldContainOneInputType<RecursingTypes.Request, RecursingTypes.PostHandler>();
            spec.types.ShouldNotContainAnyType<Uri>();
        }

        [Test]
        public void should_exclude_member_types_of_properties_are_enumerations()
        {
            var spec = BuildSpec<RecursingTypes.PostHandler>();
            spec.types.ShouldContainOneInputType<RecursingTypes.Request, RecursingTypes.PostHandler>();
            spec.types.ShouldNotContainAnyType<RecursingTypes.Options>();
        }

        [Test]
        public void should_order_types_by_type_name()
        {
            var spec = BuildSpec<TypeOrder.PostHandler>();
            spec.types[0].id.ShouldEqual(typeof(TypeOrder.AResponse).GetHash());
            spec.types[1].id.ShouldEqual(typeof(TypeOrder.MyRequest).GetHash(typeof(TypeOrder.PostHandler).GetExecuteMethod()));
            spec.types[2].id.ShouldEqual(typeof(TypeOrder.YourResponse).GetHash());
            spec.types[3].id.ShouldEqual(typeof(TypeOrder.ZeeRequest).GetHash(typeof(TypeOrder.UpdateHandler).GetExecuteMethod()));
        }

        [Test]
        public void should_define_unique_input_type_and_output_type_when_they_are_the_same_type()
        {
            var spec = BuildSpec<SameInputOutputType.PutHandler>();
            spec.types.ShouldContainOneOutputType<SameInputOutputType.Data>();
            spec.types.ShouldContainOneInputType<SameInputOutputType.Data, SameInputOutputType.PutHandler>();
        }

        [Test]
        public void should_set_type_description()
        {
            var spec = BuildSpec<TypeDescription.PostHandler>();
            spec.types[0].name.ShouldEqual("Request");
            spec.types[0].comments.ShouldBeNull();
            spec.types[1].name.ShouldEqual("Response");
            spec.types[1].comments.ShouldEqual("This is a nice response type.");
        }
    }
}