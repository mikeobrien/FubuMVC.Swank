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
}
