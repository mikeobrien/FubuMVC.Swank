using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using System.Reflection;

namespace FubuMVC.Swank.Description
{
    public class HeaderConvention : IDescriptionConvention<BehaviorChain, List<HeaderDescription>>
    {
        public virtual List<HeaderDescription> GetDescription(BehaviorChain chain)
        {
            var action = chain.FirstCall();
            return action.Method.GetCustomAttributes<HeaderDescriptionAttribute>()
                .Concat(action.HandlerType.GetCustomAttributes<HeaderDescriptionAttribute>())
                 .Select(x => new HeaderDescription {
                    Type = x.Type,
                    Name = x.Name,
                    Comments = x.Comments,
                    Optional = x.Optional
                }).OrderBy(x => x.Type).ThenBy(x => x.Name).ToList();
        }
    }
}