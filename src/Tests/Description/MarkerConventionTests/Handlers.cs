using FubuMVC.Swank.Description;

namespace Tests.Description.MarkerConventionTests
{
    namespace MarkerDescriptions
    {
        namespace NoDescription
        {
            public class Description : FubuMVC.Swank.Description.Description { }
        }

        namespace Description
        {
            public class Description : FubuMVC.Swank.Description.Description { public Description() { Name = "Some Description"; Comments = "Some comments."; } }
        }

        namespace EmbeddedTextComments
        {
            public class Description : FubuMVC.Swank.Description.Description { public Description() { Name = "Some Text Description"; } }
        }

        namespace EmbeddedMarkdownComments
        {
            public class Description : FubuMVC.Swank.Description.Description { public Description() { Name = "Some Markdown Description"; } }
        }
    }

    namespace MarkerOrder
    {
        namespace AFirstMarker
        {
            public class LastDescription : FubuMVC.Swank.Description.Description { public LastDescription() { Name = "Last Description"; } }
            public class FirstDescription : FubuMVC.Swank.Description.Description { public FirstDescription() { Name = "First Description"; } }
        }

        namespace ZeeLastMarker
        {
            public class LastDescription : FubuMVC.Swank.Description.Description { public LastDescription() { Name = "Last Description"; } }
            public class FirstDescription : FubuMVC.Swank.Description.Description { public FirstDescription() { Name = "First Description"; } }
        }
    }

    namespace MarkerCommentsPriority
    {
        public class Description : FubuMVC.Swank.Description.Description { public Description() { Name = "Some Description"; Comments = "Some comments."; } }
    }
}