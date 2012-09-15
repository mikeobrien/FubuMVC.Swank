using System.Collections.Generic;
using NUnit.Framework;
using Should;
using Swank;
using Swank.Description;

namespace Tests.Description.DataTypeSourceTests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void should_return_default_description_of_datatype()
        {
            var type = typeof(AdminAddressResponse);
            var description = new DataTypeSource().GetDescription(type);
            description.Type.ShouldEqual(type);
            description.Name.ShouldEqual("AdminAddressResponse");
            description.Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_attribute_description_of_datatype()
        {
            var type = typeof(AdminAddressRequest);
            var description = new DataTypeSource().GetDescription(type);
            description.Type.ShouldEqual(type);
            description.Name.ShouldEqual("AdminAddressRequest");
            description.Comments.ShouldEqual("This is an address request yo!");
        }

        [Test]
        public void should_return_attribute_description_of_datatype_and_xml_type_attribute()
        {
            var type = typeof(AdminUserRequest);
            var description = new DataTypeSource().GetDescription(type);
            description.Type.ShouldEqual(type);
            description.Name.ShouldEqual("User");
            description.Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_default_description_of_list_datatype()
        {
            var description = new DataTypeSource().GetDescription(typeof(List<AdminAddressResponse>));
            description.Type.ShouldEqual(typeof(AdminAddressResponse));
            description.Name.ShouldEqual("ArrayOfAdminAddressResponse");
            description.Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_attribute_description_of_inherited_list_datatype()
        {
            var description = new DataTypeSource().GetDescription(typeof(AdminAddresses));
            description.Type.ShouldEqual(typeof(AdminAddressResponse));
            description.Name.ShouldEqual("ArrayOfAdminAddressResponse");
            description.Comments.ShouldEqual("These are addresses yo!");
        }

        [Test]
        public void should_return_attribute_description_of_inherited_list_datatype_with_xml_type_attribute()
        {
            var description = new DataTypeSource().GetDescription(typeof(AdminUsers));
            description.Type.ShouldEqual(typeof(AdminUserResponse));
            description.Name.ShouldEqual("Users");
            description.Comments.ShouldEqual("These are users yo!");
        }
    }
}