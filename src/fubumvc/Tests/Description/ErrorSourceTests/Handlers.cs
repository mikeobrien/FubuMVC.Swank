using Swank.Description;

namespace Tests.Description.ErrorSourceTests
{
    namespace Administration
    {
        namespace Users
        {
            public class AdminAddressGetAllHandler
            {
                [ErrorDescription(411, "Swank address")]
                [ErrorDescription(410, "Invalid address", "An invalid address was entered fool!")]
                public object Execute(object request) { return null; }
            }
        }
    }
}