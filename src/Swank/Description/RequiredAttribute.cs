using System;

namespace FubuMVC.Swank.Description
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RequiredAttribute : Attribute
    {
        public RequiredAttribute()
        {
            IsRequired = true;
        }

        public RequiredAttribute(bool isRequired)
        {
            IsRequired = isRequired;
        }

        public bool IsRequired { get; private set; }
    }
}