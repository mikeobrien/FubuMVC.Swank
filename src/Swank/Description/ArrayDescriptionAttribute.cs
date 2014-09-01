namespace FubuMVC.Swank.Description
{
    public class ArrayDescriptionAttribute : DescriptionAttribute
    {
        public ArrayDescriptionAttribute(
            string name = null,
            string comments = null, 
            string itemName = null,
            string itemComments = null) : 
                base(name, comments)
        {
            ItemName = itemName;
            ItemComments = itemComments;
        }

        public string ItemName { get; private set; }
        public string ItemComments { get; private set; }
    }
}