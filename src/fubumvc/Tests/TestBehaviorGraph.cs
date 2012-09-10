using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using Swank.Description;
using Tests.Administration.Users;
using Tests.Batches.Cells;
using Tests.Batches.Schedules;
using Tests.Templates;

namespace Tests
{
    public enum HttpVerbs
    {
        Head, Get, Post, Put, Delete, Options, Trace, Connect, Patch
    }

    public static class Behaviors
    {

        public static BehaviorGraph BuildGraph()
        {
            return new BehaviorGraph();
        }

        public static BehaviorGraph AddAction<T>(this BehaviorGraph graph, string route, HttpVerbs verb)
        {
            var chain = graph.AddActionFor(route, typeof(T));
            chain.Route.AddHttpMethodConstraint(verb.ToString().ToUpper());
            return graph;
        }
    }

    public static class TestBehaviorGraph
    {
        public static BehaviorGraph Build()
        {
            return Behaviors.BuildGraph()
                .AddAction<TemplateGetAllHandler>("/templates", HttpVerbs.Get)
                .AddAction<TemplatePostHandler>("/templates", HttpVerbs.Post)
                .AddAction<TemplateGetHandler>("/templates/{Id}", HttpVerbs.Get)
                .AddAction<TemplatePutHandler>("/templates/{Id}", HttpVerbs.Put)
                .AddAction<TemplateDeleteHandler>("/templates/{Id}", HttpVerbs.Delete)
                .AddAction<TemplateFileGetAllHandler>("/templates/files", HttpVerbs.Get)
                .AddAction<TemplateFilePostHandler>("/templates/files", HttpVerbs.Post)
                .AddAction<TemplateFileGetHandler>("/templates/files/{Id}", HttpVerbs.Get)
                .AddAction<TemplateFilePutHandler>("/templates/files/{Id}", HttpVerbs.Put)
                .AddAction<TemplateFileDeleteHandler>("/templates/files/{Id}", HttpVerbs.Delete)
                .AddAction<AdminAccountGetAllHandler>("/admin", HttpVerbs.Get)
                .AddAction<AdminAccountPostHandler>("/admin", HttpVerbs.Post)
                .AddAction<AdminAccountGetHandler>("/admin/{Id}", HttpVerbs.Get)
                .AddAction<AdminAccountPutHandler>("/admin/{Id}", HttpVerbs.Put)
                .AddAction<AdminAccountDeleteHandler>("/admin/{Id}", HttpVerbs.Delete)
                .AddAction<AdminUserGetAllHandler>("/admin/users", HttpVerbs.Get)
                .AddAction<AdminUserPostHandler>("/admin/users", HttpVerbs.Post)
                .AddAction<AdminUserGetHandler>("/admin/users/{Id}", HttpVerbs.Get)
                .AddAction<AdminUserPutHandler>("/admin/users/{Id}", HttpVerbs.Put)
                .AddAction<AdminUserDeleteHandler>("/admin/users/{Id}", HttpVerbs.Delete)
                .AddAction<AdminAddressGetAllHandler>("/admin/users/{UserId}/addresses", HttpVerbs.Get)
                .AddAction<AdminAddressGetAllOfTypeHandler>("/admin/users/{UserId}/addresses/{AddressType}", HttpVerbs.Get)
                .AddAction<AdminAddressPostHandler>("/admin/users/{UserId}/addresses", HttpVerbs.Post)
                .AddAction<AdminAddressGetHandler>("/admin/users/{UserId}/addresses/{Id}", HttpVerbs.Get)
                .AddAction<AdminAddressPutHandler>("/admin/users/{UserId}/addresses/{Id}", HttpVerbs.Put)
                .AddAction<AdminAddressDeleteHandler>("/admin/users/{UserId}/addresses/{Id}", HttpVerbs.Delete)
                .AddAction<BatchCellGetAllHandler>("/batches/cells", HttpVerbs.Get)
                .AddAction<BatchCellPostHandler>("/batches/cells", HttpVerbs.Post)
                .AddAction<BatchCellGetHandler>("/batches/cells/{Id}", HttpVerbs.Get)
                .AddAction<BatchCellPutHandler>("/batches/cells/{Id}", HttpVerbs.Put)
                .AddAction<BatchCellDeleteHandler>("/batches/cells/{Id}", HttpVerbs.Delete)
                .AddAction<BatchScheduleGetAllHandler>("/batches/schedules", HttpVerbs.Get)
                .AddAction<BatchSchedulePostHandler>("/batches/schedules", HttpVerbs.Post)
                .AddAction<BatchScheduleGetHandler>("/batches/schedules/{Id}", HttpVerbs.Get)
                .AddAction<BatchSchedulePutHandler>("/batches/schedules/{Id}", HttpVerbs.Put)
                .AddAction<BatchScheduleDeleteHandler>("/batches/schedules/{Id}", HttpVerbs.Delete);
        }

