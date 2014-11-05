using System;
using System.Reflection;
using System.Xml.Serialization;
using FubuCore.Reflection;
using FubuMVC.Swank.Extensions;
using System.Runtime.Serialization;

namespace FubuMVC.Swank.Description
{
    public class EnumConvention : IDescriptionConvention<Type, EnumDescription>
    {
        public virtual EnumDescription GetDescription(Type type)
        {
            var description = type.GetAttribute<DescriptionAttribute>();

            return new EnumDescription
            {
                Name = type.GetCustomAttribute<XmlRootAttribute>().WhenNotNull(x => x.ElementName).OtherwiseDefault() ??
                    type.GetCustomAttribute<XmlTypeAttribute>().WhenNotNull(x => x.TypeName).OtherwiseDefault() ??
                    type.GetCustomAttribute<DataContractAttribute>().WhenNotNull(x => x.Name).OtherwiseDefault() ??
                    description.WhenNotNull(x => x.Name).OtherwiseDefault() ??
                    type.Name,
                Comments = type.GetCustomAttribute<CommentsAttribute>().WhenNotNull(x => x.Comments).OtherwiseDefault() ??
                    description.WhenNotNull(x => x.Comments).OtherwiseDefault()
            };
        }
    }
}