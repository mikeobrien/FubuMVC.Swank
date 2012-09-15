using System;

namespace Swank.Description
{
    public class ModuleDescription : Description { }

    public class ResourceDescription<THandler> : ResourceDescription { }
    public class ResourceDescription : Description
    {
        public Type Handler { get; set; }
    }

    public class EndpointDescription : Description { }

    public class ParameterDescription : Description
    {
        public object DefaultValue { get; set; }
    }

    public class OptionDescription : Description { }

    public class ErrorDescription : Description
    {
        public int Status { get; set; }
    }

    public class DataTypeDescription : Description
    {
        public Type Type { get; set; }
    }
}