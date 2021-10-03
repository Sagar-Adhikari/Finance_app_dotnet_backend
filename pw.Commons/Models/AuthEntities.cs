using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pw.Commons.Models
{
    public interface IEntity
    {

    }

    [Table("TblUsers")]
    public class TblUsersModel: IEntity
    {
        [Key]
        public int UserNo { get; set; }
        public string UserID { get; set; }
        public string UserPass { get; set; }
        public string FName { get; set; }
        public string Lname { get; set; }
        public string Post { get; set; }
        public byte Permission { get; set; }
        public bool Status { get; set; }
        public bool Locked { get; set; }
        public string LockMsg { get; set; }
        public byte? PerMissType { get; set; }
        public string FullNameDev { get; set; }
        public string PostDev { get; set; }
        public string LockMsgDev { get; set; }
        public float? DrLimit { get; set; }
        public float? CrLimit { get; set; }
        public bool? StackVerifier { get; set; }
        public string Comp1 { get; set; }
        public string Comp2 { get; set; }
        public bool? Hidden { get; set; }
        public byte? ShakhaID { get; set; }
    }

    [Table("TblAuth")]
    public class TblAuthModel: IEntity
    {
        [Key]
        public Int64 SN { get; set; }
        public int TaskId { get; set; }
        public int RoleId { get; set; }
        public int UserNo { get; set; }
    }

    [Table("TblTaskList")]
    public class TblTaskListModel: IEntity
    {
        [Key]
        public int TaskId { get; set; }
        public string TaskName { get; set; }
        public string Remarks { get; set; }
        public bool IsDeny { get; set; }
        public double OrdId { get; set; }
    }

    [Table("TblRoles")]
    public class TblRoleModel: IEntity
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Remarks { get; set; }
        public bool? Locked { get; set; }
        public string LockMsg { get; set; }
        public string LockMsgDev { get; set; }
        public float? DrLimit { get; set; }
        public float? CrLimit { get; set; }
    }

    [Table("UserRoles")]
    public class UserRolesModel : IEntity
    {
        [Key]
        public int UserNo { get; set; }
        [Key]
        public int RoleId { get; set; }
    }   
}
