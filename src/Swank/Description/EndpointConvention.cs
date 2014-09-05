using System;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Description
{
    public class EndpointConvention : IDescriptionConvention<BehaviorChain, EndpointDescription>
    {
        public virtual EndpointDescription GetDescription(BehaviorChain chain)
        {
            var action = chain.FirstCall();
            var attribute = action.Method.GetCustomAttribute<DescriptionAttribute>() ??
                            action.HandlerType.GetCustomAttribute<DescriptionAttribute>();

            return new EndpointDescription {
                    Name = attribute != null ? attribute.Name : null,
                    Comments = GetEndpointComments(chain),
                    RequestComments = GetDataComments<RequestCommentsAttribute>(action, x => x.Comments, "Request"),
                    ResponseComments = GetDataComments<ResponseCommentsAttribute>(action, x => x.Comments, "Response")
                };
        }

        private static string GetEndpointComments(BehaviorChain chain)
        {
            var action = chain.FirstCall();
            var comments = (action.Method.GetCustomAttribute<DescriptionAttribute>() ??
                                action.HandlerType.GetCustomAttribute<DescriptionAttribute>()).WhenNotNull(x => x.Comments)
                            .Otherwise((action.Method.GetCustomAttribute<CommentsAttribute>() ??
                                action.HandlerType.GetCustomAttribute<CommentsAttribute>()).WhenNotNull(x => x.Comments)
                            .OtherwiseDefault());

            if (comments.IsEmpty())
            {
                comments = action.HandlerType.Assembly.FindTextResourceNamed(action.HandlerType.FullName + "." + 
                    action.GetRouteParameters().Join(x => x.Name, "."));
                if (comments == null && !action.HandlerType.HasAttribute<ResourceAttribute>()) 
                    comments = action.HandlerType.Assembly.FindTextResourceNamed(action.HandlerType.FullName);
            }
            return comments;
        }

        private static string GetDataComments<TAttribute>(ActionCall action, Func<TAttribute, string> attributeComments, string resourcePostfix = "") 
            where TAttribute : Attribute
        {
            var comments = (action.Method.GetCustomAttribute<TAttribute>() ??
                                action.HandlerType.GetCustomAttribute<TAttribute>()).WhenNotNull(attributeComments)
                            .OtherwiseDefault();
            if (comments.IsEmpty())
                comments = action.HandlerType.Assembly.FindTextResourceNamed(action.HandlerType.FullName + "." + action.Method.Name + "." + resourcePostfix) ??
                           action.HandlerType.Assembly.FindTextResourceNamed(action.HandlerType.FullName + "." + resourcePostfix);
            return comments;
        }
    }
}