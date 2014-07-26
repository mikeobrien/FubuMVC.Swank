using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using System.Reflection;

namespace FubuMVC.Swank.Description
{
    public class StatusCodeConvention : IDescriptionConvention<BehaviorChain, List<StatusCodeDescription>>
    {
        public virtual List<StatusCodeDescription> GetDescription(BehaviorChain chain)
        {
            var action = chain.FirstCall();
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