using System;
using FubuMVC.Core.Registration.Nodes;

namespace Swank
{
    public class ResourceSourceConfig
    {
        public ResourceSourceConfig()
        {
            Grouping = x => x.ParentChain().Route.GetRouteResource();
        }

        public Func<ActionCall, object> Grouping { get; private set; } 

        public ResourceSourceConfig GroupBy(Func<ActionCall, object> grouping)
        {
            Grouping = grouping;
            return this;
        }
    }
}