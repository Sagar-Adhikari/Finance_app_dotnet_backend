using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pw.Auth.Services;
using pw.Auth.Views;
using pw.Commons.Middlewares;

namespace pw.Auth.Controllers
{
    [ApiController]
    //todo: check role for auth
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]

    public class RoleController : Controller
    {
        private readonly IRoleService roleService;

        public RoleController(IRoleService roleService)
        {
            this.roleService = roleService;
        }

        [HttpPost("createNewRole")]
        [AuditAction]
        public async Task<IActionResult> createNewRole([FromBody] RoleView roleData)
        {
            var roleExists = await roleService.Find(p => p.RoleName.ToLower() == roleData.Name.ToLower());
            if (roleExists != null)
            {
                return Conflict($"Role {roleData.Name} Already Exists");
            }

            var roleMaxId = (await roleService.GetAll()).OrderByDescending(p => p.RoleId).FirstOrDefault();

            var role = new Commons.Models.TblRoleModel()
            {
                RoleName = roleData.Name,
                RoleId = roleMaxId == null ? 1 : roleMaxId.RoleId + 1
            };

            var resp = await roleService.Add(role);

            return Ok(new RoleView(resp));
        }

        [HttpGet("getAllRoles")]
        public async Task<IActionResult> geAllRoles()
        {
            var roles = (await roleService.GetAll()).ToList().Select(role => new RoleView(role));

            return Ok(roles);
        }

        [HttpPost("deleteRole")]
        [AuditAction]
        public async Task<IActionResult> deleteRole([FromBody] RoleView roleData)
        {
            //todo: check if has auth roles assigned here...
            var role = await roleService.Find(p => p.RoleName.ToLower() == roleData.Name.ToLower());
            if (role == null)
            {
                return Conflict($"Role {roleData.Name} does not exist");
            }

            var res = await roleService.Delete(role);

            if (res > 0)
            {
                return Ok();
            }
            return Conflict("Unable to delete role");
        }

        [HttpGet("getTasksForRole")]
        public async Task<IActionResult> getTasksForRole(int roleId)
        {
            var tasks = await roleService.GetTasksForRole(roleId);
            return Ok(tasks);
        }

        [HttpPost("updateTasksForRole")]
        [AuditAction]
        public async Task<IActionResult> updateTasksForRole([FromBody] List<RoleTaskView> roleTaskViews)
        {
            var tasks = await roleService.UpdateTasksForRole(roleTaskViews);
            return Ok(tasks);
        }

    }
}