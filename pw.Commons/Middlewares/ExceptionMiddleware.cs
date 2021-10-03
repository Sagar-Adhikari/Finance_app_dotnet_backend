using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using pw.Commons.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace pw.Commons.Middlewares
{
    public static class ExceptionsMiddlewareExtension
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }

    public class ExceptionMiddleware
    {
        //The _next parameter of RequestDeleagate type is a function delegate which can process our HTTP requests.
        private readonly RequestDelegate _next;
        
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        //After the registration process, we need to create the InvokeAsync() method. 
        //RequestDelegate can’t process requests without it.
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                //If everything goes well, 
                //the _next delegate should process the request and the action from our controller should generate 
                //the successful response.
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                //if a request is unsuccessful, 
                //our middleware will trigger the catch block and call the HandleExceptionAsync method.

                var userNameClaim = httpContext.User.Claims.FirstOrDefault(p => p.Type == "name");
                var userName = userNameClaim == null ? "" : userNameClaim.Value;

                TblLogService.InsertLog("Error!", ex.Message, userName, actionText: ex.StackTrace);
                throw ex;
            }
        }
    }
}
