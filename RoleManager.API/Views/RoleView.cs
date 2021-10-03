using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoleManager.API.Views
{
    public class RoleView
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public RoleView() { }

        public RoleView(IdentityRole role)
        {
            this.Id = role.Id;
            this.Name = role.Name;
        }
    }
}
