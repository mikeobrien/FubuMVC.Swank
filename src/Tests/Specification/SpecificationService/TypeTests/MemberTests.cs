using FubuMVC.Swank;
using FubuMVC.Swank.Extensions;
using NUnit.Framework;
using Should;
using Tests.Specification.SpecificationService.Tests;

namespace Tests.Specification.SpecificationService.TypeTests
{
    public class MemberTests : InteractionContext
    {
        [Test]
        public void should_enumerate_type_members()
        {
            var spec = BuildSpec<MemberEnumeration.PutHandler>();
            var type = spec.Types
                .GetType<MemberEnumeration.Request, MemberEnumeration.PutHandler>();

            type.Members.Count.ShouldEqual(2);
            type.ShouldContainMember<MemberEnumeration.Request>(x => x.Name);
            type.ShouldContainMember<MemberEnumeration.Request>(x => x.Birthday);

            type = spec.Types
                .GetType<MemberEnumeration.Response>();

            type.Members.Count.ShouldEqual(1);
            type.ShouldContainMember<MemberEnumeration.Response>(x => x.Id);
        }

        [Test]
        public void should_enumerate_projection_members()
        {
            var spec = BuildSpec<ProjectionMemberEnumeration.PutHandler>();
            var type = spec.Types.GetType<ProjectionMemberEnumeration.Request, ProjectionMemberEnumeration.PutHandler>();

            type.Members.Count.ShouldEqual(2);
            type.ShouldContainMember<ProjectionMemberEnumeration.Model>(x => x.Id);
            type.ShouldContainMember<ProjectionMemberEnumeration.Model>(x => x.Key);

            type = spec.Types.GetType<ProjectionMemberEnumeration.Response>();

            type.Members.Count.ShouldEqual(2);
            type.ShouldContainMember<ProjectionMemberEnumeration.Model>(x => x.Name);
            type.ShouldContainMember<ProjectionMemberEnumeration.Model>(x => x.Sort);
        }

        [Test]
        public void should_exclude_auto_bound_properties_from_input_type_members()
        {
            var type = BuildSpec<MemberEnumeration.PutHandler>().Types
                .GetType<MemberEnumeration.Request, MemberEnumeration.PutHandler>();

            type.ShouldNotContainMember<MemberEnumeration.Request>(x => x.UserAgent);
        }

        [Test]
        public void should_exclude_url_parameters_from_input_type_members()
        {
            var type = BuildSpec<MemberEnumeration.PutHandler>().Types
                .GetType<MemberEnumeration.Request, MemberEnumeration.PutHandler>();

            type.ShouldNotContainMember<MemberEnumeration.Request>(x => x.Id);
        }

        [Test]
        public void should_exclude_querystring_parameters_from_input_type_members()
        {
            var type = BuildSpec<MemberEnumeration.PutHandler>().Types
                .GetType<MemberEnumeration.Request, MemberEnumeration.PutHandler>();

            type.ShouldNotContainMember<MemberEnumeration.Request>(x => x.Sort);
        }

        [Test]
        public void should_exclude_members_marked_with_hide()
        {
            var type = BuildSpec<MemberEnumeration.PutHandler>().Types
                .GetType<MemberEnumeration.Request, MemberEnumeration.PutHandler>();

            type.ShouldNotContainMember<MemberEnumeration.Request>(x => x.Code);
        }

        [Test]
        public void should_exclude_members_marked_with_xml_ignore()
        {
            var type = BuildSpec<MemberEnumeration.PutHandler>().Types
                .GetType<MemberEnumeration.Request, MemberEnumeration.PutHandler>();

            type.ShouldNotContainMember<MemberEnumeration.Request>(x => x.Key);
        }

        [Test]
        public void should_set_member_description()
        {
            var type = BuildSpec<MemberDescription.PutHandler>().Types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>();

            type.GetMember<MemberDescription.Request>(x => x.Name).Comments.ShouldBeNull();
            type.GetMember<MemberDescription.Request>(x => x.Birthday).Comments.ShouldEqual("This is da birfday yo.");
        }

        [Test]
        public void should_indicate_a_members_default_value()
        {
            var type = BuildSpec<MemberDescription.PutHandler>().Types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>();

            type.GetMember<MemberDescription.Request>(x => x.Name).DefaultValue.ShouldEqual("John Joseph Dingleheimer Smith");
            type.GetMember<MemberDescription.Request>(x => x.Birthday).DefaultValue.ShouldBeNull();
        }

