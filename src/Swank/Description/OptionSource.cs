using System.Reflection;

namespace FubuMVC.Swank.Description
{
    public class OptionSource : IDescriptionSource<FieldInfo, OptionDescription>
    {
        public OptionDescription GetDescription(FieldInfo field)
        {
            var description = field.GetCustomAttribute<DescriptionAttribute>();
            return new OptionDescription {
                    Name = description != null ? description.Name : null,
                    Comments = description != null ? description.Comments : null
                };
        }
    }
}