using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgrammersBlog.Core.Concrete.Dtos.RoleDto;
using ProgrammersBlog.Core.Concrete.Dtos.UserDto;
using ProgrammersBlog.Core.Concrete.Entities;
using ProgrammersBlog.Core.Enums;
using ProgrammersBlog.Mvc.Areas.Admin.Models;
using ProgrammersBlog.Mvc.Extantions;
using System.Text.Json;

namespace ProgrammersBlog.Mvc.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public RoleController(RoleManager<Role> roleManager, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Authorize(Roles = "SuperAdmin,Role.Read")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(new RoleListDto { Roles = roles });
        }

        [Authorize(Roles = "SuperAdmin,Role.Read")]
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var jsonRoles = JsonSerializer.Serialize(new RoleListDto { Roles = roles });
            return Json(jsonRoles);
        }

        [Authorize(Roles = "SuperAdmin,User.Update")]
        [HttpGet]
        public async Task<PartialViewResult> UserRoleAssign(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var userRoles = await _userManager.GetRolesAsync(user!);
            var roles = await _roleManager.Roles.ToListAsync();

            var userRoleAssign = new UserRoleAssignDto
            {
                UserId = userId,
                UserName = user.UserName
            };

            foreach (var role in roles)
            {
                var RoleAssignDto = new RoleAssignDto
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    HasRole = await _userManager.IsInRoleAsync(user, role.Name)
                };
                userRoleAssign.RoleAssignDtos.Add(RoleAssignDto);
            }
            return PartialView("_RoleAssignPartial", userRoleAssign);
        }

        [Authorize(Roles = "SuperAdmin,User.Update")]
        [HttpPost]
        public async Task<JsonResult> UserRoleAssign(UserRoleAssignDto assignDto)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(assignDto.UserName);

                foreach (var role in assignDto.RoleAssignDtos)
                {
                    if (role.HasRole)
                    {
                        await _userManager.AddToRoleAsync(user, role.RoleName);
                    }
                    else
                    {
                        await _userManager.RemoveFromRoleAsync(user, role.RoleName);
                    }
                }

                await _userManager.UpdateSecurityStampAsync(user);
                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(user, isPersistent: false);
                var userRoleAssignAjaxModel = new UserRoleAssignAjaxViewModel
                {
                    UserDto = new UserDto { User = user, Message = $"{user.UserName} adlı kullanıcıya ait rol işlemi başarıyla tamamlanmıştır.", ResultStatusType = ResultStatusType.Success },
                    RoleAssignPartial = await this.RenderViewToStringAsync("_RoleAssignPartial", assignDto)

                };
                var userRoleAssignJson = JsonSerializer.Serialize(userRoleAssignAjaxModel);
                return Json(userRoleAssignJson);
            }
            var userRoleErrorAssignJson = JsonSerializer.Serialize(new UserRoleAssignAjaxViewModel
            {

                RoleAssignPartial = await this.RenderViewToStringAsync("_RoleAssignPartial", assignDto),
                UserRoleAssignDto = assignDto
            });
            return Json(userRoleErrorAssignJson);
        }
    }
}
