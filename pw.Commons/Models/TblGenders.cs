
namespace pw.Commons.Models
{
    public class TblGenders
    {
        public int GendID { get; set; }
        public string GendName { get; set; }
        public string GendNameDev { get; set; }
        public string Remarks { get; set; }
    }

    public class MembersCountByGender
    {
        public int Sex { get; set; }
        public string GendName { get; set; }
        public int Total { get; set; }
    }
}
