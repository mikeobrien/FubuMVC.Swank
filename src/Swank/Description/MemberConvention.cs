using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using FubuCore.Reflection;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Description
{
    public class MemberConvention : IDescriptionConvention<PropertyInfo, MemberDescription>
    {
        public MemberDescription GetDescription(PropertyInfo property)
        {
            return new MemberDescription {
                Name = property.GetCustomAttribute<XmlElementAttribute>()
                            .WhenNotNull(x => x.ElementName)
                            .Otherwise(property.GetCustomAttribute<DataMemberAttribute>()
                                .WhenNotNull(x => x.Name)
                                .Otherwise(property.Name)),
                Comments = property.GetCustomAttribute<CommentsAttribute>().WhenNotNull(x => x.Comments).OtherwiseDefault(),
                DefaultValue = property.GetCustomAttribute<DefaultValueAttribute>().WhenNotNull(x => x.Value).OtherwiseDefault(),
                Required = property.HasAttribute<RequiredAttribute>(),
                Type = property.PropertyType.GetListElementType() ?? property.PropertyType,
                ArrayItemName = property.GetAttribute<XmlArrayItemAttribute>().WhenNotNull(x => x.ElementName).OtherwiseDefault()
            };
        }
    }
}