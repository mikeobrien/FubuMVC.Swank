using System;
using System.Reflection;
using System.Xml.Serialization;
using FubuCore.Reflection;
using FubuMVC.Swank.Extensions;
using System.Runtime.Serialization;

namespace FubuMVC.Swank.Description
{
    public class TypeConvention : IDescriptionConvention<Type, TypeDescription>
    {
        public virtual TypeDescription GetDescription(Type type)
        {
            var arrayComments = type.GetAttribute<ArrayCommentsAttribute>();
            var dictionaryComments = type.GetAttribute<DictionaryCommentsAttribute>();

            return new TypeDescription {
                Name = type.GetCustomAttribute<XmlRootAttribute>().WhenNotNull(x => x.ElementName).OtherwiseDefault() ??
                    type.GetCustomAttribute<XmlTypeAttribute>().WhenNotNull(x => x.TypeName).OtherwiseDefault() ??
                    type.GetCustomAttribute<DataContractAttribute>().WhenNotNull(x => x.Name).OtherwiseDefault() ??
                    type.GetCustomAttribute<CollectionDataContractAttribute>().WhenNotNull(x => x.Name).OtherwiseDefault() ??
                    type.GetXmlName(),
                Comments = type.GetCustomAttribute<CommentsAttribute>().WhenNotNull(x => x.Comments).OtherwiseDefault(),
                ArrayItem = new Description
                {
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