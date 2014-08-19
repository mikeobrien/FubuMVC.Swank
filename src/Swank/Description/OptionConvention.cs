using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Description
{
    public class OptionConvention : IDescriptionConvention<FieldInfo, OptionDescription>
    {
        public virtual OptionDescription GetDescription(FieldInfo field)
        {
            var description = field.GetCustomAttribute<DescriptionAttribute>();
            return new OptionDescription {
                    Name = description.WhenNotNull(x => x.Name).Otherwise(field.Name),
                    Comments = description.WhenNotNull(x => x.Comments)
                        .Otherwise(field.GetCustomAttribute<CommentsAttribute>()
                                        .WhenNotNull(x => x.Comments).OtherwiseDefault()),
                    Hidden = field.HasAttribute<HideAttribute>()
                };
        }
    }
}