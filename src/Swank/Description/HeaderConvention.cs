using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Description
{
    public class HeaderConvention : IDescriptionConvention<ActionCall, List<HeaderDescription>>
    {
        public List<HeaderDescription> GetDescription(ActionCall action)
        {
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