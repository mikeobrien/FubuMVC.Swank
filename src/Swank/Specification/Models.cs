using System.Collections.Generic;

namespace FubuMVC.Swank.Specification
{
    public class Specification
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Copyright { get; set; }
        public List<Type> Types { get; set; }
        public List<Module> Modules { get; set; }
        public List<Resource> Resources { get; set; }
    }

    public class Type
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public List<Member> Members { get; set; }
    }

    public class Member
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public bool Required { get; set; }
        public string DefaultValue { get; set; }
        public bool Collection { get; set; }
        public string Type { get; set; }
        public List<Option> Options { get; set; }
    }

    public class Option
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Value { get; set; }
    }

    public class Module
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public List<Resource> Resources { get; set; }
    }

    public class Resource
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public List<Endpoint> Endpoints { get; set; }
    }

    public class Endpoint
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public List<UrlParameter> UrlParameters { get; set; }
        public List<QuerystringParameter> QuerystringParameters { get; set; }
        public List<Error> Errors { get; set; }
        public Data Request { get; set; }
        public Data Response { get; set; }
    }

    public class UrlParameter
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Type { get; set; }
        public List<Option> Options { get; set; }
    }

    public class QuerystringParameter
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Type { get; set; }
        public string DefaultValue { get; set; }
        public bool MultipleAllowed { get; set; }
        public bool Required { get; set; }
        public List<Option> Options { get; set; }
    }

    public class Error
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public int Status { get; set; }
    }

    public class Data
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Type { get; set; }
        public bool Collection { get; set; }
    }
}