using BusinessLogicLayer.Services.RoleServices;
using BusinessLogicLayer.Services.UserRoleServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ActionRequests.UserRolesActionRequest;
using PresentationLayer.VMs.UserRolesVms;

namespace PresentationLayer.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserRoleService _userRoleService;
        private readonly IRoleService _roleService;

        public AdminController(IUserRoleService userRoleService, IRoleService roleService)
        {
            _userRoleService = userRoleService;
            _roleService = roleService;
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ViewUserRoles(string searchTerm, int page = 1, int pageSize = 3)
        {
            var (userRoles, totalUsers) =
                await _userRoleService.GetUsersWithRolesAsync(searchTerm, page, pageSize);
            IEnumerable<UserRoleVm>? userRolesVMs = userRoles.Select(ur => ur.ToVm()).ToList();
            var allRoles = await _roleService.GetAllRolesAsync();
            ViewBag.SearchTerm = searchTerm;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);
            ViewBag.AllRoles = allRoles.Select(r => r.RoleName).ToList();

            return View(userRolesVMs);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateUserRoles(EditUserRolesActionRequest request)
        {
            var dto = request.ToDto();
            var success = await _userRoleService.UpdateUserRolesAsync(dto);
            if (!success)
            {
                return BadRequest("User not found.");
            }

            TempData["Success"] = "User roles updated successfully.";
            return RedirectToAction(nameof(ViewUserRoles));
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
