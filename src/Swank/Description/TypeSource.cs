using System.Xml.Serialization;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Description
{
    public class TypeSource : IDescriptionSource<System.Type, DataTypeDescription>
    {
        public DataTypeDescription GetDescription(System.Type type)
        {
            var elementType = type.GetListElementType();
            return new DataTypeDescription {
                Type = elementType ?? type,
                Name = type.GetCustomAttribute<XmlTypeAttribute>().WhenNotNull(x => x.TypeName)
                            .Otherwise(elementType.WhenNotNull(x => "ArrayOf" + x.Name).Otherwise(type.Name)),
                Comments = type.GetCustomAttribute<CommentsAttribute>().WhenNotNull(x => x.Comments).OtherwiseDefault()
            };
        }
    }
}