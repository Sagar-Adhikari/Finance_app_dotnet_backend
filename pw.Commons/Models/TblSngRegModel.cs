
using System;

namespace pw.Commons.Models
{
    public class TblSngRegModel
    {
        public int SN { get; set; }
        public int RegCode { get; set; }
        public DateTime DatedE { get; set; }
        public string RegValue { get; set; }
        public string Remarks { get; set; }
    }

    public class RegView
    {
        public RegView(TblSngRegModel tblModel)
        {
            this.RegCode = tblModel.RegCode;
            this.RegValue = tblModel.RegValue;
        }
        public int RegCode { get; set; }
        public string RegValue { get; set; }
    }
}
