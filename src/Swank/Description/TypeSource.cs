using System.Xml.Serialization;

namespace FubuMVC.Swank.Description
{
    public class TypeSource : IDescriptionSource<System.Type, DataTypeDescription>
    {
        public DataTypeDescription GetDescription(System.Type type)
        {
            var description = type.GetCustomAttribute<CommentsAttribute>();
            var xmlType = type.GetCustomAttribute<XmlTypeAttribute>();
            var elementType = type.GetListElementType();
            return new DataTypeDescription {
                Type = elementType ?? type,
                Name = xmlType != null ? xmlType.TypeName : elementType != null ? "ArrayOf" + elementType.Name : type.Name,
                Comments = description != null ? description.Comments : null
            };
        }
    }
}