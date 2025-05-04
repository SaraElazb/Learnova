using AutoMapper;
using BusinessLogicLayer.DTOs.QuizDtos;
using BusinessLogicLayer.Manager.LessonManager;
using BusinessLogicLayer.Manager.QuizManager;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace PresentationLayer.Controllers
{
    public class QuizController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IQuizManager _quizManager;
        private readonly ILessonManager _lessonManager;
        private readonly IUnitOfWork _unitOfWork;

        public QuizController(
            IMapper mapper,
            IQuizManager quizManager,
            ILessonManager lessonManager,
            IUnitOfWork unitOfWork)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _quizManager = quizManager ?? throw new ArgumentNullException(nameof(quizManager));
            _lessonManager = lessonManager ?? throw new ArgumentNullException(nameof(lessonManager));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        private async Task<IEnumerable<SelectListItem>> GetLessonsAsync()
        {
            try
            {
                Console.WriteLine("Starting GetLessonsAsync in QuizController");
                var lessons = await _lessonManager.FindAllAsync();
                
                Console.WriteLine($"GetLessonsAsync received {lessons?.Count() ?? 0} lessons from LessonManager");
                
                if (lessons == null || !lessons.Any())
                {
                    Console.WriteLine("No lessons found or lessons is null");
                    return new List<SelectListItem>();
                }
                
                var selectItems = lessons.Select(l => new SelectListItem
                {
                    Value = l.Lesson_ID.ToString(),
                    Text = l.Title
                }).ToList();
                
                Console.WriteLine($"Created {selectItems.Count} select list items");
                return selectItems;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetLessonsAsync: {ex.Message}");
                return new List<SelectListItem>();
            }
        }

        public async Task<IActionResult> AdminIndex()
        {
            var quizDtos = await _quizManager.FindAllAsync();
            return View(quizDtos);
        }

        public async Task<IActionResult> UserIndex(int lessonId)
        {
            var quizDtos = await _quizManager.GetQuizzesByLessonAsync(lessonId);
            ViewBag.LessonId = lessonId;
            return View(quizDtos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var quizDto = await _quizManager.FindAsync(id);
            if (quizDto == null) return NotFound();

            return View(quizDto);
        }

        public async Task<IActionResult> Create(int? lessonId = null)
        {
            var lessonsList = await GetLessonsAsync();
            
            if (!lessonsList.Any())
            {
                TempData["ErrorMessage"] = "No lessons are available. Please create a lesson first.";
                return RedirectToAction("Create", "Lesson");
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
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(QuizRequest model)
        {
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
                
                return RedirectToAction(nameof(AdminIndex));
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
                model.LessonSelectList = await GetLessonsAsync();
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var quiz = await _quizManager.GetByIdAsync(id);
            if (quiz == null) return NotFound();

            var model = _mapper.Map<QuizRequest>(quiz);
            model.LessonSelectList = await GetLessonsAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, QuizRequest model)
        {
            try
            {
                Console.WriteLine("======= QUIZ EDIT POST DIAGNOSTICS =======");
                Console.WriteLine($"Quiz ID from route: {id}");
                Console.WriteLine($"Quiz ID from model: {model.Quiz_ID}");
                Console.WriteLine($"Title: {model.Title}");
                Console.WriteLine($"Lesson ID: {model.Lesson_ID}");
                Console.WriteLine($"Passing Score: {model.Passing_score}");
                Console.WriteLine($"Instructions: {model.Instructions?.Substring(0, Math.Min(model.Instructions?.Length ?? 0, 50))}...");
                Console.WriteLine($"LessonSelectList count: {model.LessonSelectList?.Count() ?? 0}");
                
                // The LessonSelectList is coming back as null, which is okay for an edit operation
                // Clear the model state before validation to ignore binding issues with complex properties
                ModelState.Clear();
                
                // Manually add validation errors for required fields
                if (string.IsNullOrEmpty(model.Title))
                {
                    ModelState.AddModelError("Title", "Title is required");
                }
                
                if (model.Lesson_ID <= 0)
                {
                    ModelState.AddModelError("Lesson_ID", "A valid lesson must be selected");
                }
                
                if (model.Quiz_ID != id)
                {
                    Console.WriteLine($"ID mismatch: route ID {id} doesn't match model ID {model.Quiz_ID}");
                    ModelState.AddModelError("", $"ID mismatch: route ID {id} doesn't match model ID {model.Quiz_ID}");
                }
                
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("======= MODEL STATE ERRORS =======");
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            Console.WriteLine($"Validation error for {state.Key}: {error.ErrorMessage}");
                        }
                    }
                    
                    Console.WriteLine("Model state is invalid. Returning to edit form.");
                    
                    // Refresh lesson select list
                    model.LessonSelectList = await GetLessonsAsync();
                    Console.WriteLine($"Refreshed LessonSelectList with {model.LessonSelectList.Count()} items");
                    
                    return View(model);
                }

                Console.WriteLine($"Updating quiz: {model.Title}, Lesson ID: {model.Lesson_ID}");
                
                // Create a clean model with only the needed properties
                var quizToUpdate = new QuizRequest
                {
                    Quiz_ID = id,
                    Title = model.Title,
                    Lesson_ID = model.Lesson_ID,
                    Passing_score = model.Passing_score,
                    Instructions = model.Instructions
                };
                
                var success = await _quizManager.EditQuizAsync(id, quizToUpdate);
                
                if (!success)
                {
                    Console.WriteLine($"Failed to update quiz with ID: {id}");
                    TempData["ErrorMessage"] = "Failed to update quiz. The quiz may not exist or there was a server error.";
                    model.LessonSelectList = await GetLessonsAsync();
                    return View(model);
                }
                
                Console.WriteLine($"Quiz updated successfully: {model.Title}");
                TempData["SuccessMessage"] = $"Quiz '{model.Title}' updated successfully!";
                
                return RedirectToAction(nameof(AdminIndex));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating quiz: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner stack trace: {ex.InnerException.StackTrace}");
                }
                
                TempData["ErrorMessage"] = $"An error occurred while updating the quiz: {ex.Message}";
                model.LessonSelectList = await GetLessonsAsync();
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var quiz = await _quizManager.GetByIdAsync(id);
            if (quiz == null) return NotFound();

            await _quizManager.SoftDelete(quiz);

            return RedirectToAction(nameof(AdminIndex));
        }

        public async Task<IActionResult> CheckLessonsForQuiz()
        {
            var lessonItems = await GetLessonsAsync();
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
                
                var selectItems = await GetLessonsAsync();
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