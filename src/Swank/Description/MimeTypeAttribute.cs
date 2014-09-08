using System;
using FubuMVC.Swank.Net;

namespace FubuMVC.Swank.Description
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class MimeTypeAttribute : Attribute
    {
        public MimeTypeAttribute(HttpDirection type, string name)
        {
            Type = type;
            Name = name;
        }

        public MimeTypeAttribute(HttpDirection type, MimeType mimeType)
        {
            Type = type;
            Name = mimeType.ToMimeType();
        }
        
        public HttpDirection Type { get; private set; }
        public string Name { get; private set; }
    }
}