using System;

namespace FubuMVC.Swank.Description
{
    public class CommentsAttribute : Attribute
    {
        public CommentsAttribute(string comments)
        {
            Comments = comments;
        }

        public string Comments { get; private set; }
    }

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

    public class DictionaryCommentsAttribute : CommentsAttribute
    {
        public DictionaryCommentsAttribute(
            string comments = null,
            string keyComments = null, 
            string valueComments = null) : 
            base(comments)
        {
            KeyComments = keyComments;
            ValueComments = valueComments;
        }

        public string KeyComments { get; private set; }
        public string ValueComments { get; private set; }
    }
}
