namespace FubuMVC.Swank.Description
{
    public class ModuleDescription : DescriptionBase { }

    public class ResourceDescription<THandler> : ResourceDescription { }
    public class ResourceDescription : DescriptionBase
    {
        public System.Type Handler { get; set; }
    }

    public class EndpointDescription : DescriptionBase { }

    public class ParameterDescription : DescriptionBase
    {
        public object DefaultValue { get; set; }
    }

    public class OptionDescription : DescriptionBase { }

    public class ErrorDescription : DescriptionBase
    {
        public int Status { get; set; }
    }

    public class DataTypeDescription : DescriptionBase
    {
        public System.Type Type { get; set; }
    }
}