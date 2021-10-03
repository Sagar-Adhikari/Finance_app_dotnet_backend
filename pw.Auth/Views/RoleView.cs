using pw.Commons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pw.Auth.Views
{
    public class RoleView
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public RoleView() { }

        public RoleView(TblRoleModel role)
        {
            this.Id = role.RoleId.ToString();
            this.Name = role.RoleName;
        }
    }
}
