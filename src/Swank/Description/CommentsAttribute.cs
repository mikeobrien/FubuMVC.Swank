using System;

namespace Swank.Description
{
    public class CommentsAttribute : Attribute
    {
        public CommentsAttribute(string comments)
        {
            Comments = comments;
        }

        public string Comments { get; private set; }
    }
}
