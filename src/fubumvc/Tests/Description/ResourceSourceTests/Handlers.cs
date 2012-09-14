using System;
using System.Collections.Generic;
using Swank.Description;

namespace Tests.Description.ResourceSourceTests
{
    namespace Templates
    {
        public class TemplateAllGetHandler { public object Execute(object request) { return null; } }
    }

    namespace Administration
    {
        public class AdministrationModule : ModuleDescription { public AdministrationModule() { Name = "Administration"; } }

        namespace Users // These are ordered a certian way on purpose, don't change that.
        {
            public class AdminAccountResource : ResourceDescription<AdminAccountAllGetHandler>
            { public AdminAccountResource() { Name = "Accounts"; } }

            public class AdminAccountRequest { public Guid Id { get; set; } }
            public class AdminAccountResponse { }
            public class AdminAccountAllGetHandler { public AdminAccountResponse Execute_UserId(AdminAccountRequest request) { return null; } }
            public class AdminAccountPostHandler { public AdminAccountResponse Execute_UserId(AdminAccountRequest request) { return null; } }
            public class AdminAccountGetHandler { public AdminAccountResponse Execute_UserId_Id(AdminAccountRequest request) { return null; } }
            public class AdminAccountPutHandler { public AdminAccountResponse Execute_UserId_Id(AdminAccountRequest request) { return null; } }
            public class AdminAccountDeleteHandler { public AdminAccountResponse Execute_UserId_Id(AdminAccountRequest request) { return null; } }

            public class AdminAddressResource : ResourceDescription
            { public AdminAddressResource() { Name = "User addresses"; Comments = "These are user addresses yo!"; } }

            public class AdminAddressRequest { public Guid Id { get; set; } public Guid UserId { get; set; } public string AddressType { get; set; } }
            public class AdminAddressResponse { }
            public class AdminAddressAllGetHandler { public AdminAddressResponse Execute_UserId_Address(AdminAddressRequest request) { return null; } }
            public class AdminAddressAllOfTypeGetHandler { public AdminAddressResponse Execute_UserId_Address_AddressType(AdminAddressRequest request) { return null; } }
            public class AdminAddressPostHandler { public AdminAddressResponse Execute_UserId_Address(AdminAddressRequest request) { return null; } }
            public class AdminAddressGetHandler { public List<AdminAddressResponse> Execute_UserId_Address_Id(AdminAddressRequest request) { return null; } }
            public class AdminAddressPutHandler { public AdminAddressResponse Execute_UserId_Address_Id(AdminAddressRequest request) { return null; } }
            public class AdminAddressDeleteHandler { public AdminAddressResponse Execute_UserId_Address_Id(AdminAddressRequest request) { return null; } }

            public class AdminUserResource : ResourceDescription<AdminUserAllGetHandler>
            { public AdminUserResource() { Name = "Users"; Comments = "These are users yo!"; } }

            public class AdminUserRequest { public Guid Id { get; set; } }
            public class AdminUserResponse { }
            public class AdminUserAllGetHandler { public AdminUserResponse Execute(AdminUserRequest request) { return null; } }
            public class AdminUserPostHandler { public AdminUserResponse Execute(AdminUserRequest request) { return null; } }
            public class AdminUserGetHandler { public AdminUserResponse Execute_Id(AdminUserRequest request) { return null; } }
            public class AdminUserPutHandler { public AdminUserResponse Execute_Id(AdminUserRequest request) { return null; } }
            public class AdminUserDeleteHandler { public AdminUserResponse Execute_Id(AdminUserRequest request) { return null; } }
        }
    }
}