        public static ActionCall CreateAction<T>()
        {
            return Build().Actions().First(x => x.HandlerType == typeof(T));
        }
    }

    namespace Templates
    {
        public class TemplateRequest { public Guid Id { get; set; } }
        public class TemplateResponse { }

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

        public class TemplateGetHandler { public TemplateResponse Execute_Id(TemplateRequest request) { return null; } }
        public class TemplatePutHandler { public TemplateResponse Execute_Id(TemplateRequest request) { return null; } }
        public class TemplateDeleteHandler { public TemplateResponse Execute_Id(TemplateRequest request) { return null; } }

        public class TemplateFileRequest { public Guid Id { get; set; } }
        public class TemplateFileResponse { }
        public class TemplateFileGetAllHandler { public TemplateFileResponse Execute_File(TemplateFileRequest request) { return null; } }
        public class TemplateFilePostHandler { public TemplateFileResponse Execute_File(TemplateFileRequest request) { return null; } }
        public class TemplateFileGetHandler { public TemplateFileResponse Execute_File_Id(TemplateFileRequest request) { return null; } }
        public class TemplateFilePutHandler { public TemplateFileResponse Execute_File_Id(TemplateFileRequest request) { return null; } }
        public class TemplateFileDeleteHandler { public TemplateFileResponse Execute_File_Id(TemplateFileRequest request) { return null; } }
    }

    namespace Batches
    {
        public class BatchesModule : ModuleDescription
        {
            public const string ExpectedComments = "<b>These are batches yo!</b>";
            public BatchesModule()
            {
                Name = "Batches";
            }
        }

        namespace Schedules
        {
            public class SchedulesModule : ModuleDescription
            {
                public const string ExpectedComments = "<p><strong>These are schedules yo!</strong></p>";
                public SchedulesModule()
                {
                   Name = "Schedules";
                }
            }

            public class BatchScheduleRequest { public Guid Id { get; set; } }
            public class BatchScheduleResponse { }
            public class BatchScheduleGetAllHandler { public BatchScheduleResponse Execute(BatchScheduleRequest request) { return null; } }
            public class BatchSchedulePostHandler { public BatchScheduleResponse Execute(BatchScheduleRequest request) { return null; } }
            public class BatchScheduleGetHandler { public BatchScheduleResponse Execute_Id(BatchScheduleRequest request) { return null; } }
            public class BatchSchedulePutHandler { public BatchScheduleResponse Execute_Id(BatchScheduleRequest request) { return null; } }
            public class BatchScheduleDeleteHandler { public BatchScheduleResponse Execute_Id(BatchScheduleRequest request) { return null; } }
        }

        namespace Cells
        {
            public class BatchCellResource : ResourceDescription
            {
                public BatchCellResource()
                {
                    Name = "Batch cells";
                    Comments = "These are batch cells yo!";
                }
            }

            public class BatchCellRequest { public Guid Id { get; set; } }
            public class BatchCellResponse { }
            public class BatchCellGetAllHandler { public BatchCellResponse Execute(BatchCellRequest request) { return null; } }
            public class BatchCellPostHandler { public BatchCellResponse Execute(BatchCellRequest request) { return null; } }
            public class BatchCellGetHandler { public BatchCellResponse Execute_Id(BatchCellRequest request) { return null; } }
            public class BatchCellPutHandler { public BatchCellResponse Execute_Id(BatchCellRequest request) { return null; } }
            public class BatchCellDeleteHandler { public BatchCellResponse Execute_Id(BatchCellRequest request) { return null; } }
        }
    }

