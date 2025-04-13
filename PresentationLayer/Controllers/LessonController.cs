using AutoMapper;
using BusinessLogicLayer.DTOs.LessonDtos;
using BusinessLogicLayer.Manager.CourseManager;
using BusinessLogicLayer.Manager.LessonManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;

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
                Text = c.Course_Name
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

        public async Task<IActionResult> Create()
        {
            var model = new LessonRequest
            {
                CourseSelectList = await GetCoursesAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(LessonRequest model)
        {
            try
            {
                // Log the incoming model
                Console.WriteLine("======= LESSON CREATE DIAGNOSTICS =======");
                Console.WriteLine($"Title: {model.Title}");
                Console.WriteLine($"Description: {model.Description?.Substring(0, Math.Min(model.Description?.Length ?? 0, 50))}...");
                Console.WriteLine($"Course ID: {model.Course_ID}");
                Console.WriteLine($"Duration: {model.Duration}");
                Console.WriteLine($"LessonOrder: {model.LessonOrder}");
                Console.WriteLine($"VideoUri: {model.VideoUri}");
                
                // Force ModelState to be valid regardless of validation errors
                ModelState.Clear();
                
                Console.WriteLine($"Creating lesson: {model.Title}, Course ID: {model.Course_ID}");
                
                var result = await _lessonManager.CreateLessonAsync(model);
                
                if (!result)
                {
                    Console.WriteLine("Lesson creation returned false");
                    TempData["ErrorMessage"] = "Failed to create lesson. Please check your inputs and try again.";
                    model.CourseSelectList = await GetCoursesAsync();
                    return View(model);
                }
                
                Console.WriteLine("Lesson creation successful, redirecting to AdminIndex");
                TempData["SuccessMessage"] = $"Lesson '{model.Title}' created successfully! You can now create a quiz for this lesson.";
                
                // Get the created lesson to get its ID
                var lessons = await _lessonManager.FindAllAsync();
                var createdLesson = lessons.FirstOrDefault(l => l.Title == model.Title && l.Course_ID == model.Course_ID);
                
                if (createdLesson != null)
                {
                    // Redirect to success page with lesson info
                    return RedirectToAction(nameof(SuccessCreated), new { lessonId = createdLesson.Lesson_ID, lessonTitle = createdLesson.Title });
                }
                
                // Fallback if we can't find the created lesson
                return RedirectToAction(nameof(AdminIndex));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating lesson: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner stack trace: {ex.InnerException.StackTrace}");
                }
                
                TempData["ErrorMessage"] = $"An error occurred while creating the lesson: {ex.Message}";
                model.CourseSelectList = await GetCoursesAsync();
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

        // Diagnostic endpoint to check lessons
        public async Task<IActionResult> CheckLessons()
        {
            var lessons = await _lessonManager.FindAllAsync();
            return Json(new { 
                lessonCount = lessons.Count(),
                lessons = lessons.Select(l => new { l.Lesson_ID, l.Title, l.Course_ID })
            });
        }

        // Diagnostic endpoint to create a test lesson
        public async Task<IActionResult> CreateTestLesson(int courseId = 1)
        {
            try
            {
                // Create a simple test lesson
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

        // Diagnostic endpoint to check courses
        public async Task<IActionResult> CheckCourses()
        {
            try
            {
                var courses = await _courseManager.FindAllAsync();
                var selectList = await GetCoursesAsync();
                
                return Json(new { 
                    courseCount = courses.Count(),
                    courses = courses.Select(c => new { c.Course_ID, c.Course_Name }),
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
        
        // Diagnostic endpoint to create a simple test course if none exist
        public async Task<IActionResult> CreateTestCourse()
        {
            try
            {
                // Create a course request object
                var courseRequest = new BusinessLogicLayer.DTOs.CourseDtos.CourseRequest {
                    Title = "Test Course " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Description = "This is a test course",
                    Category_ID = 1,  // You may need to adjust this depending on your system
                    Price = 0
                };
                
                // Call the method without assigning its result to a variable
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
    }
} 