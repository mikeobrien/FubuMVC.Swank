using System;
using System.Reflection;
using System.Xml.Serialization;

namespace Swank.Description
{
    public class TypeSource : IDescriptionSource<Type, DataTypeDescription>
    {
        public DataTypeDescription GetDescription(Type type)
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