using pw.Commons.Models;
using System.Collections.Generic;

namespace pwAuth.Views
{
    public class AuthUserView
    {
        public AuthUserView(TblUsersModel tblUserModel)
        {
            this.Id = tblUserModel.UserNo;
            this.Username = tblUserModel.UserID;
            if (tblUserModel.ShakhaID != null)
            {
                this.ShakhaId = tblUserModel.ShakhaID.Value;
            }

            this.FirstName = tblUserModel.FName;
            this.LastName = tblUserModel.Lname;
            this.Locked = tblUserModel.Locked;
            this.Post = tblUserModel.Post;
            //this.Shakha = tblUserModel.Shakha;
        }

        public AuthUserView(TblUserMinView tblUserModel)
        {
            this.Id = tblUserModel.UserNo;
            this.Username = tblUserModel.UserID;
            if (tblUserModel.ShakhaID != null)
            {
                this.ShakhaId = tblUserModel.ShakhaID.Value;
            }
            
            this.FirstName = tblUserModel.FName;
            this.LastName = tblUserModel.Lname;
            this.Locked = tblUserModel.Locked;
            this.Post = tblUserModel.Post;
            this.Shakha = tblUserModel.ShakhaName;
        }
        public bool Locked { get; set; }
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Post { get; set; }
        public int ShakhaId { get; set; }
        public string Shakha { get; set; }
        public List<string> Roles { get; set; }
    }
}
