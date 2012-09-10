using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuMVC.Core.Registration.Nodes;

namespace Swank.Description
{
    public class ErrorSource : IDescriptionSource<ActionCall, List<ErrorDescription>>
    {
        public List<ErrorDescription> GetDescription(ActionCall action)
        {
            return action.Method.GetCustomAttributes<ErrorDescriptionAttribute>()
                 .Select(x => new ErrorDescription {
                    Status = x.Status,
                    Name = x.Name,
                    Comments = x.Comments,
                    Namespace = action.HandlerType.Namespace
                }).OrderBy(x => x.Status).ToList();
        }
    }
}