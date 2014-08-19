using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
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

        public List<Option> BuildOptions(Type type)
        {
            return !type.IsEnum || (type.IsNullable() && !Nullable.GetUnderlyingType(type).IsEnum) ? new List<Option>() :
                type.GetEnumOptions()
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
                            Value = _configuration.EnumValue == EnumValue.AsString ? x.Option.Name :
                                        x.Option.GetRawConstantValue().ToString()
                        }))
                    .OrderBy(x => x.Name).ToList();
        }
    }
}