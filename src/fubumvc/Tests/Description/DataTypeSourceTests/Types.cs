using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Swank.Description;

namespace Tests.Description.DataTypeSourceTests
{
    [Comments("These are addresses yo!")]
    public class AdminAddresses : List<AdminAddressResponse> {}

    [Comments("This is an address request yo!")]
    public class AdminAddressRequest { }

    public class AdminAddressResponse { }

    [XmlType("User")]
    public class AdminUserRequest { public Guid Id { get; set; } }
    public class AdminUserResponse { } 

    [Comments("These are users yo!"), XmlType("Users")]
    public class AdminUsers : List<AdminUserResponse> { }
}