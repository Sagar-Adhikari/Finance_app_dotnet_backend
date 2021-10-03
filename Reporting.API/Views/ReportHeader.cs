
using pw.Commons.Utils;
using System;
using System.Linq;

namespace Reporting.API.Views
{
    public class ReportHeader
    {
        public ReportHeader(LicensedClientModel client, string reportTitle, string fromDate, string toDate)
        {
            this.OrganizationName = client.CLIENT_NAME;
            this.Address = client.CLIENT_ADDRESS;
            this.Phone = client.PHONE_NO;
            this.RegdNo = client.CLIENT_REGNO;

            this.ReportTitle = reportTitle;
            this.FromDate = fromDate;
            this.ToDate = toDate;            
        }
        public string OrganizationName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string RegdNo { get; set; }
        public string ReportTitle { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public static ReportHeader Create(string reportTitle, string minDate, string maxDate)
        {
            var licenseHelper = new LicenseHelper();
            var vfs = licenseHelper.ValidateLicense();

            if (vfs.Any() || vfs.Count > 0)
            {
                throw new Exception("Software license validation error!");
            }

            var licensedClient = licenseHelper.GetLicensedClient();
            if (licensedClient == null)
            {
                throw new Exception("Software license not found!");
            }

            var reportHeader = new ReportHeader(licensedClient, reportTitle, minDate, maxDate);
            return reportHeader;
        }
    }    
}
