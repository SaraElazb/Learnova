using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Entities;
using PresentationLayer.Models;
using System.Security.Claims;
using DataAccessLayer.Repositories;
using System.Threading.Tasks;
using BusinessLogicLayer.Manager.CourseManager;

namespace PresentationLayer.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ICourseManager _courseManager;
        private readonly IUnitOfWork _unitOfWork;

        public ProfileController(
            UserManager<User> userManager,
            ICourseManager courseManager,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _courseManager = courseManager;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index", "Home");
            }

            var model = new ProfileViewModel
            {
                Id = user.Id,
                FirstName = user.First_name,
                LastName = user.Last_name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserName = user.UserName,
                DateJoined = user.Registration_date
            };

            // Add role-specific information
            if (User.IsInRole("Teacher"))
            {
                // Get instructor-specific data
                var courses = await _courseManager.GetInstructorCoursesAsync(userId);
                model.IsInstructor = true;
                model.TotalCourses = courses.Count();
                model.TotalStudents = await _unitOfWork.Enrollments
                    .FindByCondition(e => courses.Select(c => c.Course_ID).Contains(e.Course_ID))
                    .CountAsync();
            }
            else if (User.IsInRole("Student"))
            {
                // Get student-specific data
                var enrollments = await _unitOfWork.Enrollments
                    .FindByCondition(e => e.User_ID == userId)
                    .ToListAsync();
                
                model.IsStudent = true;
                model.EnrolledCourses = enrollments.Count;
                
                // Get completion statistics
                var completedLessons = await _unitOfWork.Studies
                    .FindByCondition(s => s.User_ID == userId && s.Status == "Completed")
                    .CountAsync();
                
                model.CompletedLessons = completedLessons;
                
                // Get quiz data
                var quizSubmissions = await _unitOfWork.GetRepository<Submission>()
                    .FindByCondition(s => s.User_ID == userId)
                    .ToListAsync();
                
                model.QuizzesCompleted = quizSubmissions.Count;
                model.QuizzesPassed = quizSubmissions.Count(s => s.Passed);
            }

            return View("~/Views/Shared/Profile.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index", "Home");
            }

            var model = new EditProfileViewModel
            {
                FirstName = user.First_name,
                LastName = user.Last_name,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email
            };

            return View("~/Views/Shared/EditProfile.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);
                
                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Index", "Home");
                }

                user.First_name = model.FirstName;
                user.Last_name = model.LastName;
                user.PhoneNumber = model.PhoneNumber;
                
                // Only update email if it changed
                if (user.Email != model.Email)
                {
                    var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                    if (!setEmailResult.Succeeded)
                    {
                        foreach (var error in setEmailResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View("~/Views/Shared/EditProfile.cshtml", model);
                    }
                    
                    // Update username to match email if they were previously the same
                    if (user.UserName == user.Email)
                    {
                        user.UserName = model.Email;
                    }
                }

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("~/Views/Shared/EditProfile.cshtml", model);
                }

                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Shared/EditProfile.cshtml", model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View("~/Views/Shared/ChangePassword.cshtml", new ChangePasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Shared/ChangePassword.cshtml", model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index", "Home");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("~/Views/Shared/ChangePassword.cshtml", model);
            }

            // Update security stamp to force sign-out on all devices
            await _userManager.UpdateSecurityStampAsync(user);
            
            TempData["Success"] = "Password changed successfully! Please sign in with your new password.";
            return RedirectToAction("LogOut", "Account");
        }
    }
} 