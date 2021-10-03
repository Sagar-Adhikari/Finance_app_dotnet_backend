using System.Collections.Generic;

namespace IdentityServer.views
{
    public class LicenseExpiredModel
    {
        public string ErrorMessage { get; set; }
        public List<Standard.Licensing.Validation.IValidationFailure> ValidationFailures { get; set; }
    }
}
