using System;
using FubuMVC.Swank.Description;

namespace Tests.Description.EndpointConventionTests
{
    namespace EndpointDescriptions
    {
        public class NoDescriptionGetHandler { public object Execute_NoDescription(object request) { return null; } }

        namespace HandlerDescription
        {
            [Description("Some embedded handler name")]
            public class EmbeddedDescriptionGetHandler { public object Execute_EmbeddedDescription(object request) { return null; } }

            [Comments("Some handler comments")]
            public class AttrbuteCommentsGetHandler { public object Execute_EmbeddedDescription(object request) { return null; } }

            [Description("Some handler name", "Some handler description")]
            public class GetHandler { public object Execute(object request) { return null; } }
        }

        namespace ActionDescription
        {
            public class EmbeddedDescriptionGetHandler { public object Execute_EmbeddedDescription(object request) { return null; } }

            public class AttrbuteCommentsGetHandler
            {
                [Comments("Some action comments")]
                public object Execute(object request) { return null; }
            }

            public class GetHandler
            {
                [Description("Some action name", "Some action description")]
                public object Execute(object request) { return null; }
            }
        }

        namespace RequestDescription
        {
            public class EmbeddedDescriptionGetHandler { public object Execute(object request) { return null; } }
            public class EmbeddedHandlerDescriptionGetHandler { public object Execute(object request) { return null; } }

            public class AttributeGetHandler
            {
                [RequestComments("Some request description")]
                public object Execute(object request) { return null; }
            }
        }

        namespace ResponseDescription
        {
            public class EmbeddedDescriptionGetHandler { public object Execute(object request) { return null; } }
            public class EmbeddedHandlerDescriptionGetHandler { public object Execute(object request) { return null; } }

            public class AttributeGetHandler
            {
                [ResponseComments("Some response description")]
                public object Execute(object request) { return null; }
            }
        }
    }

    namespace ControllerResource
    {
        [Resource("Some Controller")]
        public class Controller
        {
            public object Execute(object request) { return null; }
        }
    }

    namespace AttributePriority
    {
        [Description("Some action name", "Some action description")]
        public class GetHandler
        {
            public object Execute(object request) { return null; }
        }
    }
}