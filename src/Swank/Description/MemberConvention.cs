using System;
using System.Collections.Generic;
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
            var isArray = property.PropertyType.IsArray || property.PropertyType.IsList();
            var isDictionary = property.PropertyType.IsGenericDictionary();
            var dictionaryTypes = isDictionary ? property.PropertyType.GetGenericDictionaryTypes() : new KeyValuePair<Type, Type>();
            return new MemberDescription {
                Name = property.GetCustomAttribute<XmlElementAttribute>()
                            .WhenNotNull(x => x.ElementName)
                            .Otherwise(property.GetCustomAttribute<DataMemberAttribute>()
                                .WhenNotNull(x => x.Name)
                                .Otherwise(property.Name)),
                Comments = property.GetCustomAttribute<CommentsAttribute>().WhenNotNull(x => x.Comments).OtherwiseDefault(),
                DefaultValue = property.GetCustomAttribute<DefaultValueAttribute>().WhenNotNull(x => x.Value).OtherwiseDefault(),
                Required = !property.HasAttribute<OptionalAttribute>() && !property.PropertyType.IsNullable(),
                Type = isDictionary ? dictionaryTypes.Value :
                    (isArray ? property.PropertyType.GetListElementType() : property.PropertyType),
                IsArray = isArray,
                ArrayItemName = property.GetAttribute<XmlArrayItemAttribute>().WhenNotNull(x => x.ElementName).OtherwiseDefault(),
                IsDictionary = isDictionary,
                DictionaryKeyType = isDictionary ? dictionaryTypes.Key : null
            };
        }
    }
}