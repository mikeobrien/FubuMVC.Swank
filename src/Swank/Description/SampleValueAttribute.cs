using System;

namespace FubuMVC.Swank.Description
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SampleValueAttribute : Attribute
    {
        public SampleValueAttribute(object value)
        {
            Value = value;
        }

        public object Value { get; private set; }
    }
}