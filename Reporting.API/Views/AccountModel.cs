namespace Reporting.API.Views
{
    public class NewSubAccountModel
    {
        public int AccID { get; set; }
        public string SubAccName { get; set; }
        public string SubAccNameDev { get; set; }
        public string SubAccAdd { get; set; }
        public string Contact { get; set; }
        public int AccTypeCat { get; set; }
        public bool Hidden { get; set; }
        public bool DiffACAuto { get; set; }
        public int DiffACAutoASN { get; set; }
        public bool BillToNonAcName { get; set; }
        public bool IncMem { get; set; }//share members
        public bool IncGMem { get; set; }
        public bool IncMemCat { get; set; }
        public bool IncMemType { get; set; }
        public bool SubAcIDWala { get; set; }
        public int DefByjAccSN { get; set; } //Int. ASN
    }
}
