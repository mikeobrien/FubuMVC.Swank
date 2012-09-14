using NUnit.Framework;
using Should;
using Swank.Description;
using Tests.Description.ParameterSourceTests.Administration.Users;

namespace Tests.Description.ParameterSourceTests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void should_return_default_description_of_parameter()
        {
            var action = Behaviors.CreateAction<AdminAddressGetAllOfTypeHandler>("/admin/users/{AddressType}", HttpVerbs.Get);
            var parameterSource = new ParameterSource();
            var description = parameterSource.GetDescription(action.InputType().GetProperty("AddressType"));
            description.Name.ShouldEqual("AddressType");
            description.Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_attribute_description_of_parameter()
        {
            var action = Behaviors.CreateAction<AdminAddressGetAllOfTypeHandler>("/admin/users/{AddressType}", HttpVerbs.Get);
            var parameterSource = new ParameterSource();
            var description = parameterSource.GetDescription(action.InputType().GetProperty("UserId"));
            description.Name.ShouldEqual("UserId");
            description.Comments.ShouldEqual("This is the id of the user.");
        }
    }
}