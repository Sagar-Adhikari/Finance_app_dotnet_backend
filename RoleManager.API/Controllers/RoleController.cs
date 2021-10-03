using IdentityServer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoleManager.API.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoleManager.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "super")]
    [Route("api/[controller]")]

    public class RoleController : BaseController
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }

        [HttpPost("createNewRole")]
        public async Task<IActionResult> AddnewUser([FromBody] RoleView roleData)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleData.Name);
            if(roleExists)
            {
                return badRequest($"Role {roleData.Name} Already Exists");
            }
            var role = new IdentityRole(roleData.Name);
            await roleManager.CreateAsync(role);
            return result(new RoleView(role));
        }

        [HttpGet("getAllRoles")]
        public IActionResult geAllRoles()
        {
            var roles = roleManager.Roles.ToList().Select(role => new RoleView(role));

            return result(roles);
        }

        [HttpPost("deleteRole")]
        public async Task<IActionResult> deleteRole([FromBody] RoleView roleData)
        {
            var role = await roleManager.FindByIdAsync(roleData.Id);
            if (role == null)
            {
                return badRequest($"Role Does not Exist");
            }

            var res = await roleManager.DeleteAsync(role);
            if (res.Succeeded)
            {
                return result(new ApiResult($"Role {role.Name} deleted"));
            }
            return serverError();
        }

    }
}