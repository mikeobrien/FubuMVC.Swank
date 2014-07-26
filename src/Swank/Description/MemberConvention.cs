using System;
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
            var arrayDescription = property.GetAttribute<ArrayDescriptionAttribute>();
            var dictionaryDescription = property.GetAttribute<DictionaryDescriptionAttribute>();
            var description = property.GetAttribute<DescriptionAttribute>();
            var obsolete = property.GetAttribute<ObsoleteAttribute>();

            return new MemberDescription {
                Name = description.WhenNotNull(x => x.Name).OtherwiseDefault() ??
                    property.GetCustomAttribute<XmlElementAttribute>().WhenNotNull(x => x.ElementName).OtherwiseDefault() ??
                    property.GetCustomAttribute<DataMemberAttribute>().WhenNotNull(x => x.Name).OtherwiseDefault() ??
                    property.Name,
                Comments = description.WhenNotNull(x => x.Comments).OtherwiseDefault() ??
                    property.GetCustomAttribute<CommentsAttribute>().WhenNotNull(x => x.Comments).OtherwiseDefault(),
                DefaultValue = property.GetCustomAttribute<DefaultValueAttribute>().WhenNotNull(x => x.Value).OtherwiseDefault(),
                SampleValue = property.GetCustomAttribute<SampleValueAttribute>().WhenNotNull(x => x.Value).OtherwiseDefault(),
                Optional = property.HasAttribute<OptionalAttribute>() || property.PropertyType.IsNullable(),
                Hidden = property.PropertyType.HasAttribute<HideAttribute>() || 
                    property.HasAttribute<HideAttribute>() ||
                    property.HasAttribute<XmlIgnoreAttribute>(),
                Deprecated = obsolete != null,
                DeprecationMessage = obsolete.WhenNotNull(x => x.Message).OtherwiseDefault(),
                ArrayItem = new Description
                {
                    Name = property.GetAttribute<XmlArrayItemAttribute>().WhenNotNull(x => x.ElementName).OtherwiseDefault() ??
                           arrayDescription.WhenNotNull(x => x.ItemName).OtherwiseDefault(),
                    Comments = arrayDescription.WhenNotNull(x => x.ItemComments).OtherwiseDefault()
                },
                DictionaryEntry = new DictionaryDescription
                {
                    KeyName = dictionaryDescription.WhenNotNull(x => x.KeyName).OtherwiseDefault(),
                    KeyComments = dictionaryDescription.WhenNotNull(x => x.KeyComments).OtherwiseDefault(),
                    ValueComments = dictionaryDescription.WhenNotNull(x => x.ValueComments).OtherwiseDefault()
                }
            };
        }
    }
}