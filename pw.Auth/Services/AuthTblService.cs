using pw.Auth.DAL;
using pw.Commons.Models;

namespace pw.Auth.Services
{
    public interface IAuthTblService : IBaseService<TblAuthModel> { }

    public class AuthTblService : BaseService<TblAuthModel>, IAuthTblService
    {
        private readonly AuthDbContext dbContext;

        public AuthTblService(AuthDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

    }
}
