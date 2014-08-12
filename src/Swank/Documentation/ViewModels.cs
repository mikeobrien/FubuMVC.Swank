using System.Collections.Generic;

namespace FubuMVC.Swank.Documentation
{
    public class SpecificationModel
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public List<ModuleModel> Modules { get; set; }
    }

    public class ModuleModel
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public List<ResourceModel> Resources { get; set; }
    }

    public class ResourceModel
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public List<EndpointModel> Endpoints { get; set; }
    }

    public class EndpointModel
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public List<UrlParameterModel> UrlParameters { get; set; }
        public List<QuerystringParameterModel> QuerystringParameters { get; set; }
        public List<StatusCodeModel> StatusCodes { get; set; }
        public List<HeaderModel> Headers { get; set; }
        public DataModel Request { get; set; }
        public DataModel Response { get; set; }
    }

    public class UrlParameterModel
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Type { get; set; }
        public List<OptionModel> Options { get; set; }
    }

    public class QuerystringParameterModel
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Type { get; set; }
        public string DefaultValue { get; set; }
        public bool MultipleAllowed { get; set; }
        public bool Required { get; set; }
        public List<OptionModel> Options { get; set; }
    }

    public class StatusCodeModel
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public int Code { get; set; }
    }

    public class HeaderModel
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Type { get; set; }
        public bool Optional { get; set; }
    }

    public class DataModel
    {
        public string Comments { get; set; }
        public List<SchemaModel> Schema { get; set; }
    }

    public class SchemaModel
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string TypeName { get; set; }
        public string DefaultValue { get; set; }
        public bool? Required { get; set; }
        public bool? Optional { get; set; }
        public string Whitespace { get; set; }
        public bool? IsDeprecated { get; set; }
        public string DeprecationMessage { get; set; }

        public bool? IsOpening { get; set; }
        public bool? IsClosing { get; set; }
        public bool? IsMember { get; set; }
        public bool? IsLastMember { get; set; }

        public bool? IsSimpleType { get; set; }
        public bool? IsComplexType { get; set; }
        public bool? IsArray { get; set; }
        public bool? IsDictionary { get; set; }
        public bool? IsDictionaryEntry { get; set; }

        public bool? IsString { get; set; }
        public bool? IsBoolean { get; set; }
        public bool? IsNumeric { get; set; }
        public bool? IsDateTime { get; set; }
        public bool? IsDuration { get; set; }
        public bool? IsGuid { get; set; }

        public List<OptionModel> Options { get; set; }

        public KeyModel Key { get; set; }
    }

    public class KeyModel
    {
        public string Comments { get; set; }
        public string TypeName { get; set; }
        public List<OptionModel> Options { get; set; }
    }

    public class OptionModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Comments { get; set; }
    }
}