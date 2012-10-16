using FubuMVC.Swank.Description;
using NUnit.Framework;
using Should;

namespace Tests.Description
{
    [TestFixture]
    public class OptionConventionTests
    {
        private OptionConvention _optionConvention;

        [SetUp]
        public void Setup()
        {
            _optionConvention = new OptionConvention();
        }

        public enum Options
        {
            [FubuMVC.Swank.Description.Description("Option 2", "This is option 2.")]
            Option2,
            Option1,
            [Comments("This is option 3!")]
            Option3
        }

        [Test]
        public void should_return_default_description_of_option()
        {
            var description = _optionConvention.GetDescription(typeof(Options).GetField("Option1"));
            description.Name.ShouldEqual("Option1");
            description.Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_attribute_description_of_option()
        {
            var description = _optionConvention.GetDescription(typeof(Options).GetField("Option2"));
            description.Name.ShouldEqual("Option 2");
            description.Comments.ShouldEqual("This is option 2.");
        }

        [Test]
        public void should_return_attribute_comments_of_option()
        {
            var description = _optionConvention.GetDescription(typeof(Options).GetField("Option3"));
            description.Name.ShouldEqual("Option3");
            description.Comments.ShouldEqual("This is option 3!");
        }
    }
}