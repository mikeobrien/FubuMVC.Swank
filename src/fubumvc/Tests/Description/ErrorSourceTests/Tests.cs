using FubuMVC.Core.Registration;
using NUnit.Framework;
using Should;
using Swank.Description;
using Tests.Description.ErrorSourceTests.Administration.Users;

namespace Tests.Description.ErrorSourceTests
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
        public void should_return_errors()
        {
            var action = _graph.GetAction<AdminAddressAllGetHandler>();
            var errorSource = new ErrorSource();
            var errorDescriptions = errorSource.GetDescription(action);
            errorDescriptions.Count.ShouldEqual(2);

            var error = errorDescriptions[0];
            error.Status.ShouldEqual(410);
            error.Name.ShouldEqual("Invalid address");
            error.Comments.ShouldEqual("An invalid address was entered fool!");
            
            error = errorDescriptions[1];
            error.Status.ShouldEqual(411);
            error.Name.ShouldEqual("Swank address");
            error.Comments.ShouldBeNull();
        }
    }
}