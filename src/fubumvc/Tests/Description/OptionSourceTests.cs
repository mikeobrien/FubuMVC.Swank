using NUnit.Framework;
using Should;
using Swank.Description;
using Tests.Administration.Users;

namespace Tests.Description
{
    [TestFixture]
    public class OptionSourceTests
    {
        [Test]
        public void should_return_default_description_of_option()
        {
            var action = TestBehaviorGraph.CreateAction<AdminAddressGetAllOfTypeHandler>();
            var optionSource = new OptionSource();
            var description = optionSource.GetDescription(action.InputType().GetProperty("AddressType").PropertyType.GetField("Emergency"));
            description.Name.ShouldBeNull();
            description.Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_attribute_description_of_option()
        {
            var action = TestBehaviorGraph.CreateAction<AdminAddressGetAllOfTypeHandler>();
            var optionSource = new OptionSource();
            var description = optionSource.GetDescription(action.InputType().GetProperty("AddressType").PropertyType.GetField("Work"));
            description.Name.ShouldEqual("Work Address");
            description.Comments.ShouldEqual("This is the work address of the user.");
        }
    }
}