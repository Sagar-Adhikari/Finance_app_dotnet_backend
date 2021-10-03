using pw.Auth.DAL;
using pw.Commons.Models;

namespace pw.Auth.Services
{
    public interface ITaskListService : IBaseService<TblTaskListModel> { }

    public class TaskListService : BaseService<TblTaskListModel>, ITaskListService
    {
        public TaskListService(AuthDbContext dbContext) : base(dbContext)
        {
        }

    }
}
