using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pw.Auth.Services;
using pw.Commons.Middlewares;
using pw.Commons.Models;
using System.Linq;
using System.Threading.Tasks;

namespace pw.Auth.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    public class TaskListController: Controller
    {
        private readonly ITaskListService taskListService;

        public TaskListController(ITaskListService taskListService)
        {
            this.taskListService = taskListService;
        }

        [HttpGet("getTask")]
        public async Task<IActionResult> GetTask(int taskId)
        {
            var resp = await taskListService.Find(p => p.TaskId == taskId);

            return Ok(resp);
        }

        [HttpGet("getTaskList")]
        public async Task<IActionResult> GetTskList()
        {
            var resp = await taskListService.GetAll();

            return Ok(resp);
        }

        [HttpPost("upsertTask")]
        [AuditAction]
        public async Task<IActionResult> UpsertTask(TblTaskListModel tblTaskListModel)
        {
            var existsCheck = await taskListService.Find(p => p.TaskName.ToLower().Equals(tblTaskListModel.TaskName.ToLower()));

            if (tblTaskListModel.TaskId <= 0)
            {
                //create mode

                //check if same task name is already used somewhere...
                if (existsCheck != null) return Conflict("Error! Same named task already exist in table.");

                var taskMaxId = (await taskListService.GetAll()).OrderByDescending(p => p.TaskId).FirstOrDefault();
                tblTaskListModel.TaskId = taskMaxId == null ? 1 : taskMaxId.TaskId + 1;

                await taskListService.Add(tblTaskListModel);
                return Ok(true);
            }

            //update mode

            //check if same task name is already used somewhere in another task id...
            if (existsCheck != null && existsCheck.TaskId != tblTaskListModel.TaskId)
            {
                return Conflict("Error! Same named task already exist in table.");
            }

            var resp = await taskListService.Update(tblTaskListModel, tblTaskListModel.TaskId);
            return Ok(true);
        }
    }
}
