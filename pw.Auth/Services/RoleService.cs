using pw.Auth.DAL;
using pw.Auth.Views;
using pw.Commons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pw.Auth.Services
{
    public interface IRoleService : IBaseService<TblRoleModel> 
    {
        Task<List<string>> GetRolesForUser(int userNo);
        Task<List<RoleTaskView>> GetTasksForRole(int roleId);
        Task<bool> UpdateTasksForRole(List<RoleTaskView> roleTaskViews);
    }

    public class RoleService : BaseService<TblRoleModel>, IRoleService
    {
        public RoleService(AuthDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<string>> GetRolesForUser(int userNo)
        {
            var userRoles = _context.UserRoles.Where(p => p.UserNo == userNo);

            if (userRoles.Any())
            {
                var roles = await this.GetAll();

                var userRoleNames = (from a in userRoles.ToList()
                                 join b in roles on a.RoleId equals b.RoleId
                                 select b).ToList();
                if (userRoleNames.Count > 0)
                {
                    return userRoleNames.Select(p => p.RoleName).ToList();
                }
            }
            return null;
        }

        public async Task<List<RoleTaskView>> GetTasksForRole(int roleId)
        {
           var selectedTaskIds = (from a in _context.UserRoleTasks
                    join b in _context.Tasks on a.TaskId equals b.TaskId
                    where a.RoleId == roleId
                    select b.TaskId).ToList();

            var tasks = _context.Tasks.Select(p => new { p.TaskId, p.TaskName}).ToList();
            var ls = new List<RoleTaskView>();

            tasks.ForEach(task => {
                bool selected = selectedTaskIds.Contains(task.TaskId);
                var roleTaskView = new RoleTaskView(roleId, task.TaskId, task.TaskName, selected);
                ls.Add(roleTaskView);
            });
            return ls;
        }

        public async Task<bool> UpdateTasksForRole(List<RoleTaskView> roleTaskViews)
        {
            if (roleTaskViews.Count == 0) return false;

            var roleId = roleTaskViews[0].RoleId;

            //first remove all tasks for this role
            var toRemove = _context.UserRoleTasks.Where(p => p.RoleId == roleId);
            if (toRemove.Any())
            {
                _context.UserRoleTasks.RemoveRange(toRemove);
            }

            //now add all selected tasks for this role
            var lsToAdd = roleTaskViews.Where(p => p.IsSelected).Select(p => new TblAuthModel() {
                TaskId = p.TaskId,
                RoleId = roleId,
                UserNo = 0,
            }).ToList();

            _context.UserRoleTasks.AddRange(lsToAdd);
            _context.SaveChanges();
            return true;
        }
    }
}
