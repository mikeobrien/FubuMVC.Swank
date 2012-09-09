using System.Reflection;

namespace Swank.Description
{
    public class ParameterSource : IDescriptionSource<PropertyInfo, ParameterDescription>
    {
        public ParameterDescription GetDescription(PropertyInfo property)
        {
            var description = property.GetCustomAttribute<DescriptionAttribute>();
            return new ParameterDescription {
                    Name = description != null ? description.Name : property.Name,
                    Comments = description != null ? description.Comments : null,
                    Namespace = property.DeclaringType.Namespace
                };
        }
    }
}