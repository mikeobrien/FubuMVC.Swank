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
            var dataTypeSource = new DataTypeSource();
            var description = dataTypeSource.GetDescription(type);
            description.Name.ShouldEqual(type.FullName.Hash());
            description.Comments.ShouldBeNull();
            description.Alias.ShouldEqual("AdminAddressResponse");
        }

        [Test]
        public void should_return_attribute_description_of_datatype()
        {
            var type = typeof(AdminAddressRequest);
            var dataTypeSource = new DataTypeSource();
            var description = dataTypeSource.GetDescription(type);
            description.Name.ShouldEqual(type.FullName.Hash());
            description.Comments.ShouldEqual("This is an address request yo!");
            description.Alias.ShouldEqual("AddressRequest");
        }

        [Test]
        public void should_return_default_description_of_list_datatype()
        {
            var dataTypeSource = new DataTypeSource();
            var description = dataTypeSource.GetDescription(typeof(List<AdminAddressResponse>));
            description.Name.ShouldEqual(typeof(AdminAddressResponse).FullName.Hash());
            description.Comments.ShouldBeNull();
            description.Alias.ShouldEqual("AdminAddressResponse");
        }

        [Test]
        public void should_return_attribute_description_of_inherited_list_datatype()
        {
            var dataTypeSource = new DataTypeSource();
            var description = dataTypeSource.GetDescription(typeof(AdminAddresses));
            description.Name.ShouldEqual(typeof(AdminAddressResponse).FullName.Hash());
            description.Comments.ShouldEqual("These are addresses yo!");
            description.Alias.ShouldEqual("Addresses");
        }
    }
}