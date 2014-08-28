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
        private readonly Configuration _configuration;

        public TypeConvention(Configuration configuration)
        {
            _configuration = configuration;
        }

        public virtual TypeDescription GetDescription(Type type)
        {
            var arrayComments = type.GetAttribute<ArrayCommentsAttribute>();
            var dictionaryDescription = type.GetAttribute<DictionaryDescriptionAttribute>();

            return new TypeDescription {
                Name = type.GetCustomAttribute<XmlRootAttribute>().WhenNotNull(x => x.ElementName).OtherwiseDefault() ??
                    type.GetCustomAttribute<XmlTypeAttribute>().WhenNotNull(x => x.TypeName).OtherwiseDefault() ??
                    type.GetCustomAttribute<DataContractAttribute>().WhenNotNull(x => x.Name).OtherwiseDefault() ??
                    type.GetCustomAttribute<CollectionDataContractAttribute>().WhenNotNull(x => x.Name).OtherwiseDefault() ??
                    dictionaryDescription.WhenNotNull(x => x.Name).OtherwiseDefault() ??
                    type.GetXmlName(_configuration.EnumValue == EnumValue.AsString),
                Comments = type.GetCustomAttribute<CommentsAttribute>().WhenNotNull(x => x.Comments).OtherwiseDefault() ??
                    dictionaryDescription.WhenNotNull(x => x.Comments).OtherwiseDefault(),
                ArrayItem = new Description
                {
                    Comments = arrayComments.WhenNotNull(x => x.ItemComments).OtherwiseDefault()
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