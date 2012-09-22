using System.Reflection;
using System.Xml.Serialization;
using FubuCore.Reflection;

namespace FubuMVC.Swank.Description
{
    public class MemberSource : IDescriptionSource<PropertyInfo, MemberDescription>
    {
        public MemberDescription GetDescription(PropertyInfo property)
        {
            return new MemberDescription {
                    Name = property.GetCustomAttribute<XmlElementAttribute>().WhenNotNull(x => x.ElementName, property.Name),
                    Comments = property.GetCustomAttribute<CommentsAttribute>().WhenNotNull(x => x.Comments),
                    DefaultValue = property.GetCustomAttribute<DefaultValueAttribute>().WhenNotNull(x => x.Value),
                    Required = property.HasAttribute<RequiredAttribute>(),
                    Type = property.PropertyType.GetListElementType() ?? property.PropertyType
                };
        }
    }
}