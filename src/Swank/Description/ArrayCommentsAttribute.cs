namespace FubuMVC.Swank.Description
{
    public class ArrayCommentsAttribute : CommentsAttribute
    {
        public ArrayCommentsAttribute(
            string comments = null, 
            string itemComments = null) : 
                base(comments)
        {
            ItemComments = itemComments;
        }

        public string ItemComments { get; private set; }
    }
}