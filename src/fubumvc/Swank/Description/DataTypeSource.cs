using System;
using System.Reflection;

namespace Swank.Description
{
    public class DataTypeSource : IDescriptionSource<Type, DataTypeDescription>
    {
        public DataTypeDescription GetDescription(Type type)
        {
            var description = type.GetCustomAttribute<DescriptionAttribute>();
            return new DataTypeDescription
            {
                    Name = type.FullName.Hash(),
                    Comments = description != null ? description.Comments : null,
                    Namespace = type.Namespace,
                    Alias = description != null ? description.Name : type.Name
                };
        }
    }
}