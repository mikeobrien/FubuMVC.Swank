using System;

namespace FubuMVC.Swank.Description
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class HeaderDescriptionAttribute : Attribute
    {
        public HeaderDescriptionAttribute(HttpHeaderType type, string name, string comments = null, bool optional = false)
        {
            Type = type;
            Name = name;
            Comments = comments;
            Optional = optional;
        }
        
        public HttpHeaderType Type { get; private set; }
        public string Name { get; private set; }
        public string Comments { get; private set; }
        public bool Optional { get; set; }
    }
}