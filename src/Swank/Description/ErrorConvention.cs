using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Description
{
    public class ErrorConvention : IDescriptionConvention<ActionCall, List<ErrorDescription>>
    {
        public List<ErrorDescription> GetDescription(ActionCall action)
        {
            return action.Method.GetCustomAttributes<ErrorDescriptionAttribute>()
                .Concat(action.HandlerType.GetCustomAttributes<ErrorDescriptionAttribute>())
                 .Select(x => new ErrorDescription {
                    Status = x.Status,
                    Name = x.Name,
                    Comments = x.Comments
                }).OrderBy(x => x.Status).ToList();
        }
    }
}