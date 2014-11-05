using System;
using System.Linq;
using System.Reflection;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Specification
{
    public class OptionFactory
    {
        private readonly Configuration _configuration;
        private readonly IDescriptionConvention<Type, EnumDescription> _enumConvention;
        private readonly IDescriptionConvention<FieldInfo, EnumOptionDescription> _optionConvention;

        public OptionFactory(
            Configuration configuration,
            IDescriptionConvention<Type, EnumDescription> enumConvention,
            IDescriptionConvention<FieldInfo, EnumOptionDescription> optionConvention)
        {
            _configuration = configuration;
            _enumConvention = enumConvention;
            _optionConvention = optionConvention;
        }

        public Enumeration BuildOptions(Type type)
        {
            type = type.GetNullableUnderlyingType();
            if (!type.IsEnum) return null;
            var description = _enumConvention.GetDescription(type);
            return new Enumeration
            {
                Name = description.WhenNotNull(y => y.Name).Otherwise(type.Name),
                Comments = description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                Options = type.GetEnumOptions()
                    .Select(x => new
                    {
                        Option = x,
                        Description = _optionConvention.GetDescription(x)
                    })
                    .Where(x => !x.Description.Hidden)
                    .Select(x =>
                        _configuration.OptionOverrides.Apply(x.Option, new Option
                        {
                            Name = x.Description.WhenNotNull(y => y.Name).OtherwiseDefault(),
                            Comments = x.Description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                            Value = _configuration.EnumFormat == EnumFormat.AsString
                                ? x.Option.Name
                                : x.Option.GetRawConstantValue().ToString()
                        }))
                    .OrderBy(x => x.Name).ToList()
            };
        }
    }
}