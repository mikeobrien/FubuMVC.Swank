using Swank.Description;

namespace Tests.Description.OptionSourceTests
{
    namespace Administration
    {
        namespace Users
        {
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

            public class AdminAddressRequest { public AddressType AddressType { get; set; } }

            public class AdminAddressGetAllOfTypeHandler { public object Execute_AddressType(AdminAddressRequest request) { return null; } }
        }
    }
}