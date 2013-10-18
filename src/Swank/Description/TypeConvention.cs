using System.Xml.Serialization;
using FubuMVC.Swank.Extensions;
using System.Runtime.Serialization;

namespace FubuMVC.Swank.Description
{
    public class TypeConvention : IDescriptionConvention<System.Type, TypeDescription>
    {
        public virtual TypeDescription GetDescription(System.Type type)
        {
            var elementType = type.GetListElementType();
            return new TypeDescription {
                Type = elementType ?? type,
                Name = type.GetCustomAttribute<XmlRootAttribute>()
                            .WhenNotNull(x => x.ElementName)
                            .Otherwise(type.GetCustomAttribute<XmlTypeAttribute>()
                                .WhenNotNull(x => x.TypeName)
                                .Otherwise(type.GetCustomAttribute<DataContractAttribute>() 
                                    .WhenNotNull(x => x.Name)
                                    .Otherwise(type.GetCustomAttribute<CollectionDataContractAttribute>() 
                                        .WhenNotNull(x => x.Name)
                                        .Otherwise(type.GetXmlName())))),
                Comments = type.GetCustomAttribute<CommentsAttribute>().WhenNotNull(x => x.Comments).OtherwiseDefault()
            };
        }
    }
}