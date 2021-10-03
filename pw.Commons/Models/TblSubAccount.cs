namespace pw.Commons.Models
{
    public class AccountModel
    {
        public int AccID { get; set; }
        public int ActAccID { get; set; }
        public string AccName { get; set; }
        public string AccNameDev { get; set; }
    }
    public class SubAccountModel
    {
        public int AccSN { get; set; }
        public int SubAccID { get; set; }
        public int AccID { get; set; }
        public string SubAccName { get; set; }
    }
}
