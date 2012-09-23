using System;
using System.Collections.Generic;

namespace FubuMVC.Swank.Specification
{
    public class OrphanedModuleActionException : Exception
    {
        public OrphanedModuleActionException(IEnumerable<string> actions)
            : base(string.Format("The following actions are not associated with a module. Either assocate them with a module or turn off orphaned action exceptions. {0}",
                    string.Join(", ", actions))) { }
    }
}