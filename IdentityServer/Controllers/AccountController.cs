using IdentityServer.views;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using pw.Commons;
using pw.Commons.Services;
using pw.Commons.Utils;
using Standard.Licensing.Validation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.Controllers
{
    public class AccountController: Controller
    {
        private readonly IIdentityServerInteractionService interaction;
        private readonly IConfiguration configuration;

        public AccountController(
            IIdentityServerInteractionService interaction, IConfiguration configuration
        )
        {
            this.interaction = interaction;
            this.configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            try
            {
                var vfs = ValidateLicense();
                if (vfs.Any())
                {
                    var licenseExpiredModel = new LicenseExpiredModel();
                    licenseExpiredModel.ValidationFailures = vfs;
                    return View("LicenseExpired", licenseExpiredModel);
                }

                return View(new LoginViewModel() { ReturnUrl = returnUrl });
            }
            catch(Exception ex) {
                var licenseExpiredModel = new LicenseExpiredModel();
                if (ex is FileNotFoundException)
                {
                    licenseExpiredModel.ErrorMessage = "License file not found!";
                }
                else {
                    licenseExpiredModel.ErrorMessage = ex.Message;
                }               
                
                return View("LicenseExpired", licenseExpiredModel);
            }
        }

        private List<IValidationFailure> ValidateLicense()
        {
            var licenseHelper = new LicenseHelper();
            return licenseHelper.ValidateLicense();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginData)
        {
            if (ModelState.IsValid)
            {
                var conn = DbHelper.getPearlsDbConn();
                var user = await (new TblUserService(conn)).GetUser(loginData.Username, loginData.Password);
                if (user == null)
                {
                    return View(loginData);
                }
                if (user.Locked)
                {
                    //show locked message
                    ModelState.AddModelError(string.Empty, user.LockMsg);
                    return View(loginData);
                }
                //success
                var userClaims = new List<Claim>
                {
                    new Claim("name", user.UserID.ToString()),
                    new Claim("sub", user.UserNo.ToString()),
                    new Claim("shakhaId", user.ShakhaID.ToString()),
                    new Claim("userNo", user.UserNo.ToString()),
                };

                var claimsIdentity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    null);
                TblLogService.InsertLog("Account", "Login", loginData.Username);
                return Redirect(loginData.ReturnUrl);
            }

            ModelState.AddModelError(string.Empty, "Incorrect username or password.");

            //failed..show login screen again. send loginData to ensure redirectUrl is not lost.
            return View(loginData);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var claims = User.Claims;
            var userName = claims.FirstOrDefault(p => p.Type == "name").Value;

            var conn = DbHelper.getPearlsDbConn();
            var user = (new TblUserService(conn)).UpdateUserActiveStatus(userName, false);

            await HttpContext.SignOutAsync();
            var context = await interaction.GetLogoutContextAsync(logoutId);
            return Redirect(context.PostLogoutRedirectUri);
        }
    }
}
