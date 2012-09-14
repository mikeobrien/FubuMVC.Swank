using NUnit.Framework;
using Should;
using Swank.Description;
using Tests.Description.OptionSourceTests.Administration.Users;

namespace Tests.Description.OptionSourceTests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void should_return_default_description_of_option()
        {
            var action = Behaviors.CreateAction<AdminAddressGetAllOfTypeHandler>("/admin/users/{AddressType}", HttpVerbs.Get);
            var optionSource = new OptionSource();
            var description = optionSource.GetDescription(action.InputType().GetProperty("AddressType").PropertyType.GetField("Emergency"));
            description.Name.ShouldBeNull();
            description.Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_attribute_description_of_option()
        {
            var action = Behaviors.CreateAction<AdminAddressGetAllOfTypeHandler>("/admin/users/{AddressType}", HttpVerbs.Get);
            var optionSource = new OptionSource();
            var description = optionSource.GetDescription(action.InputType().GetProperty("AddressType").PropertyType.GetField("Work"));
            description.Name.ShouldEqual("Work Address");
            description.Comments.ShouldEqual("This is the work address of the user.");
        }
    }
}