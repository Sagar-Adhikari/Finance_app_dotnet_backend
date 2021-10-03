using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace pw.Commons.Models
{
    [Table("tbllog")]
    public class TblLogModel : IEntity
    {
        [Key]
        public long SN { get; set; }
        public string DatedN { get; set; }
        public string MaxTranDate { get; set; }
        public long TranAN { get; set; }
        public string ModuleName { get; set; }
        public string ActionName { get; set; }
        public string ActionText { get; set; }
        public string UserID { get; set; }
        public Int16 LogType { get; set; }
        public string SysDate { get; set; }
    }
}
