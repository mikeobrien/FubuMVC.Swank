using System;
using FubuMVC.Core.Registration.Nodes;

namespace Swank.Description
{
    public class EndpointSource : IDescriptionSource<ActionCall, EndpointDescription>
    {
        public EndpointDescription GetDescription(ActionCall action)
        {
            throw new NotImplementedException();
        }
    }
}