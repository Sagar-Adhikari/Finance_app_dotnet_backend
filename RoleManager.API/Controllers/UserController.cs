using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoleManager.API.Services;
using RoleManager.API.Views;

namespace RoleManager.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]

    public class UserController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserService userService;

        public UserController(UserManager<ApplicationUser> userManager, IUserService userService)
        {
            this.userManager = userManager;
            this.userService = userService;
        }

        [HttpGet("getAuth")]

        public async Task<IActionResult> GetAuthUser()
        {
            var identityUser = await userManager.GetUserAsync(User);

            var roles = await userManager.GetRolesAsync(identityUser);

            return result(new AuthUserView(identityUser, roles.ToList()));
        }


        [HttpGet("getUsers")]
        public async Task<IActionResult> GetUsersAsync([FromQuery] int page = 0, int limit = 20)
        {

            var identityUsers = userManager.Users.Skip(page * limit).Take(limit).ToList();

            var total = userManager.Users.Count();

            List<AuthUserView> allUsers = new List<AuthUserView>();

            foreach(var user in identityUsers)
            {
                var roles = await userManager.GetRolesAsync(user);

                var userDetail = await userService.GetUserDetail(user.Id);
                allUsers.Add(new AuthUserView(user, roles.ToList(), userDetail));
            }


            return result(new PaginationView<AuthUserView>() { 
                Total = total,
                Page = page,
                Contents = allUsers
            });;
        }

        [HttpPost("createNewUser")]
        public async Task<IActionResult> AddnewUser([FromBody] SignupView signupData)
        {
            var identityUser = new ApplicationUser() {
                Firstname = signupData.FirstName,
                Lastname = signupData.LastName,
                UserName = signupData.UserName,
                Email = signupData.Email,                
            };

            var res = await  userManager.CreateAsync(identityUser, signupData.Password);
            if (!res.Succeeded) return badRequest("Failed to Create User.");

            foreach (var role in signupData.Roles)
            {
                await userManager.AddToRoleAsync(identityUser, role);
            }

            var claims = signupData.Roles.Select(role => new System.Security.Claims.Claim("role", role)).ToList();

            claims.Add(new System.Security.Claims.Claim("userName", identityUser.UserName));
            claims.Add(new System.Security.Claims.Claim("email", identityUser.Email));

            await userManager.AddClaimsAsync(identityUser, claims);

            var userDetail = await userService.CreateUserDetail(signupData.UserDetail, identityUser.Id);

            return result(new AuthUserView(identityUser, signupData.Roles, userDetail ));

        }

        [HttpPost("updateUser/{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] SignupView updateData)
        {
            var user = await userManager.FindByIdAsync(userId);

            var roles = await userManager.GetRolesAsync(user);

            if(user.Email != updateData.Email)
            {
                await userManager.RemoveClaimAsync(user, new System.Security.Claims.Claim("email", user.Email));
                await userManager.AddClaimAsync(user, new System.Security.Claims.Claim("email", updateData.Email));
                user.Email = updateData.Email;
            }

            if (user.UserName != updateData.UserName)
            {
                await userManager.RemoveClaimAsync(user, new System.Security.Claims.Claim("userName", user.UserName));
                await userManager.AddClaimAsync(user, new System.Security.Claims.Claim("userName", updateData.UserName));
                user.UserName = updateData.UserName;
            }

            user.Firstname = updateData.FirstName;
            user.Lastname = updateData.LastName;

            var removedRoles = roles.Except(updateData.Roles);
            var addedRoles = updateData.Roles.Except(roles);

        
            await userManager.AddClaimsAsync(user, addedRoles.Select(role => new System.Security.Claims.Claim("role", role)));

            await userManager.RemoveClaimsAsync(user, removedRoles.Select(role => new System.Security.Claims.Claim("role", role)));

            await userManager.AddToRolesAsync(user, addedRoles);
            await userManager.RemoveFromRolesAsync(user, removedRoles);

            var res = await userManager.UpdateAsync(user);

            var userDetail = await userService.UpdateUserDetail(updateData.UserDetail, user.Id);

            if (!res.Succeeded) return badRequest("Failed to Update User.");

            return result(new AuthUserView(user, updateData.Roles, userDetail));

        }

        [HttpPost("deleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(string id )
        {
            var identityUser = await userManager.FindByIdAsync(id);

            if(identityUser == null)
            {
                return badRequest("User does not exist");
            }

            var res = await userManager.DeleteAsync(identityUser);

            return result(new ApiResult("User deleted"));
        }

        [HttpPost("editUser")]
        public async Task<IActionResult> EditUser(EditUserView editUserData)
        {
            var identityUser = await userManager.FindByIdAsync(editUserData.UserId);

            if (identityUser == null)
            {
                return badRequest("User does not exist");
            }

            if (identityUser.UserName != editUserData.UserName)
            {
                await userManager.ReplaceClaimAsync(identityUser, new System.Security.Claims.Claim("userName", identityUser.UserName), new System.Security.Claims.Claim("userName", editUserData.UserName));
            }

            if (identityUser.Email != editUserData.Email)
            {
                await userManager.ReplaceClaimAsync(identityUser, new System.Security.Claims.Claim("email", identityUser.Email), new System.Security.Claims.Claim("email", editUserData.UserName));
            }


            identityUser.UserName = editUserData.UserName;
            identityUser.Email = editUserData.Password;
            
            await userManager.UpdateAsync(identityUser);

            return result("User Updated");
        }

        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePasssword(ChangePasswordView changePasswordData)
        {
            if (!ModelState.IsValid)
            {
                return conflict("Validations failed.");
            }

            var identityUser = await userManager.GetUserAsync(User);

            if (identityUser == null)
            {
                return badRequest("User does not exist");
            }

            var res = await userManager.ChangePasswordAsync(identityUser, changePasswordData.Password, changePasswordData.NewPassword);
            if(res.Succeeded) 
                return result("User Password Changed");
            //todo: handler needed for errors
            //res.Errors
            if (res.Errors != null && res.Errors.Any())
            {
                return conflict(getIdentityErrorString(res.Errors));
            }
            return conflict("Invalid password entered");

        }

        private string getIdentityErrorString(IEnumerable<IdentityError> errs) 
        {
            var result = "";
            foreach (var err in errs)
            {
                result += err.Description + " ";
            }

            return result;
        }

    }
}