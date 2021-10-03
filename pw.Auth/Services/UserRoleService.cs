using pw.Auth.DAL;
using pw.Commons.Models;

namespace pw.Auth.Services
{
    public interface IUserRoleService : IBaseService<UserRolesModel>
    {
    }

    public class UserRoleService : BaseService<UserRolesModel>, IUserRoleService
    {
        public UserRoleService(AuthDbContext dbContext) : base(dbContext)
        {
        }
    }
}