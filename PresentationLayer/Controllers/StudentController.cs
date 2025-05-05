using BusinessLogicLayer.Services.AccountServices;
using BusinessLogicLayer.Services.StudentServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IStudentDashboardService _dashboardService;

        public StudentController(
            IAccountService accountService,
            IStudentDashboardService dashboardService)
        {
            _accountService = accountService;
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Dashboard()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            try
            {
                // Get enrolled courses
                var courses = await _dashboardService.GetEnrolledCoursesAsync(userId);
                
                // Get upcoming assignments
                var assignments = await _dashboardService.GetUpcomingAssignmentsAsync(userId);
                
                // Get recent activities
                var activities = await _dashboardService.GetRecentActivitiesAsync(userId);
                
                // Get overall progress
                var overallProgress = await _dashboardService.GetOverallProgressAsync(userId);
                
                // Get course progress
                var courseProgress = await _dashboardService.GetCourseProgressAsync(userId);
                
                // Pass data to view
                ViewBag.EnrolledCourses = courses;
                ViewBag.UpcomingAssignments = assignments;
                ViewBag.RecentActivities = activities;
                ViewBag.OverallProgress = overallProgress;
                ViewBag.CourseProgress = courseProgress;
                
                return View();
            }
            catch (System.Exception ex)
            {
                // Log the error
                return View("Error", ex.Message);
            }
        }
    }
} 