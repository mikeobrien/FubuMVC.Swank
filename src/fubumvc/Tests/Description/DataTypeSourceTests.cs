using System.Collections.Generic;
using NUnit.Framework;
using Should;
using Swank;
using Swank.Description;
using Tests.Administration.Users;

namespace Tests.Description
{
    [TestFixture]
    public class DataTypeSourceTests
    {
        [Test]
        public void should_return_default_description_of_datatype()
        {
            var type = typeof(AdminAddressResponse);
            var description = new DataTypeSource().GetDescription(type);
            description.Id.ShouldEqual(type.FullName.Hash());
            description.Name.ShouldEqual("AdminAddressResponse");
            description.Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_attribute_description_of_datatype()
        {
            var type = typeof(AdminAddressRequest);
            var description = new DataTypeSource().GetDescription(type);
            description.Id.ShouldEqual(type.FullName.Hash());
            description.Name.ShouldEqual("AdminAddressRequest");
            description.Comments.ShouldEqual("This is an address request yo!");
        }

        [Test]
        public void should_return_attribute_description_of_datatype_and_xml_type_attribute()
        {
            var type = typeof(AdminUserRequest);
            var description = new DataTypeSource().GetDescription(type);
            description.Id.ShouldEqual(type.FullName.Hash());
            description.Name.ShouldEqual("User");
            description.Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_default_description_of_list_datatype()
        {
            var description = new DataTypeSource().GetDescription(typeof(List<AdminAddressResponse>));
            description.Id.ShouldEqual(typeof(AdminAddressResponse).FullName.Hash());
            description.Name.ShouldEqual("ArrayOfAdminAddressResponse");
            description.Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_attribute_description_of_inherited_list_datatype()
        {
            var description = new DataTypeSource().GetDescription(typeof(AdminAddresses));
            description.Id.ShouldEqual(typeof(AdminAddressResponse).FullName.Hash());
            description.Name.ShouldEqual("ArrayOfAdminAddressResponse");
            description.Comments.ShouldEqual("These are addresses yo!");
        }

        [Test]
        public void should_return_attribute_description_of_inherited_list_datatype_with_xml_type_attribute()
        {
            var description = new DataTypeSource().GetDescription(typeof(AdminUsers));
            description.Id.ShouldEqual(typeof(AdminUserResponse).FullName.Hash());
            description.Name.ShouldEqual("Users");
            description.Comments.ShouldEqual("These are users yo!");
        }
    }
}