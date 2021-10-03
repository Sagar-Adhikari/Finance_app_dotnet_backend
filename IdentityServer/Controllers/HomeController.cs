using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService identityServerInteractionService;

        public HomeController(
            IIdentityServerInteractionService identityServerInteractionService)
        {
            this.identityServerInteractionService = identityServerInteractionService;
        }

        public async Task<IActionResult> Error(string errorId)
        {
            var errormessage = await identityServerInteractionService.GetErrorContextAsync(errorId);
            return View(errormessage);
        }
    }
}