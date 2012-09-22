using System;
using FubuMVC.Swank;
using NUnit.Framework;
using Should;

namespace Tests.SpecificationBuilderTypeTests
{
    public class MemberTests : TestBase
    {
        [Test]
        public void should_enumerate_type_members()
        {
            var type = BuildSpec<MemberEnumeration.PutHandler>().types
                .GetType<MemberEnumeration.Request, MemberEnumeration.PutHandler>();

            type.ShouldContainMember<MemberEnumeration.Request>(x => x.Name);
            type.ShouldContainMember<MemberEnumeration.Request>(x => x.Birthday);
        }

        [Test]
        public void should_exclude_auto_bound_properties_from_input_type_members()
        {
            var type = BuildSpec<MemberEnumeration.PutHandler>().types
                .GetType<MemberEnumeration.Request, MemberEnumeration.PutHandler>();

            type.ShouldNotContainMember<MemberEnumeration.Request>(x => x.UserAgent);
        }

        [Test]
        public void should_exclude_url_parameters_from_input_type_members()
        {
            var type = BuildSpec<MemberEnumeration.PutHandler>().types
                .GetType<MemberEnumeration.Request, MemberEnumeration.PutHandler>();

            type.ShouldNotContainMember<MemberEnumeration.Request>(x => x.Id);
        }

        [Test]
        public void should_exclude_querystring_parameters_from_input_type_members()
        {
            var type = BuildSpec<MemberEnumeration.PutHandler>().types
                .GetType<MemberEnumeration.Request, MemberEnumeration.PutHandler>();

            type.ShouldNotContainMember<MemberEnumeration.Request>(x => x.Sort);
        }

        [Test]
        public void should_exclude_members_marked_with_hide()
        {
            var type = BuildSpec<MemberEnumeration.PutHandler>().types
                .GetType<MemberEnumeration.Request, MemberEnumeration.PutHandler>();

            type.ShouldNotContainMember<MemberEnumeration.Request>(x => x.Code);
        }

        [Test]
        public void should_exclude_members_marked_with_xml_ignore()
        {
            var type = BuildSpec<MemberEnumeration.PutHandler>().types
                .GetType<MemberEnumeration.Request, MemberEnumeration.PutHandler>();

            type.ShouldNotContainMember<MemberEnumeration.Request>(x => x.Key);
        }

        [Test]
        public void should_set_member_description()
        {
            var type = BuildSpec<MemberDescription.PutHandler>().types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>();

            type.GetMember<MemberDescription.Request>(x => x.Name).comments.ShouldBeNull();
            type.GetMember<MemberDescription.Request>(x => x.Birthday).comments.ShouldEqual("This is da birfday yo.");
        }

        [Test]
        public void should_indicate_a_members_default_value()
        {
            var type = BuildSpec<MemberDescription.PutHandler>().types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>();

            type.GetMember<MemberDescription.Request>(x => x.Name).defaultValue.ShouldEqual("John Joseph Dingleheimer Smith");
            type.GetMember<MemberDescription.Request>(x => x.Birthday).defaultValue.ShouldBeNull();
        }

        [Test]
        public void should_indicate_if_a_member_is_required()
        {
            var type = BuildSpec<MemberDescription.PutHandler>().types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>();

            type.GetMember<MemberDescription.Request>(x => x.Name).required.ShouldBeFalse();
            type.GetMember<MemberDescription.Request>(x => x.Birthday).required.ShouldBeTrue();
        }

        [Test]
        public void should_set_member_name_to_xml_override()
        {
            var type = BuildSpec<MemberDescription.PutHandler>().types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>();

            type.ShouldContainMember("R2D2");
        }

        [Test]
        public void should_reference_system_type_members_as_the_type_name()
        {
            var member = BuildSpec<MemberDescription.PutHandler>().types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>()
                   .GetMember<MemberDescription.Request>(x => x.Name);

            member.collection.ShouldBeFalse();
            member.type.ShouldEqual("string");
        }

        [Test]
        public void should_reference_non_system_type_members_as_the_type_id()
        {
            var member = BuildSpec<MemberDescription.PutHandler>().types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>()
                   .GetMember<MemberDescription.Request>(x => x.Drive);

            member.collection.ShouldBeFalse();
            member.type.ShouldEqual(typeof(MemberDescription.HyperDrive).GetHash());
        }

        [Test]
        public void should_reference_collections_of_system_types_as_the_type_name()
        {
            var member = BuildSpec<MemberDescription.PutHandler>().types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>()
                   .GetMember<MemberDescription.Request>(x => x.Ids);

            member.collection.ShouldBeTrue();
            member.type.ShouldEqual("int");
        }

        [Test]
        public void should_reference_collections_of_non_system_types_as_the_type_id()
        {
            var member = BuildSpec<MemberDescription.PutHandler>().types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>()
                   .GetMember<MemberDescription.Request>(x => x.Drives);

            member.collection.ShouldBeTrue();
            member.type.ShouldEqual(typeof(MemberDescription.HyperDrive).GetHash());
        }

        [Test]
        public void should_enumerate_options_for_enum_members()
        {
            var member = BuildSpec<MemberDescription.PutHandler>().types
                   .GetType<MemberDescription.Request, MemberDescription.PutHandler>()
                   .GetMember<MemberDescription.Request>(x => x.Status);

            member.options.Count.ShouldEqual(2);
            
            var option = member.options[0];
            option.name.ShouldEqual("Active yo!");
            option.comments.ShouldEqual("This is a very nice status.");
            option.value.ShouldEqual("Active");

            option = member.options[1];
            option.name.ShouldBeNull();
            option.comments.ShouldBeNull();
            option.value.ShouldEqual("Inactive");
        }
    }
}