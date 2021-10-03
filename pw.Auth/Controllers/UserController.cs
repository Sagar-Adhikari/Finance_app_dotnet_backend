using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using pw.Auth.Services;
using pw.Auth.Views;
using pw.Commons;
using pw.Commons.Middlewares;
using pw.Commons.Models;
using pw.Commons.Services;
using pwAuth.Views;

namespace pw.Auth.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]

    public class UserController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IRoleService roleService;
        private readonly IAuthTblService authTblService;
        private readonly IUserTblService userTblService;
        private readonly IUserRoleService userRoleService;

        public UserController(
            IConfiguration configuration,
            IRoleService roleService,
            IAuthTblService authTblService,
            IUserTblService userTblService,
            IUserRoleService userRoleService)
        {
            this.configuration = configuration;
            this.roleService = roleService;
            this.authTblService = authTblService;
            this.userTblService = userTblService;
            this.userRoleService = userRoleService;
        }

        [HttpGet("getAuth")]
        public async Task<IActionResult> GetAuthUser()
        {
            var claims = User.Claims;
            var userNo = Convert.ToInt32(claims.FirstOrDefault(p => p.Type == "userNo").Value);

            var conn = DbHelper.getPearlsDbConn();
            var tblService = new TblUserService(conn);
            var user = await tblService.GetUser(userNo);
            tblService.UpdateUserActiveStatus(user.UserID, true);

            var authUser = new AuthUserView(user);

            var userRoles = await roleService.GetRolesForUser(userNo);

            if (userRoles != null && userRoles.Count > 0)
            {
                authUser.Roles = userRoles;
            }
            return Ok(authUser);
        }

        [HttpGet("getUsers")]
        public async Task<IActionResult> GetUsersAsync([FromQuery] int page = 0, int limit = 20)
        {
            var allUsers = await userTblService.GetAll();
            var total = allUsers.AsEnumerable().Count();

            var paginatedUsers = allUsers.Skip(page * limit).Take(limit).ToList();

            List<AuthUserView> users = new List<AuthUserView>();

            foreach (var user in paginatedUsers)
            {
                var u = new AuthUserView(user);

                var userRoles = await roleService.GetRolesForUser(user.UserNo);

                if (userRoles != null && userRoles.Count > 0)
                {
                    u.Roles = userRoles;
                }
                users.Add(u);
            }

            return Ok(new PaginationView<AuthUserView>()
            {
                Total = total,
                Page = page,
                Contents = users
            });
        }

        [HttpPost("createNewUser")]
        [AuditAction]
        public async Task<IActionResult> AddnewUser([FromBody] SignupView signupData)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Please enter valid values.");
                return Conflict("Please enter valid values.");
            }

            //check valid username

            var userExists = await userTblService.Find(p => p.UserID == signupData.UserName);
            if (userExists != null)
            {
                return Conflict("Username already taken.");
            }

            var user = new TblUsersModel()
            {
                UserID = signupData.UserName,
                UserPass = signupData.Password,
                Post = signupData.Post,
                FName = signupData.FirstName,
                Lname = signupData.LastName,
                ShakhaID = (byte)signupData.ShakhaId,
                Locked = false, //todo:
                Status = false, //todo:?
                Permission = 1 //todo:?                
            };

            var resp = await userTblService.Add(user);
            if (resp == null)
            {
                //server error
                return StatusCode(500);
            }

            var authUser = new AuthUserView(resp);

            if (signupData.Roles != null && signupData.Roles.Count > 0)
            {
                var lsUserRole = new List<UserRolesModel>();
                foreach (var item in signupData.Roles)
                {
                    var role = await roleService.Find(p => p.RoleName == item);
                    lsUserRole.Add(new UserRolesModel() { RoleId = role.RoleId, UserNo = resp.UserNo });
                }

                await userRoleService.AddRange(lsUserRole);

                authUser.Roles = signupData.Roles;
            }

            return Ok(authUser);
        }

    }
}


//        [HttpPost("updateUser/{userId}")]
//        public async Task<IActionResult> UpdateUser(string userId, [FromBody] SignupView updateData)
//        {
//            var user = await userManager.FindByIdAsync(userId);

