using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using FubuMVC.Core;
using Swank.Description;
using Tests.SpecificationBuilderEndpointTests.Administration.Users;

namespace Tests.SpecificationBuilderEndpointTests
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

    namespace Batches
    {
        public class BatchesModule : ModuleDescription { public BatchesModule() { Name = "Batches"; } }

        namespace Schedules
        {
            public class SchedulesModule : ModuleDescription { public SchedulesModule() { Name = "Schedules"; } }
            public class BatchScheduleGetAllHandler { public object Execute(object request) { return null; } }
            public class BatchSchedulePostHandler { public object Execute(object request) { return null; } }
        }

        namespace Cells
        {
            public class BatchCellResource : ResourceDescription { public BatchCellResource() { Name = "Batch cells"; } }
            public class BatchCellGetAllHandler { public object Execute(object request) { return null; } }
            public class BatchCellPostHandler { public object Execute(object request) { return null; } }
        }
    }

    namespace Administration
    {
        public class AdministrationModule : ModuleDescription { public AdministrationModule() { Name = "Administration";  } }

        public class AdminAccountResource : ResourceDescription<AdminAccountGetAllHandler>
        { public AdminAccountResource() { Name = "Accounts"; } }

        namespace Users // These are ordered a certian way on purpose, don't change that.
        {
            public class AdminAccountRequest
            {
                public Guid Id { get; set; }
                public Guid UserId { get; set; }
                [QueryString, DefaultValue(Sort.Desc)]
                public Sort Order { get; set; }
                [QueryString]
                public List<Guid> Show { get; set; }
                [QueryString, Hide]
                public string SystemOption { get; set; }
            }

            public class AdminAccountGetAllHandler { public object Execute_UserId(AdminAccountRequest request) { return null; } }
            public class AdminAccountPostHandler { public object Execute_UserId(AdminAccountRequest request) { return null; } }

            public class AdminAddressResource : ResourceDescription { public AdminAddressResource() { Name = "User addresses"; } }

            public enum AddressType
            {
                [Description("Work Address", "This is the work address of the user.")]
                Work,
                [Description("Home address", "This is the home address of the user.")]
                Home,
                Emergency,
                [Hide]
                Private
            }

            public enum Sort { Asc, Desc }

            [Comments("This is an address request yo!")]
            public class AdminAddressRequest
            {
                [Comments("This is the id.")]
                public Guid Id { get; set; }
                public Sort Order { get; set; }
                [Hide]
                public string SystemOption { get; set; }
                public string ContentType { get; set; }
                [Comments("This is the id of the user.")]
                public Guid UserId { get; set; }
                public AddressType AddressType { get; set; }
            }

            public class AdminAddressResponse { }

            [Comments("These are addresses yo!")]
            public class AdminAddresses : List<AdminAddressResponse> { }

            public class AdminAddressGetAllHandler
            {
                [ErrorDescription(411, "Swank address")]
                [ErrorDescription(410, "Invalid address", "An invalid address was entered fool!")]
                public List<AdminAddressResponse> Execute_UserId(AdminAddressRequest request) { return null; }
            }
            public class AdminAddressGetAllOfTypeHandler { public AdminAddresses Execute_UserId_AddressType(AdminAddressRequest request) { return null; } }

            public class AdminUserResource : ResourceDescription<AdminUserGetAllHandler> { public AdminUserResource() { Name = "Users"; } }

            [Comments("These are users yo!"), XmlType("Users")]
            public class AdminUsers : List<AdminUserResponse> { }

            [XmlType("User")]
            public class AdminUserRequest { public Guid Id { get; set; } }
            public class AdminUserResponse { }
            public class AdminUserGetAllHandler { public AdminUsers Execute(AdminUserRequest request) { return null; } }
            public class AdminUserPostHandler { public AdminUserResponse Execute(AdminUserRequest request) { return null; } }
        }
    }
}
