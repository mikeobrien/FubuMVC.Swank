using System.Reflection;
using System.Xml.Serialization;
using FubuMVC.Swank.Extensions;
using System.Runtime.Serialization;

namespace FubuMVC.Swank.Description
{
    public class TypeConvention : IDescriptionConvention<System.Type, TypeDescription>
    {
        public virtual TypeDescription GetDescription(System.Type type)
        {
            return new TypeDescription {
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