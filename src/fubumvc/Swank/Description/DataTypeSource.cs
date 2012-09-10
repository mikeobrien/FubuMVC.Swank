using System;
using System.Reflection;

namespace Swank.Description
{
    public class DataTypeSource : IDescriptionSource<Type, DataTypeDescription>
    {
        public DataTypeDescription GetDescription(Type type)
        {
            var description = type.GetCustomAttribute<DescriptionAttribute>();
            var elementType = type.GetElementTypeOrDefault();
            return new DataTypeDescription {
                Name = elementType.FullName.Hash(),
                Comments = description != null ? description.Comments : null,
                Namespace = type.Namespace,
                Alias = description != null ? description.Name : elementType.Name
            };
        }
    }
}