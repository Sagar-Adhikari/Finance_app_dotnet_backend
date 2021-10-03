using pw.Auth.DAL;
using pw.Commons.Models;

namespace pw.Auth.Services
{
    public interface IUserTblService : IBaseService<TblUsersModel> { }

    public class UserTblService : BaseService<TblUsersModel>, IUserTblService
    {
        private readonly AuthDbContext dbContext;

        public UserTblService(AuthDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

    }
}