        [Test]
        public void should_indicate_a_members_custom_formatted_default_value()
        {
            var type = BuildSpec<MemberDescription.PutHandler>(x => x.WithIntegerFormat("0.00")).Types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>();

            type.GetMember<MemberDescription.Request>(x => x.Id).DefaultValue.ShouldEqual("5.00");
        }

        [Test]
        public void should_indicate_a_members_enum_numeric_default_value()
        {
            var type = BuildSpec<MemberDescription.PutHandler>(x => x.WithEnumValueTypeOf(EnumValue.AsNumber)).Types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>();

            type.GetMember<MemberDescription.Request>(x => x.Status).DefaultValue.ShouldEqual("1");
        }

        [Test]
        public void should_indicate_a_members_enum_string_default_value()
        {
            var type = BuildSpec<MemberDescription.PutHandler>(x => x.WithEnumValueTypeOf(EnumValue.AsString)).Types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>();

            type.GetMember<MemberDescription.Request>(x => x.Status).DefaultValue.ShouldEqual("Active");
        }

        [Test]
        public void should_set_member_name_to_xml_override()
        {
            var type = BuildSpec<MemberDescription.PutHandler>().Types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>();

            type.ShouldContainMember("R2D2");
        }

        [Test]
        public void should_reference_system_type_members_as_the_xml_type_name()
        {
            var member = BuildSpec<MemberDescription.PutHandler>().Types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>()
                   .GetMember<MemberDescription.Request>(x => x.Name);

            member.IsArray.ShouldBeFalse();
            member.Type.ShouldEqual(typeof(string).GetXmlName());
        }

        [Test]
        public void should_reference_enum_type_members_as_the_type_name()
        {
            var member = BuildSpec<MemberDescription.PutHandler>().Types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>()
                   .GetMember<MemberDescription.Request>(x => x.Status);

            member.IsArray.ShouldBeFalse();
            member.Type.ShouldEqual("Status");
        }

        [Test]
        public void should_reference_nullable_system_type_members_as_the_xml_type_name()
        {
            var member = BuildSpec<MemberDescription.PutHandler>().Types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>()
                   .GetMember<MemberDescription.Request>(x => x.NullableInt);

            member.IsArray.ShouldBeFalse();
            member.Type.ShouldEqual(typeof(int).GetXmlName());
        }

        [Test]
        public void should_reference_nullable_enum_type_members_as_the_type_name()
        {
            var member = BuildSpec<MemberDescription.PutHandler>().Types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>()
                   .GetMember<MemberDescription.Request>(x => x.NullableStatus);

            member.IsArray.ShouldBeFalse();
            member.Type.ShouldEqual("Status");
        }

        [Test]
        public void should_reference_non_system_type_members_as_the_type_id()
        {
            var member = BuildSpec<MemberDescription.PutHandler>().Types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>()
                   .GetMember<MemberDescription.Request>(x => x.Drive);

            member.IsArray.ShouldBeFalse();
            member.Type.ShouldEqual(typeof(MemberDescription.HyperDrive).GetHash());
        }

        [Test]
        public void should_reference_collections_of_system_types_as_the_type_name()
        {
            var member = BuildSpec<MemberDescription.PutHandler>().Types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>()
                   .GetMember<MemberDescription.Request>(x => x.Ids);

            member.IsArray.ShouldBeTrue();
            member.Type.ShouldEqual("int");
            member.ArrayItemName.ShouldBeNull();
        }

        [Test]
        public void should_reference_collections_of_non_system_types_as_the_type_id()
        {
            var member = BuildSpec<MemberDescription.PutHandler>().Types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>()
                   .GetMember<MemberDescription.Request>(x => x.Drives);

            member.IsArray.ShouldBeTrue();
            member.Type.ShouldEqual(typeof(MemberDescription.HyperDrive).GetHash());
            member.ArrayItemName.ShouldBeNull();
        }

        [Test]
        public void should_set_collections_of_system_types_custom_item_name()
        {
            var member = BuildSpec<MemberDescription.PutHandler>().Types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>()
                   .GetMember<MemberDescription.Request>(x => x.IdsWithCustomItemName);

            member.IsArray.ShouldBeTrue();
            member.Type.ShouldEqual("int");
            member.ArrayItemName.ShouldEqual("Id");
        }

