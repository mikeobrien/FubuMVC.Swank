using System;

namespace FubuMVC.Swank.Description
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class HeaderAttribute : Attribute
    {
        public HeaderAttribute(HttpDirection type, string name, string comments = null, bool optional = false)
        {
            Type = type;
            Name = name;
            Comments = comments;
            Optional = optional;
        }
        
        public HttpDirection Type { get; private set; }
        public string Name { get; private set; }
        public string Comments { get; private set; }
        public bool Optional { get; set; }
    }
}