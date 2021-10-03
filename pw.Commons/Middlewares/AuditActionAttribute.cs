using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using pw.Commons.Services;
using System;
using System.Linq;
using System.Text;

namespace pw.Commons.Middlewares
{
    public class AuditActionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userNameClaim = context.HttpContext.User.Claims.FirstOrDefault(p => p.Type == "name");
            var userName = userNameClaim == null ? "" : userNameClaim.Value;

            var ctrlActionDescptor = context.ActionDescriptor as ControllerActionDescriptor;

            var actionName = ctrlActionDescptor.ActionName;
            var controllerName = ctrlActionDescptor.ControllerName;
            var actionText="";
            if (context.ActionArguments.Any())
            {
                actionText = "Action called with parameters: ";
                StringBuilder actionTextBuilder = new StringBuilder();
                foreach (var dict in context.ActionArguments) 
                {
                    actionTextBuilder.Append(Environment.NewLine);
                    var str = Newtonsoft.Json.JsonConvert.SerializeObject(dict);
                    actionTextBuilder.Append(str);
                }
                actionText += actionTextBuilder.ToString();
            }
            TblLogService.InsertLog(controllerName, actionName, userName, actionText: actionText);
        }
    }
}
