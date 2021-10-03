using pw.Auth.DAL;
using pw.Commons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pw.Auth.Services
{
    public interface ITblLogService : IBaseService<TblLogModel> { }

    public class TblLogService : BaseService<TblLogModel>, ITblLogService
    {
        private readonly AuthDbContext dbContext;

        public TblLogService(AuthDbContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }

    }
}
