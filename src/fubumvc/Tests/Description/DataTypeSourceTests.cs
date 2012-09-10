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
    }
}