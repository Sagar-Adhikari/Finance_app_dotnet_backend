using IdentityServer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoleManager.API.Views
{
    public class UserDetailView
    {
        public UserDetailView() { }

        public UserDetailView(UserDetail userDetail)
        {
            this.Post = userDetail.Post;
            this.PostDev = userDetail.PostDev;
            this.Permission = userDetail.Permission;
            this.Status = userDetail.Status;
            this.Locked = userDetail.Locked;
            this.LockMsg = userDetail.LockMsg;
            this.FullNameDev = userDetail.FullNameDev;
            this.LockMsgDev = userDetail.LockMsgDev;
            this.DrLimit = userDetail.DrLimit;
            this.CrLimit = userDetail.CrLimit;
            this.Stackverifier = userDetail.Stackverifier;
            this.Comp1 = userDetail.Comp1;
            this.Comp2 = userDetail.Comp2;
            this.Hidden = userDetail.Hidden;
            this.ShakhaId = userDetail.ShakhaId;
        }

        public string Post { get; set; }
        public string PostDev { get; set; }
        public int Permission { get; set; }
        public bool Status { get; set; }
        public bool Locked { get; set; }
        public string LockMsg { get; set; }
        public string FullNameDev { get; set; }
        public string LockMsgDev { get; set; }
        public double DrLimit { get; set; }
        public double CrLimit { get; set; }
        public bool Stackverifier { get; set; }
        public string Comp1 { get; set; }
        public string Comp2 { get; set; }
        public bool Hidden { get; set; }
        public int ShakhaId { get; set; }
    }
}
