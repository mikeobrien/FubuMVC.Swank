using System;
using System.Net;

namespace FubuMVC.Swank.Description
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ErrorDescriptionAttribute : Attribute
    {
        public ErrorDescriptionAttribute(HttpStatusCode status, string name, string comments = null) : 
            this((int)status, name, comments) { }

        public ErrorDescriptionAttribute(int status, string name, string comments = null)
        {
            Status = status;
            Name = name;
            Comments = comments;
        }
        
        public string Name { get; private set; }
        public string Comments { get; private set; }
        public int Status { get; private set; }
    }
}