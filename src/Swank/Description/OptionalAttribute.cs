using System;

namespace FubuMVC.Swank.Description
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class OptionalAttribute : Attribute { }
}