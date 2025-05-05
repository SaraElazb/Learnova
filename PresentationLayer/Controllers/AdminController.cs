using BusinessLogicLayer.Services.AccountServices;
using BusinessLogicLayer.Services.RoleServices;
using BusinessLogicLayer.Services.UserRoleServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
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
        private readonly UserManager<User> _userManager;

        public AdminController(IAccountService accountService, IUserRoleService userRoleService, IRoleService roleService, UserManager<User> userManager)
        {
            _accountService = accountService;
            _userRoleService = userRoleService;
            _roleService = roleService;
            _userManager = userManager;
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
            ViewBag.TotalCourses = 0; // This should be replaced with actual data from a course service
            
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
        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> CheckUserDeletionStatus(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }
            
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            
            // Check if user is the current logged-in admin
            if (User.Identity.Name == user.UserName)
            {
                return BadRequest("You cannot delete your own account while logged in.");
            }
            
            var (canDelete, associatedData) = await _userRoleService.CheckUserAssociationsAsync(userId);
            
            return Json(new { 
                canDelete = canDelete, 
                username = user.UserName,
                email = user.Email,
                associatedData = associatedData 
            });
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData["Error"] = "User ID is required.";
                return RedirectToAction(nameof(ViewUserRoles));
            }
            
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(ViewUserRoles));
            }
            
            // Check if user is the current logged-in admin
            if (User.Identity.Name == user.UserName)
            {
                TempData["Error"] = "You cannot delete your own account while logged in.";
                return RedirectToAction(nameof(ViewUserRoles));
            }
            
            // Check if user has associated data
            var (canDelete, associatedData) = await _userRoleService.CheckUserAssociationsAsync(userId);
            if (!canDelete)
            {
                TempData["Error"] = $"Cannot delete user '{user.UserName}' because they have associated data: " +
                                  $"{string.Join(", ", associatedData)}. Please remove or reassign this content first.";
                return RedirectToAction(nameof(ViewUserRoles));
            }
            
            try
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["Success"] = "User deleted successfully.";
                }
                else
                {
                    TempData["Error"] = "Failed to delete user: " + string.Join(", ", result.Errors.Select(e => e.Description));
                }
            }
            catch (DbUpdateException ex)
            {
                // Check if it's a foreign key constraint violation
                if (ex.InnerException is SqlException sqlEx && 
                   (sqlEx.Number == 547 || // Foreign key constraint violation
                    sqlEx.Message.Contains("REFERENCE constraint") || 
                    sqlEx.Message.Contains("FOREIGN KEY constraint")))
                {
                    TempData["Error"] = "Cannot delete this user because they have associated data in the system " +
                                       "(courses, lessons, etc.). You must first remove or reassign this content.";
                }
                else
                {
                    TempData["Error"] = "An error occurred while deleting the user: " + ex.Message;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An unexpected error occurred: " + ex.Message;
            }
            
            return RedirectToAction(nameof(ViewUserRoles));
        }
    }
}
