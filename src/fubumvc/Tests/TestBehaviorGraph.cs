using System;
using FubuMVC.Core.Registration;
using Swank;
using Tests.Administration.Users;
using Tests.Batches.Cells;
using Tests.Batches.Schedules;
using Tests.Templates;

namespace Tests
{
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
                .AddAction<AdminUserGetAllHandler>("/admin/users", HttpVerbs.Get)
                .AddAction<AdminUserPostHandler>("/admin/users", HttpVerbs.Post)
                .AddAction<AdminUserGetHandler>("/admin/users/{Id}", HttpVerbs.Get)
                .AddAction<AdminUserPutHandler>("/admin/users/{Id}", HttpVerbs.Put)
                .AddAction<AdminUserDeleteHandler>("/admin/users/{Id}", HttpVerbs.Delete)
                .AddAction<AdminAddressGetAllHandler>("/admin/users/{UserId}/addresses", HttpVerbs.Get)
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
    }

    namespace Templates
    {
        public class TemplateRequest { public Guid Id { get; set; } }
        public class TemplateResponse { }
        public class TemplateGetAllHandler { public TemplateResponse Execute(TemplateRequest request) { return null; } }
        public class TemplatePostHandler { public TemplateResponse Execute(TemplateRequest request) { return null; } }
        public class TemplateGetHandler { public TemplateResponse Execute_Id(TemplateRequest request) { return null; } }
        public class TemplatePutHandler { public TemplateResponse Execute_Id(TemplateRequest request) { return null; } }
        public class TemplateDeleteHandler { public TemplateResponse Execute_Id(TemplateRequest request) { return null; } }
    }

    namespace Administration
    {
        public class AdministrationModule : ModuleDescription
        {
            public AdministrationModule() : base("Administration", "This is admin yo!") { }
        }

        namespace Users
        {
            public class AdminAccountResource : ResourceDescription<AdminAccountGetAllHandler>
            {
                public AdminAccountResource() : base("Accounts", "These are accounts yo!") { }
            }

            public class AdminAccountRequest { public Guid Id { get; set; } public Guid UserId { get; set; } }
            public class AdminAccountResponse { }
            public class AdminAccountGetAllHandler { public AdminAddressResponse Execute_UserId(AdminAddressRequest request) { return null; } }
            public class AdminAccountPostHandler { public AdminAddressResponse Execute_UserId(AdminAddressRequest request) { return null; } }
            public class AdminAccountGetHandler { public AdminAddressResponse Execute_UserId_Id(AdminAddressRequest request) { return null; } }
            public class AdminAccountPutHandler { public AdminAddressResponse Execute_UserId_Id(AdminAddressRequest request) { return null; } }
            public class AdminAccountDeleteHandler { public AdminAddressResponse Execute_UserId_Id(AdminAddressRequest request) { return null; } }

            public class AdminAddressResource : ResourceDescription<AdminAddressGetAllHandler>
            {
                public AdminAddressResource() : base("User addresses", "These are user addresses yo!") { }
            }

            public class AdminAddressRequest { public Guid Id { get; set; } public Guid UserId { get; set; } }
            public class AdminAddressResponse { }
            public class AdminAddressGetAllHandler { public AdminAddressResponse Execute_UserId(AdminAddressRequest request) { return null; } }
            public class AdminAddressPostHandler { public AdminAddressResponse Execute_UserId(AdminAddressRequest request) { return null; } }
            public class AdminAddressGetHandler { public AdminAddressResponse Execute_UserId_Id(AdminAddressRequest request) { return null; } }
            public class AdminAddressPutHandler { public AdminAddressResponse Execute_UserId_Id(AdminAddressRequest request) { return null; } }
            public class AdminAddressDeleteHandler { public AdminAddressResponse Execute_UserId_Id(AdminAddressRequest request) { return null; } }

            public class AdminUserResource : ResourceDescription
            {
                public AdminUserResource() : base("Users", "These are users yo!") { }
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

    namespace Batches
    {
        public class BatchesModule : ModuleDescription
        {
            public BatchesModule() : base("Batches") { }
        }

        namespace Cells
        {
            public class BatchCellResource : ResourceDescription
            {
                public BatchCellResource() : base("Batch cells", "These are batch cells yo!") { }
            }

            public class BatchCellRequest { public Guid Id { get; set; } }
            public class BatchCellResponse { }
            public class BatchCellGetAllHandler { public BatchCellResponse Execute(BatchCellRequest request) { return null; } }
            public class BatchCellPostHandler { public BatchCellResponse Execute(BatchCellRequest request) { return null; } }
            public class BatchCellGetHandler { public BatchCellResponse Execute_Id(BatchCellRequest request) { return null; } }
            public class BatchCellPutHandler { public BatchCellResponse Execute_Id(BatchCellRequest request) { return null; } }
            public class BatchCellDeleteHandler { public BatchCellResponse Execute_Id(BatchCellRequest request) { return null; } }
        }

        namespace Schedules
        {
            public class SchedulesModule : ModuleDescription
            {
                public SchedulesModule() : base("Schedules") { }
            }

            public class BatchScheduleRequest { public Guid Id { get; set; } }
            public class BatchScheduleResponse { }
            public class BatchScheduleGetAllHandler { public BatchScheduleResponse Execute(BatchScheduleRequest request) { return null; } }
            public class BatchSchedulePostHandler { public BatchScheduleResponse Execute(BatchScheduleRequest request) { return null; } }
            public class BatchScheduleGetHandler { public BatchScheduleResponse Execute_Id(BatchScheduleRequest request) { return null; } }
            public class BatchSchedulePutHandler { public BatchScheduleResponse Execute_Id(BatchScheduleRequest request) { return null; } }
            public class BatchScheduleDeleteHandler { public BatchScheduleResponse Execute_Id(BatchScheduleRequest request) { return null; } }
        }
    }
}