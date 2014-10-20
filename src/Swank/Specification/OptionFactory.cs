using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Specification
{
    public class OptionFactory
    {
        private readonly Configuration _configuration;
        private readonly IDescriptionConvention<FieldInfo, OptionDescription> _optionConvention;

        public OptionFactory(
            Configuration configuration, 
            IDescriptionConvention<FieldInfo, OptionDescription> optionConvention)
        {
            _configuration = configuration;
            _optionConvention = optionConvention;
        }

        public List<EnumOption> BuildOptions(Type type)
        {
            type = type.GetNullableUnderlyingType();
            return !type.IsEnum ? new List<EnumOption>() :
                type.GetEnumOptions()
                    .Select(x => new
                    {
                        Option = x,
                        Description = _optionConvention.GetDescription(x)
                    })
                    .Where(x => !x.Description.Hidden)
                    .Select(x =>
                        _configuration.OptionOverrides.Apply(x.Option, new EnumOption
                        {
                            Name = x.Description.WhenNotNull(y => y.Name).OtherwiseDefault(),
                            Comments = x.Description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                            Value = _configuration.EnumFormat == EnumFormat.AsString ? x.Option.Name :
                                        x.Option.GetRawConstantValue().ToString()
                        }))
                    .OrderBy(x => x.Name).ToList();
        }
    }
}