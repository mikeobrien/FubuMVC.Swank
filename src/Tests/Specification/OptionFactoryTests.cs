using System;
using System.Linq;
using FubuMVC.Swank;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Specification;
using NUnit.Framework;
using Should;
using DescriptionAttribute = FubuMVC.Swank.Description.DescriptionAttribute;

namespace Tests.Specification
{
    [TestFixture]
    public class OptionFactoryTests
    {
        public Enumeration GetOptions<T>(Action<Configuration> configure = null)
        {
            return GetOptions(typeof (T), configure);
        }

        public Enumeration GetOptions(Type type, Action<Configuration> configure = null)
        {
            var configuration = new Configuration();
            if (configure != null) configure(configuration);
            return new OptionFactory(configuration, 
                new EnumConvention(), 
                new OptionConvention()).BuildOptions(type);
        }

        public enum EnumWithoutComments { }

        [Test]
        public void should_return_default_name_and_comments()
        {
            var options = GetOptions(typeof(EnumWithoutComments));
            options.Name.ShouldEqual("EnumWithoutComments");
            options.Comments.ShouldBeNull();
        }

        [Description("SomeName", "Some comments")]
        public enum EnumWithComments { }

        [Test]
        public void should_return_name_and_comments()
        {
            var options = GetOptions(typeof(EnumWithComments));
            options.Name.ShouldEqual("SomeName");
            options.Comments.ShouldEqual("Some comments");
        }

        [Test]
        public void should_return_empty_when_not_an_enum(
            [Values(typeof(int), typeof(int?))] Type type)
        {
            GetOptions(type).ShouldBeNull();
        }

        public enum EnumOrder
        {
            Option3, Option1, Option2 
        }

        [Test]
        public void should_return_options_in_alphanumeric_order(
            [Values(typeof(EnumOrder), typeof(EnumOrder?))]Type type)
        {
            var options = GetOptions(type).Options;

            options.Count.ShouldEqual(3);

            options[0].Name.ShouldEqual("Option1");
            options[1].Name.ShouldEqual("Option2");
            options[2].Name.ShouldEqual("Option3");
        }

        public enum HiddenEnum
        {
            Option3, [Hide] Option1, Option2
        }

        [Test]
        public void should_not_return_hidden_enum_options()
        {
            var options = GetOptions<HiddenEnum>().Options;

            options.Count.ShouldEqual(2);

            options[0].Name.ShouldEqual("Option2");
            options[1].Name.ShouldEqual("Option3");
        }

        public enum EnumOverride
        {
            [FubuMVC.Swank.Description.Description("Name", "Comments")]
            Option
        }

        [Test]
        public void should_override()
        {
            var options = GetOptions<EnumOverride>(
                x => x.OptionOverrides.Add((f, o) =>
                    {
                        o.Name += f.Name;
                        o.Value += f.Name;
                        o.Comments += f.Name;
                    })).Options;

            var option = options.Single();

            option.Name.ShouldEqual("NameOption");
            option.Value.ShouldEqual("0Option");
            option.Comments.ShouldEqual("CommentsOption");
        }

        public enum EnumDescription
        {
            [FubuMVC.Swank.Description.Description("Name", "Comments")]
            Option
        }

        [Test]
        [TestCase(EnumFormat.AsNumber, "0")]
        [TestCase(EnumFormat.AsString, "Option")]
        public void should_return_option_description(EnumFormat enumMode, string value)
        {
            var options = GetOptions<EnumDescription>(x => x.EnumFormat = enumMode).Options;

            var option = options.Single();

            option.Name.ShouldEqual("Name");
            option.Value.ShouldEqual(value);
            option.Comments.ShouldEqual("Comments");
        }

        public enum EnumEmptyDescription
        {
            Option
        }

        [Test]
        [TestCase(EnumFormat.AsNumber, "0")]
        [TestCase(EnumFormat.AsString, "Option")]
        public void should_return_option_without_description(EnumFormat enumMode, string value)
        {
            var options = GetOptions<EnumEmptyDescription>(x => x.EnumFormat = enumMode).Options;

            var option = options.Single();

            option.Name.ShouldEqual("Option");
            option.Value.ShouldEqual(value);
            option.Comments.ShouldBeNull();
        }
    }
}
