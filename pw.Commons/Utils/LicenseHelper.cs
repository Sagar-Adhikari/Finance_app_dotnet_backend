using Standard.Licensing;
using Standard.Licensing.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pw.Commons.Utils
{
    public class LicensedClientModel
    {
        public LicensedClientModel(LicenseAttributes licenseAttributes, DateTime expiry)
        {
            this.ClIENT_ID = licenseAttributes.Get("ClIENT_ID");
            this.CLIENT_NAME = licenseAttributes.Get("CLIENT_NAME");
            this.CLIENT_ADDRESS = licenseAttributes.Get("CLIENT_ADDRESS");
            this.CLIENT_REGNO = licenseAttributes.Get("CLIENT_REGNO");
            this.PHONE_NO = licenseAttributes.Get("PHONE_NO");
            this.EXP_DATE = expiry;
        }

        public string ClIENT_ID { get; set; }
        public string CLIENT_NAME { get; set; }
        public string CLIENT_ADDRESS { get; set; }
        public string CLIENT_REGNO { get; set; }
        public string PHONE_NO { get; set; }
        public DateTime EXP_DATE { get; set; }
    }

    public class LicenseHelper
    {
        private readonly string licensePath;

        public LicenseHelper()
        {
            this.licensePath = Environment.GetEnvironmentVariable("PEARLS_LICENSE_PATH");
        }

        public LicensedClientModel GetLicensedClient()
        {
            var license = GetLicense();
            if (license == null) return null;
            return new LicensedClientModel(license.AdditionalAttributes, license.Expiration);
        }

        public List<IValidationFailure> ValidateLicense()
        {
            var license = GetLicense();
            var publicKeyFilePath = licensePath + "public_key.txt";
            var publicKey = System.IO.File.ReadAllText(publicKeyFilePath);

            var validationFailures = license.Validate()
                                .ExpirationDate()
                                .When(lic => lic.Type == LicenseType.Standard)
                                .And()
                                .Signature(publicKey)
                                .AssertValidLicense();

            return validationFailures.ToList();
        }

        public License GetLicense()
        {
            var licenseFilePath = licensePath + "License.lic";

            var licenseXmlText = System.IO.File.ReadAllText(licenseFilePath);
            return License.Load(licenseXmlText);
        }        
    }
}
