using FubuMVC.Swank.Description;

namespace Tests.Description.MarkerConventionTests
{
    namespace MarkerDescriptions
    {
        namespace NoDescription
        {
            public class Description : DescriptionBase { }
        }

        namespace Description
        {
            public class Description : DescriptionBase { public Description() { Name = "Some Description"; Comments = "Some comments."; } }
        }

        namespace EmbeddedTextComments
        {
            public class Description : DescriptionBase { public Description() { Name = "Some Text Description"; } }
        }

        namespace EmbeddedMarkdownComments
        {
            public class Description : DescriptionBase { public Description() { Name = "Some Markdown Description"; } }
        }
    }

    namespace MarkerOrder
    {
        namespace AFirstMarker
        {
            public class LastDescription : DescriptionBase { public LastDescription() { Name = "Last Description"; } }
            public class FirstDescription : DescriptionBase { public FirstDescription() { Name = "First Description"; } }
        }

        namespace ZeeLastMarker
        {
            public class LastDescription : DescriptionBase { public LastDescription() { Name = "Last Description"; } }
            public class FirstDescription : DescriptionBase { public FirstDescription() { Name = "First Description"; } }
        }
    }

    namespace MarkerCommentsPriority
    {
        public class Description : DescriptionBase { public Description() { Name = "Some Description"; Comments = "Some comments."; } }
    }
}