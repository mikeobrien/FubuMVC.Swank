using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Description
{
    public class StatusCodeConvention : IDescriptionConvention<ActionCall, List<StatusCodeDescription>>
    {
        public virtual List<StatusCodeDescription> GetDescription(ActionCall action)
        {
            return action.Method.GetCustomAttributes<StatusCodeDescriptionAttribute>()
                .Concat(action.HandlerType.GetCustomAttributes<StatusCodeDescriptionAttribute>())
                 .Select(x => new StatusCodeDescription {
                    Code = x.Code,
                    Name = x.Name,
                    Comments = x.Comments
                }).OrderBy(x => x.Code).ToList();
        }
    }
}