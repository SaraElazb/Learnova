using BusinessLogicLayer.Services.AccountServices;
using BusinessLogicLayer.Services.StudentServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IStudentDashboardService _dashboardService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StudentController> _logger;

        public StudentController(
            IAccountService accountService,
            IStudentDashboardService dashboardService,
            IUnitOfWork unitOfWork,
            ILogger<StudentController> logger)
        {
            _accountService = accountService;
            _dashboardService = dashboardService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            try
            {
                // Get enrolled courses
                var courses = await _dashboardService.GetEnrolledCoursesAsync(userId);
                
                if (!courses.Any())
                {
                    _logger.LogInformation($"User {userId} has no enrolled courses yet");
                    
                    // If no courses found via dashboard service, check enrollments directly
                    var enrollments = await _unitOfWork.Enrollments
                        .FindAllAsync(
                            e => e.User_ID == userId,
                            include: q => q.Include(e => e.Course)
                                .ThenInclude(c => c.Category)
                        );
                    
                    if (enrollments.Any())
                    {
                        courses = enrollments.Select(e => new BusinessLogicLayer.DTOs.CourseDtos.CourseDTO
                        {
                            Course_ID = e.Course.Course_ID,
                            Title = e.Course.Title,
                            Description = e.Course.Description,
                            Category = e.Course.Category?.Category_Name ?? "Uncategorized",
                            CategoryId = e.Course.Category_ID,
                            EnrollmentDate = e.Enrollment_date,
                            Price = e.Course.Price,
                            ThumbnailUrl = e.Course.ImagePath,
                            Progress = 0  // New enrollment, no progress yet
                        }).ToList();
                        
                        _logger.LogInformation($"Found {courses.Count()} courses from direct enrollment check for user {userId}");
                    }
                }
                
                // Get upcoming assignments
                var assignments = await _dashboardService.GetUpcomingAssignmentsAsync(userId);
                
                // Get recent activities
                var activities = await _dashboardService.GetRecentActivitiesAsync(userId);
                
                // Get overall progress
                var overallProgress = await _dashboardService.GetOverallProgressAsync(userId);
                
                // Get course progress
                var courseProgress = await _dashboardService.GetCourseProgressAsync(userId);
                
                // Add recently purchased courses to the activities
                var recentEnrollments = await _unitOfWork.Enrollments
                    .FindAllAsync(
                        e => e.User_ID == userId && e.Enrollment_date > DateTime.UtcNow.AddDays(-3),
                        include: q => q.Include(e => e.Course)
                    );
                    
                var recentActivities = new List<ActivityDto>(activities);
                
                foreach (var enrollment in recentEnrollments)
                {
                    recentActivities.Add(new ActivityDto
                    {
                        Type = "enrollment",
                        Description = $"Enrolled in course: {enrollment.Course.Title}",
                        Date = enrollment.Enrollment_date,
                        DisplayDate = GetDisplayDate(enrollment.Enrollment_date)
                    });
                }
                
                // Pass data to view
                ViewBag.EnrolledCourses = courses;
                ViewBag.UpcomingAssignments = assignments;
                ViewBag.RecentActivities = recentActivities.OrderByDescending(a => a.Date).Take(5);
                ViewBag.OverallProgress = overallProgress;
                ViewBag.CourseProgress = courseProgress;
                
                return View();
            }
            catch (System.Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"Error loading student dashboard for user {userId}");
                return View("Error", ex.Message);
            }
        }
        
        private string GetDisplayDate(DateTime date)
        {
            var diff = DateTime.UtcNow - date;
            
            if (diff.TotalDays < 1)
                return "Today";
            else if (diff.TotalDays < 2)
                return "Yesterday";
            else if (diff.TotalDays < 7)
                return $"{(int)diff.TotalDays} days ago";
            else if (diff.TotalDays < 30)
                return $"{(int)(diff.TotalDays / 7)} weeks ago";
            else
                return date.ToString("MMMM dd, yyyy");
        }
    }
} 