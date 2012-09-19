using System;

namespace FubuMVC.Swank.Description
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DefaultValueAttribute : Attribute
    {
         public DefaultValueAttribute(object value)
         {
             Value = value;
         }

        public object Value { get; private set; }
    }
}