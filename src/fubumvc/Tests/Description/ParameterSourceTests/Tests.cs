using FubuMVC.Core.Registration;
using NUnit.Framework;
using Should;
using Swank.Description;
using Tests.Description.ParameterSourceTests.Administration.Users;

namespace Tests.Description.ParameterSourceTests
{
    [TestFixture]
    public class Tests
    {
        private BehaviorGraph _graph;

        [SetUp]
        public void Setup()
        {
            _graph = Behaviors.BuildGraph().AddActionsInThisNamespace();
        }

        [Test]
        public void should_return_default_description_of_parameter()
        {
            var action = _graph.GetAction<AdminAddressAllOfTypeGetHandler>();
            var parameterSource = new ParameterSource();
            var description = parameterSource.GetDescription(action.InputType().GetProperty("AddressType"));
            description.Name.ShouldEqual("AddressType");
            description.Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_attribute_description_of_parameter()
        {
            var action = _graph.GetAction<AdminAddressAllOfTypeGetHandler>();
            var parameterSource = new ParameterSource();
            var description = parameterSource.GetDescription(action.InputType().GetProperty("UserId"));
            description.Name.ShouldEqual("UserId");
            description.Comments.ShouldEqual("This is the id of the user.");
        }
    }
}