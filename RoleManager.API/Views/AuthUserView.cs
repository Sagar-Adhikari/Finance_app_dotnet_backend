using IdentityServer.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoleManager.API.Views
{
    public class AuthUserView
    {
        public AuthUserView(ApplicationUser user, List<string> roles, UserDetailView userDetail = null)
        {
            this.Id = user.Id;
            this.Username = user.UserName;
            this.Email = user.Email;
            this.Roles = roles;
            this.FirstName = user.Firstname;
            this.LastName = user.Lastname;
            this.UserDetail = userDetail;
        }

        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }

        public UserDetailView UserDetail { get; set; }
    }
}
