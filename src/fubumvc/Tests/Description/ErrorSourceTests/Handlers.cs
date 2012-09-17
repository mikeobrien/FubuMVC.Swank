using Swank.Description;

namespace Tests.Description.ErrorSourceTests
{
    namespace ErrorsTests
    {
        [ErrorDescription(411, "411 error on handler")]
        [ErrorDescription(410, "410 error on handler", "410 error on action description")]
        public class ErrorsGetHandler
        {
            [ErrorDescription(413, "413 error on action")]
            [ErrorDescription(412, "412 error on action", "412 error on action description")]
            public object Execute_Errors(object request) { return null; }
        }

        public class NoErrorsGetHandler
        {
            public object Execute_NoErrors(object request) { return null; }
        }
    }
}