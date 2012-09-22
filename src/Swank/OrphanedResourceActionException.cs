using System;
using System.Collections.Generic;

namespace FubuMVC.Swank
{
    public class OrphanedResourceActionException : Exception
    {
        public OrphanedResourceActionException(IEnumerable<string> actions)
            : base(string.Format("The following actions are not associated with a resource. Either assocate them with a resource or turn off orphaned action exceptions. {0}",
                    string.Join(", ", actions))) { }
    }
}