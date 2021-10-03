namespace pw.Commons.Models
{
    public class TblUserMinView
    {
        public int UserNo { get; set; }
        public string UserID { get; set; }
        public string FName { get; set; }
        public string Lname { get; set; }
        public string Post { get; set; }
        public bool Locked { get; set; }
        public byte? ShakhaID { get; set; }
        public string ShakhaName { get; set; }
    }
}
