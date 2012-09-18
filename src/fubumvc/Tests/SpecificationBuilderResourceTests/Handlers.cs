using System;
using Swank.Description;

namespace Tests.SpecificationBuilderResourceTests
{
    namespace ResourceDescriptions
    {
        namespace NoDescription
        {
            public class Resource : ResourceDescription { }
            public class GetHandler { public object Execute(object request) { return null; } }
        }

        namespace Description
        {
            public class Resource : ResourceDescription { public Resource() { Name = "Some Resource"; Comments = "Some comments."; } }
            public class GetHandler { public object Execute(object request) { return null; } }
        }

        namespace EmbeddedTextComments
        {
            public class Resource : ResourceDescription { public Resource() { Name = "Some Text Resource"; } }
            public class GetHandler { public object Execute(object request) { return null; } }
        }

        namespace EmbeddedMarkdownComments
        {
            public class Resource : ResourceDescription { public Resource() { Name = "Some Markdown Resource"; } }
            public class GetHandler { public object Execute(object request) { return null; } }
        }
    }

    namespace OrphanedAction
    {
        public class GetHandler { public object Execute(object request) { return null; } }
    }

    namespace NotOrphanedAction
    {
        public class Resource : ResourceDescription { public Resource() { Name = "Some Resource"; } }
        public class GetHandler { public object Execute(object request) { return null; } }
    }

    namespace SameNamespace
    {
        public class Resource : ResourceDescription { public Resource() { Name = "Some Resource"; } }

        public class Request { public Guid Id { get; set; } }

        public class GetHandler { public object Execute_Id(Request request) { return null; } }
        public class PostHandler { public object Execute(Request request) { return null; } }

        public class WidgetGetHandler { public object Execute_Widget_Id(Request request) { return null; } }
        public class WidgetPostHandler { public object Execute_Widget(Request request) { return null; } }
    }

    namespace NestedResources
    {
        public class Resource : ResourceDescription { public Resource() { Name = "Some Resource"; } }

        public class Request { public Guid Id { get; set; } }

        public class GetHandler { public object Execute_Id(Request request) { return null; } }
        public class PostHandler { public object Execute(Request request) { return null; } }

        namespace Widget
        {
            public class WidgetGetHandler { public object Execute_Id(Request request) { return null; } }
            public class WidgetPostHandler { public object Execute(Request request) { return null; } }
        }
    }

    namespace OrphanedResources
    {
        public class Request { public Guid Id { get; set; } }

        public class GetHandler { public object Execute_Id(Request request) { return null; } }
        public class PostHandler { public object Execute(Request request) { return null; } }

        public class WidgetGetHandler { public object Execute_Widget_Id(Request request) { return null; } }
        public class WidgetPostHandler { public object Execute_Widget(Request request) { return null; } }
    }
}