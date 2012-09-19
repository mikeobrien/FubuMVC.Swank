using System;

namespace FubuMVC.Swank.Description
{
    public class ResourceAttribute : Attribute
    {
        public ResourceAttribute(string name, string comments = null)
        {
            Name = name;
            Comments = comments;
        }

        public string Name { get; private set; }
        public string Comments { get; private set; }
    }
}
