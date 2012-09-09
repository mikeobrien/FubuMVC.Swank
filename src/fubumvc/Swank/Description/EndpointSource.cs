using System.Reflection;
using FubuMVC.Core.Registration.Nodes;

namespace Swank.Description
{
    public class EndpointSource : IDescriptionSource<ActionCall, EndpointDescription>
    {
        public EndpointDescription GetDescription(ActionCall action)
        {
            string name = null;
            string comments;
            var attribute = action.Method.GetCustomAttribute<DescriptionAttribute>() ??
                            action.HandlerType.GetCustomAttribute<DescriptionAttribute>();
            if (attribute != null)
            {
                name = attribute.Name;
                comments = attribute.Comments;
            }
            else comments = action.HandlerType.Assembly.FindTextResourceNamed(action.HandlerType.FullName + "." + action.Method.Name) ??
                            action.HandlerType.Assembly.FindTextResourceNamed(action.HandlerType.FullName);
            return new EndpointDescription {
                    Name = name,
                    Comments = comments,
                    Namespace = action.HandlerType.Namespace
                };
        }
    }
}