using System.Collections.Generic;

namespace Swank
{        
    public class ResourceRequest
    {
        public string ResourceName { get; set; }
    } 

    public class ResourceHandler
    {
        public Resources Execute(ResourceRequest request)
        {
            return null;
        }
    }
}