namespace FubuMVC.Swank.Description
{
    public static class DescriptionExtensions
    {
        public static string GetNameOrDefault(this DescriptionBase description, string defaultName = null)
        {
            return (description != null ? description.Name : null) ?? defaultName;
        }

        public static string GetCommentsOrDefault(this DescriptionBase description, string defaultComments = null)
        {
            return (description != null ? description.Comments : null) ?? defaultComments;
        } 
    }

    public class DescriptionBase
    {
        public string Name { get; set; }
        public string Comments { get; set; }

        public override bool Equals(object obj)
        {
            return obj is DescriptionBase && ((DescriptionBase)obj).Name == Name;
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }
    }
}
