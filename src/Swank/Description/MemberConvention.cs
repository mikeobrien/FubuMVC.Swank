using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Description
{
    public class MemberConvention : IDescriptionConvention<PropertyInfo, MemberDescription>
    {
        public virtual MemberDescription GetDescription(PropertyInfo property)
        {
            var arrayComments = property.GetAttribute<ArrayCommentsAttribute>();
            var dictionaryComments = property.GetCustomAttribute<DictionaryCommentsAttribute>();
            var description = property.GetCustomAttribute<DescriptionAttribute>();

            return new MemberDescription {
                Name = description.WhenNotNull(x => x.Name).OtherwiseDefault() ??
                    property.GetCustomAttribute<XmlElementAttribute>().WhenNotNull(x => x.ElementName).OtherwiseDefault() ??
                    property.GetCustomAttribute<DataMemberAttribute>().WhenNotNull(x => x.Name).OtherwiseDefault() ??
                    property.Name,
                Comments = arrayComments.WhenNotNull(x => x.Comments).OtherwiseDefault() ??
                    dictionaryComments.WhenNotNull(x => x.Comments).OtherwiseDefault() ??
                    description.WhenNotNull(x => x.Comments).OtherwiseDefault() ??
                    property.GetCustomAttribute<CommentsAttribute>().WhenNotNull(x => x.Comments).OtherwiseDefault(),
                DefaultValue = property.GetCustomAttribute<DefaultValueAttribute>()
                                       .WhenNotNull(x => x.Value).OtherwiseDefault(),
                Optional = property.HasAttribute<OptionalAttribute>() && !property.PropertyType.IsNullable(),
                Hidden = property.PropertyType.HasAttribute<HideAttribute>() || 
                    property.HasAttribute<HideAttribute>() ||
                    property.HasAttribute<XmlIgnoreAttribute>(),
                ArrayItem = new Description
                {
                    Name = property.GetAttribute<XmlArrayItemAttribute>()
                                   .WhenNotNull(x => x.ElementName).OtherwiseDefault(),
                    Comments = arrayComments.WhenNotNull(x => x.ItemComments).OtherwiseDefault()
                },
                DictionaryEntry = new DictionaryDescription
                {
                    KeyComments = dictionaryComments.WhenNotNull(x => x.KeyComments).OtherwiseDefault(),
                    ValueComments = dictionaryComments.WhenNotNull(x => x.ValueComments).OtherwiseDefault()
                }
            };
        }
    }
}