using NUnit.Framework;
using Should;
using Swank.Description;
using Tests.Administration.Users;

namespace Tests.Description
{
    [TestFixture]
    public class ParameterSourceTests
    {
        [Test]
        public void should_return_default_description_of_parameter()
        {
            var action = TestBehaviorGraph.CreateAction<AdminAddressGetAllOfTypeHandler>();
            var parameterSource = new ParameterSource();
            var description = parameterSource.GetDescription(action.InputType().GetProperty("AddressType"));
            description.Name.ShouldEqual("AddressType");
            description.Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_attribute_description_of_parameter()
        {
            var action = TestBehaviorGraph.CreateAction<AdminAddressGetAllOfTypeHandler>();
            var parameterSource = new ParameterSource();
            var description = parameterSource.GetDescription(action.InputType().GetProperty("UserId"));
            description.Name.ShouldEqual("User Id");
            description.Comments.ShouldEqual("This is the id of the user.");
        }
    }
}