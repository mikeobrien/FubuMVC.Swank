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
            
            return new MemberDescription {
                Name = property.GetCustomAttribute<XmlElementAttribute>()
                            .WhenNotNull(x => x.ElementName)
                            .Otherwise(property.GetCustomAttribute<DataMemberAttribute>()
                                .WhenNotNull(x => x.Name)
                                .Otherwise(property.Name)),
                Comments = property.GetCustomAttribute<CommentsAttribute>()
                            .WhenNotNull(x => x.Comments)
                            .Otherwise(arrayComments
                                .WhenNotNull(x => x.Comments)
                                    .Otherwise(dictionaryComments
                                        .WhenNotNull(x => x.Comments)
                                        .OtherwiseDefault())),
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