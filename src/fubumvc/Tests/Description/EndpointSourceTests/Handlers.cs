using System;
using Swank.Description;

namespace Tests.Description.EndpointSourceTests
{
    namespace ControllerResource
    {
        [Resource("Some Controller")]
        public class Controller
        {
            public object ExecuteGet(object request) { return null; }
        }
    }

    namespace Templates
    {
        public class TemplateRequest { public Guid Id { get; set; } }

        [Description("GetTemplates", "This gets all the templates yo.")]
        public class TemplateAllGetHandler
        {
            public object Execute(TemplateRequest request) { return null; }
        }

        public class TemplatePostHandler
        {
            [Description("AddTemplate", "This adds a the template yo.")]
            public object Execute(TemplateRequest request) { return null; }
        }

        public class TemplateGetHandler
        {
            public object Execute_Id(TemplateRequest request) { return null; }
        }

        public class TemplatePutHandler { public object Execute_Id(TemplateRequest request) { return null; } }
    }
}