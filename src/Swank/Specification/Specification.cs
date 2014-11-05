using System.Collections.Generic;

namespace FubuMVC.Swank.Specification
{
    public class Specification : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public List<Module> Modules { get; set; }
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
        public string Id { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public List<UrlParameter> UrlParameters { get; set; }
        public List<QuerystringParameter> QuerystringParameters { get; set; }
        public bool Secure { get; set; }
        public List<StatusCode> StatusCodes { get; set; }
        public Data Request { get; set; }
        public Data Response { get; set; }
    }

    public class UrlParameter : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Type { get; set; }
        public Enumeration Options { get; set; }
    }

    public class QuerystringParameter : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Type { get; set; }
        public string DefaultValue { get; set; }
        public bool MultipleAllowed { get; set; }
        public bool Required { get; set; }
        public Enumeration Options { get; set; }
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
        public bool Optional { get; set; }
        public bool Required { get; set; }
        public bool IsContentType { get; set; }
        public bool IsAccept { get; set; }
    }

    public class Data
    {
        public string Comments { get; set; }
        public List<Header> Headers { get; set; }
        public List<string> MimeTypes { get; set; }
        public Body Body { get; set; }
    }

    public class Body
    {
        public DataType Type { get; set; }
        public List<BodyLineItem> Description { get; set; }
    }

    public class DataType : IDescription
    {
        public string Name { get; set; }
        public List<string> LongNamespace { get; set; }
        public List<string> ShortNamespace { get; set; }
        public string Comments { get; set; }

        public bool IsSimple { get; set; }
        public Enumeration Options { get; set; }

        public bool IsComplex { get; set; }
        public List<Member> Members { get; set; }

        public bool IsArray { get; set; }
        public ArrayItem ArrayItem { get; set; }

        public bool IsDictionary { get; set; }
        public DictionaryEntry DictionaryEntry { get; set; }
    }

    public class ArrayItem : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public DataType Type { get; set; }
    }

    public class DictionaryEntry
    {
        public string KeyName { get; set; }
        public DataType KeyType { get; set; }
        public string KeyComments { get; set; }

        public DataType ValueType { get; set; }
        public string ValueComments { get; set; }
    }

    public class Member : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public bool Required { get; set; }
        public bool Optional { get; set; }
        public string DefaultValue { get; set; }
        public string SampleValue { get; set; }
        public bool Deprecated { get; set; }
        public string DeprecationMessage { get; set; }
        public DataType Type { get; set; }
    }

    public class BodyLineItem
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public List<string> LongNamespace { get; set; }
        public List<string> ShortNamespace { get; set; }
        public string Comments { get; set; }
        public bool? IsFirst { get; set; }
        public bool? IsLast { get; set; }
        public string TypeName { get; set; }
        public string SampleValue { get; set; }
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
        public bool? IsString { get; set; }
        public bool? IsBoolean { get; set; }
        public bool? IsNumeric { get; set; }
        public bool? IsDateTime { get; set; }
        public bool? IsDuration { get; set; }
        public bool? IsGuid { get; set; }
        public Enumeration Options { get; set; }

        public bool? IsComplexType { get; set; }

        public bool? IsArray { get; set; }

        public bool? IsDictionary { get; set; }
        public bool? IsDictionaryEntry { get; set; }
        public Key DictionaryKey { get; set; }
    }

    public class Key
    {
        public string Comments { get; set; }
        public string TypeName { get; set; }
        public Enumeration Options { get; set; }
    }

    public class Enumeration
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public List<Option> Options { get; set; }
    }

    public class Option
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Comments { get; set; }
    }
}