//            var roles = await userManager.GetRolesAsync(user);

//            if(user.Email != updateData.Email)
//            {
//                await userManager.RemoveClaimAsync(user, new System.Security.Claims.Claim("email", user.Email));
//                await userManager.AddClaimAsync(user, new System.Security.Claims.Claim("email", updateData.Email));
//                user.Email = updateData.Email;
//            }

//            if (user.UserName != updateData.UserName)
//            {
//                await userManager.RemoveClaimAsync(user, new System.Security.Claims.Claim("userName", user.UserName));
//                await userManager.AddClaimAsync(user, new System.Security.Claims.Claim("userName", updateData.UserName));
//                user.UserName = updateData.UserName;
//            }

//            user.Firstname = updateData.FirstName;
//            user.Lastname = updateData.LastName;

//            var removedRoles = roles.Except(updateData.Roles);
//            var addedRoles = updateData.Roles.Except(roles);


//            await userManager.AddClaimsAsync(user, addedRoles.Select(role => new System.Security.Claims.Claim("role", role)));

//            await userManager.RemoveClaimsAsync(user, removedRoles.Select(role => new System.Security.Claims.Claim("role", role)));

//            await userManager.AddToRolesAsync(user, addedRoles);
//            await userManager.RemoveFromRolesAsync(user, removedRoles);

//            var res = await userManager.UpdateAsync(user);

//            var userDetail = await userService.UpdateUserDetail(updateData.UserDetail, user.Id);

//            if (!res.Succeeded) return badRequest("Failed to Update User.");

//            return result(new AuthUserView(user, updateData.Roles, userDetail));

//        }

//        [HttpPost("deleteUser/{id}")]
//        public async Task<IActionResult> DeleteUser(string id )
//        {
//            var identityUser = await userManager.FindByIdAsync(id);

//            if(identityUser == null)
//            {
//                return badRequest("User does not exist");
//            }

//            var res = await userManager.DeleteAsync(identityUser);

//            return result(new ApiResult("User deleted"));
//        }

//        [HttpPost("editUser")]
//        public async Task<IActionResult> EditUser(EditUserView editUserData)
//        {
//            var identityUser = await userManager.FindByIdAsync(editUserData.UserId);

//            if (identityUser == null)
//            {
//                return badRequest("User does not exist");
//            }

//            if (identityUser.UserName != editUserData.UserName)
//            {
//                await userManager.ReplaceClaimAsync(identityUser, new System.Security.Claims.Claim("userName", identityUser.UserName), new System.Security.Claims.Claim("userName", editUserData.UserName));
//            }

//            if (identityUser.Email != editUserData.Email)
//            {
//                await userManager.ReplaceClaimAsync(identityUser, new System.Security.Claims.Claim("email", identityUser.Email), new System.Security.Claims.Claim("email", editUserData.UserName));
//            }


//            identityUser.UserName = editUserData.UserName;
//            identityUser.Email = editUserData.Password;

//            await userManager.UpdateAsync(identityUser);

//            return result("User Updated");
//        }

//        [HttpPost("changePassword")]
//        public async Task<IActionResult> ChangePasssword(ChangePasswordView changePasswordData)
//        {
//            if (!ModelState.IsValid)
//            {
//                return conflict("Validations failed.");
//            }

//            var identityUser = await userManager.GetUserAsync(User);

//            if (identityUser == null)
//            {
//                return badRequest("User does not exist");
//            }

//            var res = await userManager.ChangePasswordAsync(identityUser, changePasswordData.Password, changePasswordData.NewPassword);
//            if(res.Succeeded) 
//                return result("User Password Changed");
//            //todo: handler needed for errors
//            //res.Errors
//            if (res.Errors != null && res.Errors.Any())
//            {
//                return conflict(getIdentityErrorString(res.Errors));
//            }
//            return conflict("Invalid password entered");

//        }

//        private string getIdentityErrorString(IEnumerable<IdentityError> errs) 
//        {
//            var result = "";
//            foreach (var err in errs)
//            {
//                result += err.Description + " ";
//            }

//            return result;
//        }

//    }
//}