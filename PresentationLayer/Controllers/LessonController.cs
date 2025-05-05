using AutoMapper;
using BusinessLogicLayer.DTOs.LessonDtos;
using BusinessLogicLayer.Manager.CourseManager;
using BusinessLogicLayer.Manager.LessonManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace PresentationLayer.Controllers
{
    public class LessonController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILessonManager _lessonManager;
        private readonly ICourseManager _courseManager;

        public LessonController(
            IMapper mapper,
            ILessonManager lessonManager,
            ICourseManager courseManager)
        {
            _mapper = mapper;
            _lessonManager = lessonManager;
            _courseManager = courseManager;
        }

        private async Task<IEnumerable<SelectListItem>> GetCoursesAsync()
        {
            var courses = await _courseManager.FindAllAsync();
            return courses.Select(c => new SelectListItem
            {
                Value = c.Course_ID.ToString(),
                Text = c.Title
            });
        }

        public async Task<IActionResult> AdminIndex()
        {
            var lessonDtos = await _lessonManager.FindAllAsync();
            return View(lessonDtos);
        }

        public async Task<IActionResult> UserIndex(int courseId)
        {
            var lessonDtos = await _lessonManager.GetLessonsByCourseAsync(courseId);
            ViewBag.CourseId = courseId;
            return View(lessonDtos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var lessonDto = await _lessonManager.FindAsync(id);
            if (lessonDto == null) return NotFound();

            return View(lessonDto);
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create(int? courseId = null)
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = new LessonRequest();
            
            if (courseId.HasValue)
            {
                // Check if the course belongs to the instructor
                var courses = await _courseManager.GetInstructorCoursesAsync(currentUserId);
                var course = courses.FirstOrDefault(c => c.Course_ID == courseId.Value);
                
                if (course == null)
                {
                    TempData["ErrorMessage"] = "You don't have permission to add lessons to this course.";
                    return RedirectToAction("Dashboard", "Instructor");
                }
                
                model.Course_ID = courseId.Value;
                model.CourseSelectList = new List<SelectListItem> {
                    new SelectListItem { Value = course.Course_ID.ToString(), Text = course.Title, Selected = true }
                };
            }
            else
            {
                // Get only courses belonging to this instructor
                model.CourseSelectList = await GetInstructorCoursesAsync(currentUserId);
            }
            
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create(LessonRequest model)
        {
            try
            {
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                // Verify this course belongs to the instructor
                var courses = await _courseManager.GetInstructorCoursesAsync(currentUserId);
                var course = courses.FirstOrDefault(c => c.Course_ID == model.Course_ID);
                
                if (course == null)
                {
                    TempData["ErrorMessage"] = "You don't have permission to add lessons to this course.";
                    model.CourseSelectList = await GetInstructorCoursesAsync(currentUserId);
                    return View(model);
                }
                
                Console.WriteLine($"Creating lesson: {model.Title}, Course ID: {model.Course_ID}");
                
                var result = await _lessonManager.CreateLessonAsync(model);
                
                if (!result)
                {
                    Console.WriteLine("Lesson creation returned false");
                    TempData["ErrorMessage"] = "Failed to create lesson. Please check your inputs and try again.";
                    model.CourseSelectList = await GetInstructorCoursesAsync(currentUserId);
                    return View(model);
                }
                
                Console.WriteLine("Lesson creation successful, redirecting to Instructor Dashboard");
                TempData["SuccessMessage"] = $"Lesson '{model.Title}' created successfully!";
                
                // Redirect back to instructor dashboard
                return RedirectToAction("Dashboard", "Instructor");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating lesson: {ex.Message}");
                
                TempData["ErrorMessage"] = $"An error occurred while creating the lesson: {ex.Message}";
                model.CourseSelectList = await GetInstructorCoursesAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var lesson = await _lessonManager.GetByIdAsync(id);
            if (lesson == null) return NotFound();

            var model = _mapper.Map<LessonRequest>(lesson);
            model.CourseSelectList = await GetCoursesAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, LessonRequest model)
        {
            if (!ModelState.IsValid)
            {
                model.CourseSelectList = await GetCoursesAsync();
                return View(model);
            }

            var success = await _lessonManager.EditLessonAsync(id, model);
            if (!success) return NotFound();

            return RedirectToAction(nameof(AdminIndex));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var lesson = await _lessonManager.GetByIdAsync(id);
            if (lesson == null) return NotFound();

            await _lessonManager.SoftDelete(lesson);

            return RedirectToAction(nameof(AdminIndex));
        }

        public async Task<IActionResult> CheckLessons()
        {
            var lessons = await _lessonManager.FindAllAsync();
            return Json(new { 
                lessonCount = lessons.Count(),
                lessons = lessons.Select(l => new { l.Lesson_ID, l.Title, l.Course_ID })
            });
        }

        public async Task<IActionResult> CreateTestLesson(int courseId = 1)
        {
            try
            {
                var lessonRequest = new LessonRequest
                {
                    Title = "Test Lesson " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Description = "This is a test lesson created for diagnostic purposes",
                    Duration = 30,
                    LessonOrder = 1,
                    Course_ID = courseId,
                    VideoUri = "https://www.example.com/video"
                };
                
                var result = await _lessonManager.CreateLessonAsync(lessonRequest);
                
                return Json(new {
                    success = result,
                    lesson = lessonRequest
                });
            }
            catch (Exception ex)
            {
                return Json(new {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        public async Task<IActionResult> CheckCourses()
        {
            try
            {
                var courses = await _courseManager.FindAllAsync();
                var selectList = await GetCoursesAsync();
                
                return Json(new { 
                    courseCount = courses.Count(),
                    courses = courses.Select(c => new { c.Course_ID, c.Title }),
                    selectListCount = selectList.Count(),
                    selectList = selectList
                });
            }
            catch (Exception ex)
            {
                return Json(new {
                    error = ex.Message,
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message
                });
            }
        }
        
        public async Task<IActionResult> CreateTestCourse()
        {
            try
            {
                var courseRequest = new BusinessLogicLayer.DTOs.CourseDtos.CourseRequest {
                    Title = "Test Course " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Description = "This is a test course",
                    Category_ID = 1,
                    Price = 0
                };
                
                await _courseManager.CreateCourseAsync(courseRequest);
                
                return Json(new {
                    success = true,
                    message = "Test course created successfully"
                });
            }
            catch (Exception ex)
            {
                return Json(new {
                    success = false,
                    error = ex.Message,
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message
                });
            }
        }

        public async Task<IActionResult> SuccessCreated(int lessonId, string lessonTitle)
        {
            ViewBag.LessonId = lessonId;
            ViewBag.LessonTitle = lessonTitle;
            return View();
        }

        // Get courses for a specific instructor
        private async Task<IEnumerable<SelectListItem>> GetInstructorCoursesAsync(string instructorId)
        {
            var courses = await _courseManager.GetInstructorCoursesAsync(instructorId);
            return courses.Select(c => new SelectListItem
            {
                Value = c.Course_ID.ToString(),
                Text = c.Title
            });
        }
    }
} 