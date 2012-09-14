using System;
using Swank.Description;

namespace Tests.Description.ParameterSourceTests
{
    namespace Administration
    {
        namespace Users
        {
            public enum AddressType { }

            public class AdminAddressRequest
            {
                [Comments("This is the id of the user.")]
                public Guid UserId { get; set; }
                public AddressType AddressType { get; set; }
            }

            public class AdminAddressGetAllOfTypeHandler { public object Execute_AddressType(AdminAddressRequest request) { return null; } }
        }
    }
}