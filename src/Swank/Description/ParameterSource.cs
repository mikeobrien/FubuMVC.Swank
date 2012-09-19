using System.Reflection;

namespace FubuMVC.Swank.Description
{
    public class ParameterSource : IDescriptionSource<PropertyInfo, ParameterDescription>
    {
        public ParameterDescription GetDescription(PropertyInfo property)
        {
            var description = property.GetCustomAttribute<CommentsAttribute>();
            var defaultValue = property.GetCustomAttribute<DefaultValueAttribute>();
            return new ParameterDescription {
                    Name = property.Name,
                    Comments = description != null ? description.Comments : null,
                    DefaultValue = defaultValue != null ? defaultValue.Value : null
                };
        }
    }
}