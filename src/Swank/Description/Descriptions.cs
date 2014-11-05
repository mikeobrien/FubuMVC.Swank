namespace FubuMVC.Swank.Description
{
    public class ModuleDescription : Description
    {
        public ModuleDescription() {}
        public ModuleDescription(string name, string comments = null) : base(name, comments) { }
    }

    public class ResourceDescription<THandler> : ResourceDescription
    {
        public ResourceDescription() { }
        public ResourceDescription(string name, string comments = null) : base(name, comments) { }
    }

    public class ResourceDescription : Description
    {
        public ResourceDescription() {}
        public ResourceDescription(string name, string comments = null) : base(name, comments) { }

        public System.Type Handler { get; set; }
    }

    public class EndpointDescription : Description
    {
        public bool Secure { get; set; }
        public string RequestComments { get; set; }
        public string ResponseComments { get; set; }
    }

    public class MemberDescription : Description
    {
        public object SampleValue { get; set; }
        public object DefaultValue { get; set; }
        public bool Optional { get; set; }
        public bool Hidden { get; set; }
        public bool Deprecated { get; set; }
        public string DeprecationMessage { get; set; }
        public Description ArrayItem { get; set; }
        public DictionaryDescription DictionaryEntry { get; set; }
    }

    public class DictionaryDescription
    {
        public string KeyName { get; set; }
        public string KeyComments { get; set; }
        public string ValueComments { get; set; }
    }

    public class EnumDescription : Description { }

    public class EnumOptionDescription : Description
    {
        public bool Hidden { get; set; }
    }

    public enum HttpDirection { Request, Response }

    public class HeaderDescription : Description
    {
        public HttpDirection Direction { get; set; }
        public bool Optional { get; set; }
    }

    public class MimeTypeDescription
    {
        public HttpDirection Direction { get; set; }
        public string Name { get; set; }
    }

    public class StatusCodeDescription : Description
    {
        public int Code { get; set; }
    }

    public class TypeDescription : Description
    {
        public Description ArrayItem { get; set; }
        public DictionaryDescription DictionaryEntry { get; set; }
    }
}