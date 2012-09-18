using System.Reflection;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;

namespace Swank.Description
{
    public class EndpointSource : IDescriptionSource<ActionCall, EndpointDescription>
    {
        public EndpointDescription GetDescription(ActionCall action)
        {
            string name = null;
            string comments = null;
            var attribute = action.Method.GetCustomAttribute<DescriptionAttribute>() ??
                            action.HandlerType.GetCustomAttribute<DescriptionAttribute>();
            if (attribute != null)
            {
                name = attribute.Name;
                comments = attribute.Comments;
            }
            else comments = action.HandlerType.Assembly.FindTextResourceNamed(action.HandlerType.FullName + "." + action.Method.Name) ??
                (!action.HandlerType.HasAttribute<ResourceAttribute>() ? action.HandlerType.Assembly.FindTextResourceNamed(action.HandlerType.FullName) : null);
            return new EndpointDescription {
                    Name = name,
                    Comments = comments
                };
        }
    }
}