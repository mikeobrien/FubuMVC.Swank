using System;
using System.Collections.Generic;
using Swank.Description;

namespace Tests.Description.ResourceSourceTests
{
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

    namespace Batches
    {
        public class BatchesModule : ModuleDescription { public BatchesModule() { Name = "Batches"; } }

        namespace Schedules
        {
            public class SchedulesModule : ModuleDescription { public SchedulesModule() { Name = "Schedules"; } }

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
            { public BatchCellResource() { Name = "Batch cells"; Comments = "These are batch cells yo!"; } }

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
        public class AdministrationModule : ModuleDescription { public AdministrationModule() { Name = "Administration"; } }

        namespace Users // These are ordered a certian way on purpose, don't change that.
        {
            public class AdminAccountResource : ResourceDescription<AdminAccountGetAllHandler>
            { public AdminAccountResource() { Name = "Accounts"; Comments = "These are accounts yo!"; } }

            public class AdminAccountRequest { public Guid Id { get; set; } }
            public class AdminAccountResponse { }
            public class AdminAccountGetAllHandler { public AdminAccountResponse Execute_UserId(AdminAccountRequest request) { return null; } }
            public class AdminAccountPostHandler { public AdminAccountResponse Execute_UserId(AdminAccountRequest request) { return null; } }
            public class AdminAccountGetHandler { public AdminAccountResponse Execute_UserId_Id(AdminAccountRequest request) { return null; } }
            public class AdminAccountPutHandler { public AdminAccountResponse Execute_UserId_Id(AdminAccountRequest request) { return null; } }
            public class AdminAccountDeleteHandler { public AdminAccountResponse Execute_UserId_Id(AdminAccountRequest request) { return null; } }

            public class AdminAddressResource : ResourceDescription
            { public AdminAddressResource() { Name = "User addresses"; Comments = "These are user addresses yo!"; } }

            public class AdminAddressRequest { public Guid Id { get; set; } public Guid UserId { get; set; } public string AddressType { get; set; } }
            public class AdminAddressResponse { }
            public class AdminAddressGetAllHandler { public AdminAddressResponse Execute_UserId(AdminAddressRequest request) { return null; } }
            public class AdminAddressGetAllOfTypeHandler { public AdminAddressResponse Execute_UserId_AddressType(AdminAddressRequest request) { return null; } }
            public class AdminAddressPostHandler { public AdminAddressResponse Execute_UserId(AdminAddressRequest request) { return null; } }
            public class AdminAddressGetHandler { public List<AdminAddressResponse> Execute_UserId_Id(AdminAddressRequest request) { return null; } }
            public class AdminAddressPutHandler { public AdminAddressResponse Execute_UserId_Id(AdminAddressRequest request) { return null; } }
            public class AdminAddressDeleteHandler { public AdminAddressResponse Execute_UserId_Id(AdminAddressRequest request) { return null; } }

            public class AdminUserResource : ResourceDescription<AdminUserGetAllHandler>
            { public AdminUserResource() { Name = "Users"; Comments = "These are users yo!"; } }

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