        [Test]
        public void should_set_collections_of_non_system_types_custom_item_name()
        {
            var member = BuildSpec<MemberDescription.PutHandler>().Types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>()
                   .GetMember<MemberDescription.Request>(x => x.DrivesWithCustomItemName);

            member.IsArray.ShouldBeTrue();
            member.Type.ShouldEqual(typeof(MemberDescription.HyperDrive).GetHash());
            member.ArrayItemName.ShouldEqual("Drive");
        }

        [Test]
        public void should_enumerate_options_for_enum_members_with_numeric()
        {
            var member = BuildSpec<MemberDescription.PutHandler>().Types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>()
                   .GetMember<MemberDescription.Request>(x => x.Status);

            member.Options.Count.ShouldEqual(3);
            
            var option = member.Options[0];
            option.Name.ShouldEqual("Active yo!");
            option.Comments.ShouldEqual("This is a very nice status.");
            option.Value.ShouldEqual("1");

            option = member.Options[1];
            option.Name.ShouldEqual("HyperActive");
            option.Comments.ShouldEqual("Very active yo!");
            option.Value.ShouldEqual("2");

            option = member.Options[2];
            option.Name.ShouldEqual("Inactive");
            option.Comments.ShouldBeNull();
            option.Value.ShouldEqual("0");
        }

        [Test]
        public void should_enumerate_options_for_enum_members_with_string_values()
        {
            var member = BuildSpec<MemberDescription.PutHandler>(x => x.WithEnumValueTypeOf(EnumValue.AsString)).Types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>()
                   .GetMember<MemberDescription.Request>(x => x.Status);

            member.Options.Count.ShouldEqual(3);

            var option = member.Options[0];
            option.Name.ShouldEqual("Active yo!");
            option.Comments.ShouldEqual("This is a very nice status.");
            option.Value.ShouldEqual("Active");

            option = member.Options[1];
            option.Name.ShouldEqual("HyperActive");
            option.Comments.ShouldEqual("Very active yo!");
            option.Value.ShouldEqual("HyperActive");

            option = member.Options[2];
            option.Name.ShouldEqual("Inactive");
            option.Comments.ShouldBeNull();
            option.Value.ShouldEqual("Inactive");
        }

        [Test]
        public void should_enumerate_options_for_nullable_enum_members()
        {
            var member = BuildSpec<MemberDescription.PutHandler>(x => x.WithEnumValueTypeOf(EnumValue.AsString)).Types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>()
                   .GetMember<MemberDescription.Request>(x => x.NullableStatus);

            member.Options.Count.ShouldEqual(3);

            var option = member.Options[0];
            option.Name.ShouldEqual("Active yo!");
            option.Comments.ShouldEqual("This is a very nice status.");
            option.Value.ShouldEqual("Active");

            option = member.Options[1];
            option.Name.ShouldEqual("HyperActive");
            option.Comments.ShouldEqual("Very active yo!");
            option.Value.ShouldEqual("HyperActive");

            option = member.Options[2];
            option.Name.ShouldEqual("Inactive");
            option.Comments.ShouldBeNull();
            option.Value.ShouldEqual("Inactive");
        }

        [Test]
        public void should_indicate_if_a_member_is_set_to_be_required()
        {
            var type = BuildSpec<MemberDescription.PutHandler>().Types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>();

            type.GetMember<MemberDescription.Request>(x => x.Name).Required.ShouldBeTrue();
        }

        [Test]
        public void should_default_nullable_types_to_not_required()
        {
            var type = BuildSpec<MemberDescription.PutHandler>(x => x.WithEnumValueTypeOf(EnumValue.AsString)).Types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>();

            type.GetMember<MemberDescription.Request>(x => x.NullableInt).Required.ShouldBeFalse();
            type.GetMember<MemberDescription.Request>(x => x.NullableStatus).Required.ShouldBeFalse();
        }

        [Test]
        public void should_default_null_nullable_types_to_required()
        {
            var type = BuildSpec<MemberDescription.PutHandler>(x => x.WithEnumValueTypeOf(EnumValue.AsString)).Types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>();

            type.GetMember<MemberDescription.Request>(x => x.Id).Required.ShouldBeFalse();
            type.GetMember<MemberDescription.Request>(x => x.Status).Required.ShouldBeTrue();
        }
    }
}