    namespace Administration
    {
        public class AdministrationModule : ModuleDescription
        {
            public AdministrationModule()
            {
                Name = "Administration";
                Comments = "This is admin yo!";
            }
        }

        namespace Users
        {
            // The following resource markers are named so that they are alpha ordered in
            // a particular way. This is important to a couple of tests so don't change that.

            public class AdminAccountResource : ResourceDescription<AdminAccountGetAllHandler>
            {
                public AdminAccountResource()
                {
                    Name = "Accounts";
                    Comments = "These are accounts yo!";
                }
            }

            public class AdminAccountRequest 
            { 
                public Guid Id { get; set; }
                public Guid UserId { get; set; }
                [QueryString, DefaultValue(Sort.Desc)]
                public Sort Order { get; set; }
                [QueryString]
                public List<Guid> Show { get; set; } 
            }

            public class AdminAccountResponse { }
            public class AdminAccountGetAllHandler { public AdminAccountResponse Execute_UserId(AdminAccountRequest request) { return null; } }
            public class AdminAccountPostHandler { public AdminAccountResponse Execute_UserId(AdminAccountRequest request) { return null; } }
            public class AdminAccountGetHandler { public AdminAccountResponse Execute_UserId_Id(AdminAccountRequest request) { return null; } }
            public class AdminAccountPutHandler { public AdminAccountResponse Execute_UserId_Id(AdminAccountRequest request) { return null; } }
            public class AdminAccountDeleteHandler { public AdminAccountResponse Execute_UserId_Id(AdminAccountRequest request) { return null; } }

            public class AdminAddressResource : ResourceDescription
            {
                public AdminAddressResource()
                {
                    Name = "User addresses";
                    Comments = "These are user addresses yo!";
                }
            }

            public enum AddressType
            {
                [Description("Work Address", "This is the work address of the user.")]
                Work,
                [Description("Home address", "This is the home address of the user.")]
                Home,
                Emergency
            }

            public enum Sort { Asc, Desc }

            public class AdminAddressRequest
            {
                [Description("Id", "This is the id.")]
                public Guid Id { get; set; }
                public Sort Order { get; set; }
                [Description("User Id", "This is the id of the user.")]
                public Guid UserId { get; set; } 
                public AddressType AddressType { get; set; }
            }

            public class AdminAddressResponse { }
            public class AdminAddressGetAllHandler
            {
                [ErrorDescription(411, "Swank address")]
                [ErrorDescription(410, "Invalid address", "An invalid address was entered fool!")]
                public AdminAddressResponse Execute_UserId(AdminAddressRequest request) { return null; }
            }
            public class AdminAddressGetAllOfTypeHandler { public AdminAddressResponse Execute_UserId_AddressType(AdminAddressRequest request) { return null; } }
            public class AdminAddressPostHandler { public AdminAddressResponse Execute_UserId(AdminAddressRequest request) { return null; } }
            public class AdminAddressGetHandler { public AdminAddressResponse Execute_UserId_Id(AdminAddressRequest request) { return null; } }
            public class AdminAddressPutHandler { public AdminAddressResponse Execute_UserId_Id(AdminAddressRequest request) { return null; } }
            public class AdminAddressDeleteHandler { public AdminAddressResponse Execute_UserId_Id(AdminAddressRequest request) { return null; } }

            public class AdminUserResource : ResourceDescription<AdminUserGetAllHandler>
            {
                public AdminUserResource()
                {
                    Name = "Users";
                    Comments = "These are users yo!";
                }
            }

            public class AdminUserRequest { public Guid Id { get; set; } }
            public class AdminUserResponse { }
            public class AdminUserGetAllHandler { public AdminUserResponse Execute(AdminUserRequest request) { return null; } }
            public class AdminUserPostHandler { public AdminUserResponse Execute(AdminUserRequest request) { return null; } }
            public class AdminUserGetHandler { public AdminUserResponse Execute_Id(AdminUserRequest request) { return null; } }
            public class AdminUserPutHandler { public AdminUserResponse Execute_Id(AdminUserRequest request) { return null; } }
            public class AdminUserDeleteHandler { public AdminUserResponse Execute_Id(AdminUserRequest request) { return null; } }
        }
    }
}