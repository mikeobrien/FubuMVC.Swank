using System;

namespace Swank.Description
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ErrorDescriptionAttribute : Attribute
    {
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