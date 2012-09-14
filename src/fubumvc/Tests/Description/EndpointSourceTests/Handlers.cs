using System;
using System.Collections.Generic;
using Swank.Description;

namespace Tests.Description.EndpointSourceTests
{
    namespace Templates
    {
        public class TemplateRequest { public Guid Id { get; set; } }

        public class TemplateRevision
        {
            public int RevisionNumber { get; set; }
            public TemplateAuthor Author { get; set; }
        }

        public interface IPermissions
        {
            bool IsAdmin { get; set; }
        }

        public class TemplateAuthor
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public IPermissions Permissions { get; set; }
        }

        public class TemplateAuthors : List<TemplateAuthor> { }

        public class TemplateResponse
        {
            public int RevisionNumber { get; set; }
            public List<TemplateRevision> Revisions { get; set; }
            public TemplateAuthors Authors { get; set; }
            public TemplateAuthor OriginalAuthor { get; set; }
            public List<DateTime> RevisionDates { get; set; }
        }

        [Description("GetTemplates", "This gets all the templates yo.")]
        public class TemplateGetAllHandler
        {
            public TemplateResponse Execute(TemplateRequest request) { return null; }
        }

        public class TemplatePostHandler
        {
            [Description("AddTemplate", "This adds a the template yo.")]
            public TemplateResponse Execute(TemplateRequest request) { return null; }
        }

        public class TemplateGetHandler
        {
            public TemplateResponse Execute_Id(TemplateRequest request) { return null; }
        }

        public class TemplatePutHandler { public TemplateResponse Execute_Id(TemplateRequest request) { return null; } }
        public class TemplateDeleteHandler { public TemplateResponse Execute_Id(TemplateRequest request) { return null; } }

        public class TemplateFileRequest { public Guid Id { get; set; } }

        public class TemplateFileGetAllHandler
        {
            [Hide]
            public object Execute_File(TemplateFileRequest request) { return null; }
        }

        [Hide]
        public class TemplateFilePostHandler { public object Execute_File(TemplateFileRequest request) { return null; } }
        public class TemplateFileGetHandler { public object Execute_File_Id(TemplateFileRequest request) { return null; } }
    }
}