using System;

namespace Swank.Description
{
    public static class DescriptionExtensions
    {
        public static string GetNameOrDefault(this Description description, string defaultName = null)
        {
            return (description != null ? description.Name : null) ?? defaultName;
        }

        public static string GetCommentsOrDefault(this Description description, string defaultComments = null)
        {
            return (description != null ? description.Comments : null) ?? defaultComments;
        } 
    }

    public abstract class Description
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Namespace { get; set; }
        public Type AppliesTo { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Description && ((Description)obj).Name == Name;
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }
    }
}
