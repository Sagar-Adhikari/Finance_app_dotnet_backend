using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoleManager.API.Views
{
    public class SignupView
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string Password { get; set; }
        public string Post { get; set; }

    }
}
