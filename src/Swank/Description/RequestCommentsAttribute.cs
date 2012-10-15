using System;

namespace FubuMVC.Swank.Description
{
    public class RequestCommentsAttribute : Attribute
    {
        public RequestCommentsAttribute(string comments)
        {
            Comments = comments;
        }

        public string Comments { get; private set; }
    }
}
