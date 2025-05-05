using BusinessLogicLayer.Services.AccountServices;
using BusinessLogicLayer.Services.RoleServices;
using BusinessLogicLayer.Services.UserRoleServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ActionRequests.UserRolesActionRequest;
using PresentationLayer.VMs.UserRolesVms;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IUserRoleService _userRoleService;
        private readonly IRoleService _roleService;

        public AdminController(IAccountService accountService, IUserRoleService userRoleService, IRoleService roleService)
        {
            _accountService = accountService;
            _userRoleService = userRoleService;
            _roleService = roleService;
        }

        public async Task<IActionResult> Dashboard(string searchTerm = "", int page = 1)
        {
            int pageSize = 10;
            
            // Get users with their roles
            var (users, totalUsers) = await _userRoleService.GetUsersWithRolesAsync(searchTerm, page, pageSize);
            
            // Calculate total pages
            int totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);
            
            // Set up ViewBag for pagination
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchTerm = searchTerm;
            
            // Get statistics
            ViewBag.TotalUsers = totalUsers;
            ViewBag.TotalStudents = users.Count(u => u.Roles.Contains("Student"));
            ViewBag.TotalInstructors = users.Count(u => u.Roles.Contains("Teacher"));
            
            // Get course statistics - you'll need to inject the appropriate service
            // ViewBag.TotalCourses = await _courseManager.GetTotalCoursesAsync();
            // ViewBag.TotalCategories = await _categoryManager.GetTotalCategoriesAsync();
            
            return View(users);
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
    }
}
