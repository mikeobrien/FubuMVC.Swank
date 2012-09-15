using System;
using System.Collections.Generic;

namespace Tests.SpecificationBuilderDataTypeTests
{
    namespace Templates
    {

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

        public class TemplatePostPutRequest 
        { 
            public Guid Id { get; set; } 
        }

        public class TemplateGetRequest { public Guid Id { get; set; } }
        public class TemplateDeleteRequest { public Guid Id { get; set; } }

        public class TemplateResponse
        {
            public int RevisionNumber { get; set; }
            public List<TemplateRevision> Revisions { get; set; }
            public TemplateAuthors Authors { get; set; }
            public TemplateAuthor OriginalAuthor { get; set; }
            public List<DateTime> RevisionDates { get; set; }
        }

        public class TemplateAllGetHandler { public TemplateResponse Execute(TemplateGetRequest request) { return null; } }
        public class TemplatePostHandler { public TemplateResponse Execute(TemplatePostPutRequest request) { return null; } }
        public class TemplateGetHandler { public TemplateResponse Execute_Id(TemplateGetRequest request) { return null; } }
        public class TemplatePutHandler { public TemplateResponse Execute_Id(TemplatePostPutRequest request) { return null; } }
        public class TemplateDeleteHandler { public TemplateResponse Execute_Id(TemplateGetRequest request) { return null; } }
    }
}