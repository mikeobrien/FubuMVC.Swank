using System;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Description
{
    public class EndpointConvention : IDescriptionConvention<ActionCall, EndpointDescription>
    {
        public EndpointDescription GetDescription(ActionCall action)
        {
            var attribute = action.Method.GetCustomAttribute<DescriptionAttribute>() ??
                            action.HandlerType.GetCustomAttribute<DescriptionAttribute>();

            return new EndpointDescription {
                    Name = attribute != null ? attribute.Name : null,
                    Comments = GetComments<DescriptionAttribute>(action, x => x.Comments),
                    RequestComments = GetComments<RequestCommentsAttribute>(action, x => x.Comments, ".Request"),
                    ResponseComments = GetComments<ResponseCommentsAttribute>(action, x => x.Comments, ".Response")
                };
        }

        private static string GetComments<TAttribute>(ActionCall action, Func<TAttribute, string> attributeComments, string resourcePostfix = "") 
            where TAttribute : Attribute
        {
            string comments = null;
            var attribute = action.Method.GetCustomAttribute<TAttribute>() ??
                            action.HandlerType.GetCustomAttribute<TAttribute>();
            if (attribute != null) comments = attributeComments(attribute);

            if (comments.IsEmpty())
            {
                comments = action.HandlerType.Assembly.FindTextResourceNamed(
                                action.HandlerType.FullName + "." + action.Method.Name + resourcePostfix) ??
                            (!action.HandlerType.HasAttribute<ResourceAttribute>() ? 
                                action.HandlerType.Assembly.FindTextResourceNamed(action.HandlerType.FullName + resourcePostfix) : null);
            }
            return comments;
        }
    }
}