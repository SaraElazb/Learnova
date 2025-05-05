using BusinessLogicLayer.Services.AccountServices;
using BusinessLogicLayer.Manager.CourseManager;
using BusinessLogicLayer.Manager.QuizManager;
using BusinessLogicLayer.Manager.LessonManager;
using BusinessLogicLayer.DTOs.CourseDtos;
using BusinessLogicLayer.DTOs.LessonDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BusinessLogicLayer.DTOs.QuizDtos;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class InstructorController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ICourseManager _courseManager;
        private readonly IQuizManager _quizManager;
        private readonly ILessonManager _lessonManager;

        public InstructorController(
            IAccountService accountService,
            ICourseManager courseManager,
            IQuizManager quizManager,
            ILessonManager lessonManager)
        {
            _accountService = accountService;
            _courseManager = courseManager;
            _quizManager = quizManager;
            _lessonManager = lessonManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            try
            {
                // Get instructor's courses with lessons included
                var courses = await _courseManager.GetInstructorCoursesAsync(currentUserId);
                
                // Get total students enrolled in instructor's courses
                int totalStudents = courses.Sum(c => c.EnrollmentCount);
                
                // Get total quizzes created by the instructor
                var quizzes = await _quizManager.GetQuizzesByInstructorAsync(currentUserId);
                
                // Get the latest quiz (if any) for quick access
                var latestQuiz = quizzes.OrderByDescending(q => q.Quiz_ID).FirstOrDefault();
                
                // Calculate total revenue (this would be implemented differently in a real app)
                decimal totalRevenue = courses.Sum(c => c.Price * c.EnrollmentCount);
                
                // Pass data to view using ViewBag
                ViewBag.TotalCourses = courses.Count();
                ViewBag.TotalStudents = totalStudents;
                ViewBag.TotalQuizzes = quizzes.Count();
                ViewBag.TotalRevenue = totalRevenue;
                ViewBag.Courses = courses;
                ViewBag.RecentActivities = GetMockRecentActivities();
                ViewBag.LatestQuiz = latestQuiz;
                
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Dashboard action: {ex.Message}");
                TempData["ErrorMessage"] = "Error loading dashboard: " + ex.Message;
                
                // Create empty ViewBag data to avoid null reference errors
                ViewBag.TotalCourses = 0;
                ViewBag.TotalStudents = 0;
                ViewBag.TotalQuizzes = 0;
                ViewBag.TotalRevenue = 0.0m;
                ViewBag.Courses = new List<CourseDTO>();
                ViewBag.RecentActivities = new List<ActivityViewModel>();
                
                return View();
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> Courses()
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var courses = await _courseManager.GetInstructorCoursesAsync(currentUserId);
            return View(courses);
        }
        
        [HttpGet]
        public async Task<IActionResult> CreateCourse()
        {
            return RedirectToAction("Create", "Course");
        }
        
        [HttpGet]
        public async Task<IActionResult> EditCourse(int id)
        {
            return RedirectToAction("Edit", "Course", new { id });
        }
        
        [HttpGet]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            return RedirectToAction("Delete", "Course", new { id });
        }
        
        [HttpGet]
        public async Task<IActionResult> Quizzes()
        {
            try
            {
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var quizzes = await _quizManager.GetQuizzesByInstructorAsync(currentUserId);
                
                // Debug information 
                Console.WriteLine($"Retrieved {quizzes.Count()} quizzes for instructor {currentUserId}");
                
                return View(quizzes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Quizzes action: {ex.Message}");
                TempData["ErrorMessage"] = "Error loading quizzes: " + ex.Message;
                return View(new List<QuizDto>());
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> CreateQuiz(int lessonId)
        {
            return RedirectToAction("Create", "Quiz", new { lessonId });
        }
        
        [HttpGet]
        public async Task<IActionResult> EditQuiz(int id)
        {
            return RedirectToAction("Edit", "Quiz", new { id });
        }
        
        [HttpGet]
        public async Task<IActionResult> DeleteQuiz(int id)
        {
            return RedirectToAction("Delete", "Quiz", new { id });
        }
        
        [HttpGet]
        public IActionResult NewQuiz()
        {
            // This is just a simple method to redirect to the Quiz creation form
            return RedirectToAction("Create", "Quiz");
        }
        
        [HttpGet]
        public IActionResult AddLesson(int? courseId = null)
        {
            // Redirect to the lesson creation form
            return RedirectToAction("Create", "Lesson", new { courseId });
        }
        
        private List<ActivityViewModel> GetMockRecentActivities()
        {
            // In a real application, this would come from a database
            return new List<ActivityViewModel>
            {
                new ActivityViewModel
                {
                    Title = "New student enrolled in 'Introduction to C#'",
                    Type = "student",
                    TimeAgo = "2 hours ago"
                },
                new ActivityViewModel
                {
                    Title = "Quiz submitted in 'Advanced Web Development'",
                    Type = "quiz",
                    TimeAgo = "Yesterday"
                },
                new ActivityViewModel
                {
                    Title = "You updated course 'Database Design'",
                    Type = "course",
                    TimeAgo = "2 days ago"
                },
                new ActivityViewModel
                {
                    Title = "You created a new quiz for 'Introduction to C#'",
                    Type = "quiz",
                    TimeAgo = "3 days ago"
                }
            };
        }
    }
    
    public class ActivityViewModel
    {
        public string Title { get; set; }
        public string Type { get; set; } // "course", "student", "quiz"
        public string TimeAgo { get; set; }
    }
} 