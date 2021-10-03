using Microsoft.AspNetCore.Mvc;
using RoleManager.API.Views;

namespace RoleManager.API.Controllers
{
    public class BaseController : ControllerBase
    {
        protected IActionResult serverError()
        {
            return StatusCode(500);
        }

        protected IActionResult unauthorized()
        {
            return Unauthorized();
        }

        protected IActionResult conflict(string msg)
        {
            return Conflict(new ApiError(msg));
        }

        protected IActionResult badRequest(string msg)
        {
            return BadRequest(new ApiError(msg));
        }

        protected OkObjectResult result(object data)
        {
            return Ok(data);
        }
    }
}
