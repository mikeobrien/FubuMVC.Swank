using System;
using System.Net;

namespace FubuMVC.Swank.Description
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class StatusCodeDescriptionAttribute : Attribute
    {
        public StatusCodeDescriptionAttribute(HttpStatusCode code, string name, string comments = null) : 
            this((int)code, name, comments) { }

        public StatusCodeDescriptionAttribute(int code, string name, string comments = null)
        {
            Code = code;
            Name = name;
            Comments = comments;
        }
        
        public string Name { get; private set; }
        public string Comments { get; private set; }
        public int Code { get; private set; }
    }
}