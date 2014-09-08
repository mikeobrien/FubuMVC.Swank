using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Registration.Nodes;
using System.Reflection;

namespace FubuMVC.Swank.Description
{
    public class MimeTypeConvention : IDescriptionConvention<BehaviorChain, List<MimeTypeDescription>>
    {
        public virtual List<MimeTypeDescription> GetDescription(BehaviorChain chain)
        {
            var action = chain.FirstCall();
            return action.Method.GetCustomAttributes<MimeTypeAttribute>()
                .Concat(action.HandlerType.GetCustomAttributes<MimeTypeAttribute>())
                 .Select(x => new MimeTypeDescription {
                    Direction = x.Type,
                    Name = x.Name
                }).OrderBy(x => x.Direction).ThenBy(x => x.Name).ToList();
        }
    }
}