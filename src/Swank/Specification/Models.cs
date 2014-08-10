using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace FubuMVC.Swank.Specification
{
    public interface IDescription
    {
        string Name { get; set; }
        string Comments { get; set; }
    }

    public class Specification : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public List<Type> Types { get; set; }
        public List<Module> Modules { get; set; }
    }

    public class Type : IDescription
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public List<Member> Members { get; set; }
    }

    public class Member : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public bool Required { get; set; }
        public string DefaultValue { get; set; }
        public bool IsArray { get; set; }
        public string ArrayItemName { get; set; }
        public bool IsDictionary { get; set; }
        public string DictionaryKeyType { get; set; }
        public string Type { get; set; }
        public List<Option> Options { get; set; }
    }

    public class Module : IDescription
    {
        public const string DefaultName = "Resources";

        public string Name { get; set; }
        public string Comments { get; set; }
        public List<Resource> Resources { get; set; }
    }

    public class Resource : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public List<Endpoint> Endpoints { get; set; }
    }

    public class Endpoint : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public List<UrlParameter> UrlParameters { get; set; }
        public List<QuerystringParameter> QuerystringParameters { get; set; }
        public List<StatusCode> StatusCodes { get; set; }
        public List<Header> Headers { get; set; }
        public Data Request { get; set; }
        public Data Response { get; set; }
    }

    public class UrlParameter : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Type { get; set; }
        public List<Option> Options { get; set; }
    }

    public class QuerystringParameter : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Type { get; set; }
        public string DefaultValue { get; set; }
        public bool MultipleAllowed { get; set; }
        public bool Required { get; set; }
        public List<Option> Options { get; set; }
    }

    public class StatusCode : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public int Code { get; set; }
    }

    public class Header : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Type { get; set; }
        public bool Optional { get; set; }
    }

    public class Data
    {
        public List<DataDescription> Description { get; set; }
    }

    public class DataDescription : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string TypeName { get; set; }
        public string DefaultValue { get; set; }
        public bool Required { get; set; }
        public string Whitespace { get; set; }
        public bool IsDeprecated { get; set; }
        public string DeprecationMessage { get; set; }

        public bool IsOpening { get; set; }
        public bool IsClosing { get; set; }
        public bool IsMember { get; set; }
        public bool IsLastMember { get; set; }

        public bool IsSimpleType { get; set; }
        public bool IsComplexType { get; set; }
        public bool IsArray { get; set; }
        public bool IsDictionary { get; set; }
        public bool IsDictionaryEntry { get; set; }

        public bool IsString { get; set; }
        public bool IsBoolean { get; set; }
        public bool IsNumeric { get; set; }
        public bool IsDateTime { get; set; }
        public bool IsDuration { get; set; }
        public bool IsGuid { get; set; }

        public bool IsEnumeration { get; set; }
        public List<Option> Options { get; set; }

        public string KeyComments { get; set; }
        public string KeyTypeName { get; set; }
        public bool KeyIsEnumeration { get; set; }
        public List<Option> KeyOptions { get; set; }
    }

    public class Option
    {
        public string OptionValue { get; set; }
        public string OptionComments { get; set; }
    }
}