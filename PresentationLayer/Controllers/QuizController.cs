using AutoMapper;
using BusinessLogicLayer.DTOs.QuizDtos;
using BusinessLogicLayer.Manager.LessonManager;
using BusinessLogicLayer.Manager.QuizManager;
using BusinessLogicLayer.Manager.CourseManager;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PresentationLayer.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IQuizManager _quizManager;
        private readonly ILessonManager _lessonManager;
        private readonly ICourseManager _courseManager;
        private readonly IUnitOfWork _unitOfWork;

        public QuizController(
            IMapper mapper,
            IQuizManager quizManager,
            ILessonManager lessonManager,
            ICourseManager courseManager,
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _quizManager = quizManager ?? throw new ArgumentNullException(nameof(quizManager));
            _lessonManager = lessonManager ?? throw new ArgumentNullException(nameof(lessonManager));
            _courseManager = courseManager ?? throw new ArgumentNullException(nameof(courseManager));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        private async Task<IEnumerable<SelectListItem>> GetLessonsForInstructorAsync(string instructorId)
        {
            try
            {
                Console.WriteLine($"[DEBUG] Starting GetLessonsForInstructorAsync in QuizController for instructor: {instructorId}");
                
                // Get all lessons
                var allLessons = await _lessonManager.FindAllAsync();
                Console.WriteLine($"[DEBUG] Found {allLessons.Count()} total lessons");
                
                // Filter lessons by instructor's courses
                var instructorCourses = await _courseManager.GetInstructorCoursesAsync(instructorId);
                Console.WriteLine($"[DEBUG] Found {instructorCourses.Count()} courses for instructor {instructorId}");
                
                var instructorCourseIds = instructorCourses.Select(c => c.Course_ID).ToList();
                Console.WriteLine($"[DEBUG] Course IDs: {string.Join(", ", instructorCourseIds)}");
                
                var instructorLessons = allLessons.Where(l => instructorCourseIds.Contains(l.Course_ID)).ToList();
                
                Console.WriteLine($"[DEBUG] GetLessonsForInstructorAsync filtered {instructorLessons.Count} lessons for instructor {instructorId}");
                
                if (!instructorLessons.Any())
                {
                    Console.WriteLine("[DEBUG] No lessons found for this instructor");
                    return new List<SelectListItem>();
                }
                
                var selectItems = instructorLessons.Select(l => new SelectListItem
                {
                    Value = l.Lesson_ID.ToString(),
                    Text = $"{l.Title} (Course: {l.CourseName})"
                }).ToList();
                
                Console.WriteLine($"[DEBUG] Created {selectItems.Count} select list items");
                return selectItems;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] GetLessonsForInstructorAsync: {ex.Message}");
                Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
                return new List<SelectListItem>();
            }
        }

        // Helper method to check if a lesson belongs to the current instructor
        private async Task<bool> IsLessonOwnedByInstructorAsync(int lessonId, string instructorId)
        {
            var lesson = await _lessonManager.FindAsync(lessonId);
            if (lesson == null) return false;
            
            var course = await _courseManager.FindAsync(lesson.Course_ID);
            return course != null && course.InstructorId == instructorId;
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create(int? lessonId = null)
        {
            try {
                Console.WriteLine("[DEBUG] === Quiz Create Action Triggered ===");
                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                Console.WriteLine($"[DEBUG] Create Quiz action - User ID: {currentUserId}, lessonId: {lessonId}");
                Console.WriteLine($"[DEBUG] Request method: {HttpContext.Request.Method}");
                Console.WriteLine($"[DEBUG] Request path: {HttpContext.Request.Path}");
                
                var lessonsList = await GetLessonsForInstructorAsync(currentUserId);
                
                if (!lessonsList.Any())
                {
                    Console.WriteLine("[DEBUG] No lessons available - redirecting to Dashboard");
                    TempData["ErrorMessage"] = "You must create a course and add lessons before creating quizzes.";
                    return RedirectToAction("Dashboard", "Instructor");
                }
                
                // If lessonId is provided, verify it belongs to the instructor
                if (lessonId.HasValue)
                {
                    bool isOwned = await IsLessonOwnedByInstructorAsync(lessonId.Value, currentUserId);
                    if (!isOwned)
                    {
                        TempData["ErrorMessage"] = "You do not have permission to create a quiz for this lesson.";
                        return RedirectToAction("Quizzes", "Instructor");
                    }
                }
                
                var model = new QuizRequest
                {
                    LessonSelectList = lessonsList
                };
                
                if (lessonId.HasValue)
                {
                    model.Lesson_ID = lessonId.Value;
                    ViewBag.SelectedLessonName = lessonsList.FirstOrDefault(l => l.Value == lessonId.Value.ToString())?.Text;
                }
                
                Console.WriteLine("[DEBUG] Successfully prepared quiz create view");
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Create Quiz action: {ex.Message}");
                Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
                TempData["ErrorMessage"] = "Error loading quiz creation page: " + ex.Message;
                return RedirectToAction("Dashboard", "Instructor");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create(QuizRequest model)
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Verify the lesson belongs to the instructor
            bool isOwned = await IsLessonOwnedByInstructorAsync(model.Lesson_ID, currentUserId);
            if (!isOwned)
            {
                TempData["ErrorMessage"] = "You do not have permission to create a quiz for this lesson.";
                model.LessonSelectList = await GetLessonsForInstructorAsync(currentUserId);
                return View(model);
            }
            
            try
            {
                Console.WriteLine("======= QUIZ CREATE DIAGNOSTICS =======");
                Console.WriteLine($"Title: {model.Title}");
                Console.WriteLine($"Lesson ID: {model.Lesson_ID}");
                Console.WriteLine($"Passing Score: {model.Passing_score}");
                Console.WriteLine($"Instructions: {model.Instructions?.Substring(0, Math.Min(model.Instructions?.Length ?? 0, 50))}...");
                
                ModelState.Clear();
                
                Console.WriteLine($"Creating quiz: {model.Title}, Lesson ID: {model.Lesson_ID}");
                
                await _quizManager.CreateQuizAsync(model);
                
                TempData["SuccessMessage"] = $"Quiz '{model.Title}' created successfully!";
                
                return RedirectToAction("Quizzes", "Instructor");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating quiz: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner stack trace: {ex.InnerException.StackTrace}");
                }
                
                TempData["ErrorMessage"] = $"An error occurred while creating the quiz: {ex.Message}";
                model.LessonSelectList = await GetLessonsForInstructorAsync(currentUserId);
                return View(model);
            }
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int id)
        {
            var quiz = await _quizManager.GetByIdAsync(id);
            if (quiz == null) return NotFound();
            
            // Check if the quiz's lesson belongs to the instructor
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isOwned = await IsLessonOwnedByInstructorAsync(quiz.Lesson_ID, currentUserId);
            if (!isOwned)
            {
                TempData["ErrorMessage"] = "You do not have permission to edit this quiz.";
                return RedirectToAction("Quizzes", "Instructor");
            }

            var model = _mapper.Map<QuizRequest>(quiz);
            model.LessonSelectList = await GetLessonsForInstructorAsync(currentUserId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int id, QuizRequest model)
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Verify the lesson belongs to the instructor
            bool isOwned = await IsLessonOwnedByInstructorAsync(model.Lesson_ID, currentUserId);
            if (!isOwned)
            {
                TempData["ErrorMessage"] = "You do not have permission to edit a quiz for this lesson.";
                model.LessonSelectList = await GetLessonsForInstructorAsync(currentUserId);
                return View(model);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _quizManager.EditQuizAsync(id, model);
                    TempData["SuccessMessage"] = "Quiz updated successfully!";
                    return RedirectToAction("Quizzes", "Instructor");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error updating quiz: {ex.Message}";
                }
            }

            model.LessonSelectList = await GetLessonsForInstructorAsync(currentUserId);
            return View(model);
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Delete(int id)
        {
            var quiz = await _quizManager.GetByIdAsync(id);
            if (quiz == null) return NotFound();
            
            // Check if the quiz's lesson belongs to the instructor
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isOwned = await IsLessonOwnedByInstructorAsync(quiz.Lesson_ID, currentUserId);
            if (!isOwned)
            {
                TempData["ErrorMessage"] = "You do not have permission to delete this quiz.";
                return RedirectToAction("Quizzes", "Instructor");
            }
            
            try
            {
                await _quizManager.SoftDelete(quiz);
                TempData["SuccessMessage"] = "Quiz deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting quiz: {ex.Message}";
            }
            
            return RedirectToAction("Quizzes", "Instructor");
        }

        public async Task<IActionResult> CheckLessonsForQuiz()
        {
            var lessonItems = await GetLessonsForInstructorAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var lessons = await _lessonManager.FindAllAsync();
            
            return Json(new {
                selectListCount = lessonItems.Count(),
                selectItems = lessonItems,
                directLessonCount = lessons.Count(),
                directLessons = lessons.Select(l => new { l.Lesson_ID, l.Title, l.Course_ID })
            });
        }

        public async Task<IActionResult> DiagnoseQuizLessonIssue()
        {
            try
            {
                var diagnosticInfo = new Dictionary<string, object>();
                
                var lessonRepo = _unitOfWork.GetRepository<Lesson>();
                var lessonsFromDb = await lessonRepo.FindAll().ToListAsync();
                diagnosticInfo.Add("1_DirectDbLessonCount", lessonsFromDb.Count);
                diagnosticInfo.Add("1_DirectDbLessons", lessonsFromDb.Select(l => new { 
                    l.Lesson_ID, l.Title, l.Course_ID
                }));
                
                var lessonsFromManager = await _lessonManager.FindAllAsync();
                diagnosticInfo.Add("2_ManagerLessonCount", lessonsFromManager.Count());
                diagnosticInfo.Add("2_ManagerLessons", lessonsFromManager.Select(l => new { 
                    l.Lesson_ID, l.Title, l.Course_ID
                }));
                
                var selectItems = await GetLessonsForInstructorAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
                diagnosticInfo.Add("3_SelectItemsCount", selectItems.Count());
                diagnosticInfo.Add("3_SelectItems", selectItems);
                
                var model = new QuizRequest
                {
                    LessonSelectList = selectItems,
                    Title = "Test Quiz",
                    Instructions = "Test instructions",
                    Passing_score = 70
                };
                diagnosticInfo.Add("4_ModelSelectListCount", model.LessonSelectList.Count());
                
                return Json(diagnosticInfo);
            }
            catch (Exception ex)
            {
                return Json(new {
                    Error = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                });
            }
        }
    }
} 