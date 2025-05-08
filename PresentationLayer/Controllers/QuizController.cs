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
using DataAccessLayer.Entities;
using PresentationLayer.Models;

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
                    TempData["Error"] = "You must create a course and add lessons before creating quizzes.";
                    return RedirectToAction("Dashboard", "Instructor");
                }
                
                // If lessonId is provided, verify it belongs to the instructor
                if (lessonId.HasValue)
                {
                    bool isOwned = await IsLessonOwnedByInstructorAsync(lessonId.Value, currentUserId);
                    if (!isOwned)
                    {
                        TempData["Error"] = "You do not have permission to create a quiz for this lesson.";
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
                TempData["Error"] = "Error loading quiz creation page: " + ex.Message;
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
                TempData["Error"] = "You do not have permission to create a quiz for this lesson.";
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
                
                TempData["Success"] = $"Quiz '{model.Title}' created successfully!";
                
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
                
                TempData["Error"] = $"An error occurred while creating the quiz: {ex.Message}";
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
                TempData["Error"] = "You do not have permission to edit this quiz.";
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
                TempData["Error"] = "You do not have permission to edit a quiz for this lesson.";
                model.LessonSelectList = await GetLessonsForInstructorAsync(currentUserId);
                return View(model);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _quizManager.EditQuizAsync(id, model);
                    TempData["Success"] = "Quiz updated successfully!";
                    return RedirectToAction("Quizzes", "Instructor");
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error updating quiz: {ex.Message}";
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
                TempData["Error"] = "You do not have permission to delete this quiz.";
                return RedirectToAction("Quizzes", "Instructor");
            }
            
            try
            {
                await _quizManager.SoftDelete(quiz);
                TempData["Success"] = "Quiz deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting quiz: {ex.Message}";
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

        // Student-facing quiz details/take quiz page
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                // Get the quiz with all questions and answers
                var quiz = await _quizManager.FindAsync(id);
                if (quiz == null)
                {
                    TempData["Error"] = "Quiz not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Get the lesson to check course access
                var lesson = await _lessonManager.FindAsync(quiz.Lesson_ID);
                if (lesson == null)
                {
                    TempData["Error"] = "The associated lesson could not be found.";
                    return RedirectToAction("Index", "Home");
                }

                // Check if the student has access to the course
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                bool hasAccess = await _courseManager.GetStudentCourseAccessAsync(userId, lesson.Course_ID);
                if (!hasAccess)
                {
                    TempData["Error"] = "You do not have access to this quiz.";
                    return RedirectToAction("UserIndex", "Course");
                }

                return View(quiz);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // Submit quiz answers
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> SubmitQuiz(int quizId)
        {
            try
            {
                // Get the quiz
                var quiz = await _quizManager.FindAsync(quizId);
                if (quiz == null)
                {
                    TempData["Error"] = "Quiz not found.";
                    return RedirectToAction("Index", "Home");
                }

                // Verify student has access (similar to Details action)
                var lesson = await _lessonManager.FindAsync(quiz.Lesson_ID);
                if (lesson == null)
                {
                    TempData["Error"] = "The associated lesson could not be found.";
                    return RedirectToAction("Index", "Home");
                }

                string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var hasAccess = await _courseManager.GetStudentCourseAccessAsync(currentUserId, lesson.Course_ID);
                if (!hasAccess)
                {
                    TempData["Error"] = "You do not have access to this quiz.";
                    return RedirectToAction("MyCourses", "Student");
                }

                // Process the form data manually
                var answers = new Dictionary<int, int>();
                foreach (var key in Request.Form.Keys)
                {
                    // Check if the key starts with "question_"
                    if (key.StartsWith("question_"))
                    {
                        // Extract the question ID from the key (remove "question_" prefix)
                        string questionIdStr = key.Substring(9); // "question_".Length = 9
                        if (int.TryParse(questionIdStr, out int questionId))
                        {
                            // Get the selected answer ID
                            string answerValue = Request.Form[key];
                            if (int.TryParse(answerValue, out int answerId))
                            {
                                answers[questionId] = answerId;
                            }
                        }
                    }
                }

                // Calculate the score
                int totalScore = 0;
                int earnedScore = 0;
                int correctAnswers = 0;
                
                // Process each question
                foreach (var question in quiz.Questions)
                {
                    totalScore += question.Score;
                    
                    // Check if the student answered this question
                    if (answers.TryGetValue(question.QuestionID, out int selectedAnswerId))
                    {
                        // Get the selected answer
                        var selectedAnswer = question.Answers.FirstOrDefault(a => a.AnswerID == selectedAnswerId);
                        
                        // Check if it's correct
                        if (selectedAnswer != null && selectedAnswer.Answers == question.RightAns)
                        {
                            earnedScore += question.Score;
                            correctAnswers++;
                        }
                    }
                }
                
                // Calculate percentage
                int percentage = totalScore > 0 ? (int)Math.Round((double)earnedScore / totalScore * 100) : 0;
                bool passed = percentage >= quiz.Passing_score;
                
                // Store submission in database
                var submission = new Submission
                {
                    User_ID = currentUserId,
                    Quiz_ID = quizId,
                    Score = percentage,
                    Passed = passed,
                    DateSubmitted = DateTime.Now,
                    // Add this to ensure Status is not null
                    Status = passed ? "Passed" : "Failed"
                };
                
                _unitOfWork.GetRepository<Submission>().Create(submission);
                
                // Save submission first before creating student answers
                try 
                {
                    await _unitOfWork.SaveAsync();
                    Console.WriteLine($"[DEBUG] Successfully saved submission with ID: {submission.ID}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Error saving submission: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"[ERROR] Inner exception: {ex.InnerException.Message}");
                    }
                    throw; // Re-throw to be caught by outer catch block
                }
                
                // Now that we have the submission ID, store the individual question answers
                try 
                {
                    foreach (var questionAnswer in answers)
                    {
                        int questionId = questionAnswer.Key;
                        int answerId = questionAnswer.Value;
                        
                        // Verify the question and answer exist in the database
                        var questionExists = quiz.Questions.Any(q => q.QuestionID == questionId);
                        if (!questionExists)
                        {
                            Console.WriteLine($"[WARNING] Question with ID {questionId} not found in quiz {quizId}, skipping");
                            continue;
                        }
                        
                        // Get the question to check if the answer is correct
                        var question = quiz.Questions.FirstOrDefault(q => q.QuestionID == questionId);
                        bool isCorrect = false;
                        bool answerExists = false;
                        
                        if (question != null)
                        {
                            var selectedAnswer = question.Answers.FirstOrDefault(a => a.AnswerID == answerId);
                            answerExists = selectedAnswer != null;
                            isCorrect = (selectedAnswer != null && selectedAnswer.Answers == question.RightAns);
                            
                            if (!answerExists)
                            {
                                Console.WriteLine($"[WARNING] Answer with ID {answerId} not found for question {questionId}, skipping");
                                continue;
                            }
                        }
                        
                        // Create student answer record
                        var studentAnswer = new StudentAnswer
                        {
                            Submission_ID = submission.ID,
                            Question_ID = questionId,
                            Answer_ID = answerId,
                            IsCorrect = isCorrect
                        };
                        
                        _unitOfWork.GetRepository<StudentAnswer>().Create(studentAnswer);
                        
                        // Save each answer individually to isolate problems
                        try 
                        {
                            await _unitOfWork.SaveAsync();
                            Console.WriteLine($"[DEBUG] Successfully saved answer for question {questionId}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[ERROR] Error saving answer for question {questionId}: {ex.Message}");
                            if (ex.InnerException != null)
                            {
                                Console.WriteLine($"[ERROR] Inner exception: {ex.InnerException.Message}");
                            }
                            // Continue processing other answers even if one fails
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Error processing answers: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"[ERROR] Inner exception: {ex.InnerException.Message}");
                    }
                    // Continue to results even if some answers failed to save
                }
                
                // Store results in TempData for the results page
                TempData["CorrectAnswers"] = correctAnswers;
                TempData["TotalQuestions"] = quiz.Questions.Count();
                TempData["Score"] = percentage;
                TempData["PassingScore"] = quiz.Passing_score;
                TempData["Passed"] = passed;
                TempData["CourseId"] = lesson.Course_ID;
                TempData["QuizTitle"] = quiz.Title;
                
                // Add a success message
                TempData["Success"] = $"Quiz '{quiz.Title}' completed successfully! {(passed ? "You passed!" : "You didn't pass this time.")}";
                
                // Directly use the full route with both controller and action names
                return RedirectToAction("Results", "Quiz", new { id = quizId });
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database update exceptions specifically
                Console.WriteLine($"[ERROR] Database update exception: {dbEx.Message}");
                if (dbEx.InnerException != null)
                {
                    Console.WriteLine($"[ERROR] Inner exception: {dbEx.InnerException.Message}");
                    TempData["Error"] = $"Database error: {dbEx.InnerException.Message}";
                }
                else
                {
                    TempData["Error"] = $"Database error: {dbEx.Message}";
                }
                
                return RedirectToAction("Details", "Quiz", new { id = quizId });
            }
            catch (Exception ex)
            {
                // More detailed error logging
                Console.WriteLine($"[ERROR] SubmitQuiz exception: {ex.Message}");
                Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[ERROR] Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"[ERROR] Inner stack trace: {ex.InnerException.StackTrace}");
                    TempData["Error"] = $"Error submitting quiz: {ex.InnerException.Message}";
                }
                else
                {
                    TempData["Error"] = $"Error submitting quiz: {ex.Message}";
                }
                
                return RedirectToAction("Details", "Quiz", new { id = quizId });
            }
        }

        // Display quiz results
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Results(int id)
        {
            try
            {
                Console.WriteLine($"[DEBUG] Results action called with id: {id}");
                
                // Get the quiz
                var quiz = await _quizManager.FindAsync(id);
                if (quiz == null)
                {
                    Console.WriteLine($"[DEBUG] Quiz with id {id} not found");
                    TempData["Error"] = "Quiz not found.";
                    return RedirectToAction("Index", "Home");
                }

                Console.WriteLine($"[DEBUG] Found quiz: {quiz.Quiz_ID} - {quiz.Title}");
                Console.WriteLine($"[DEBUG] Checking TempData for Score: {TempData["Score"] != null}");

                // Check if we have results in TempData
                if (TempData["Score"] == null)
                {
                    Console.WriteLine("[DEBUG] No Score found in TempData, looking up submission in database");
                    
                    // Try to get the last submission for this student and quiz
                    string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    Console.WriteLine($"[DEBUG] Looking up submissions for user {userId} and quiz {id}");
                    
                    var submission = await _unitOfWork.GetRepository<Submission>()
                        .FindByCondition(s => s.Quiz_ID == id && s.User_ID == userId)
                        .OrderByDescending(s => s.DateSubmitted)
                        .FirstOrDefaultAsync();

                    if (submission != null)
                    {
                        Console.WriteLine($"[DEBUG] Found submission with ID: {submission.ID}, Score: {submission.Score}");
                        
                        // Get the lesson for navigation back to course
                        var lesson = await _unitOfWork.GetRepository<Lesson>()
                            .FindByCondition(l => l.Quiz_ID == id)
                            .FirstOrDefaultAsync();
                        
                        int courseId = lesson?.Course_ID ?? 0;
                        Console.WriteLine($"[DEBUG] Found lesson with CourseId: {courseId}");

                        // Get the number of correct answers
                        int correctAnswers = await _unitOfWork.GetRepository<StudentAnswer>()
                            .FindByCondition(sa => sa.Submission_ID == submission.ID && sa.IsCorrect)
                            .CountAsync();
                        
                        Console.WriteLine($"[DEBUG] Found {correctAnswers} correct answers out of {quiz.Questions.Count()} questions");

                        int totalQuestions = quiz.Questions.Count();

                        // Create view model from database
                        var viewModel = new QuizResultViewModel
                        {
                            QuizId = id,
                            QuizTitle = quiz.Title,
                            CorrectAnswers = correctAnswers,
                            TotalQuestions = totalQuestions,
                            Score = submission.Score,
                            PassingScore = quiz.Passing_score,
                            Passed = submission.Passed,
                            CourseId = courseId
                        };

                        Console.WriteLine("[DEBUG] Created viewModel from database, returning view");
                        return View(viewModel);
                    }
                    else
                    {
                        Console.WriteLine("[DEBUG] No submission found in database");
                        // No results found, redirect to the quiz details
                        TempData["Info"] = "No quiz results found. You may need to take the quiz first.";
                        return RedirectToAction("Details", new { id });
                    }
                }

                Console.WriteLine("[DEBUG] Creating viewModel from TempData");
                
                // Create view model from TempData
                var model = new QuizResultViewModel
                {
                    QuizId = id,
                    QuizTitle = TempData["QuizTitle"]?.ToString(),
                    CorrectAnswers = TempData["CorrectAnswers"] != null ? Convert.ToInt32(TempData["CorrectAnswers"]) : 0,
                    TotalQuestions = TempData["TotalQuestions"] != null ? Convert.ToInt32(TempData["TotalQuestions"]) : 0,
                    Score = TempData["Score"] != null ? Convert.ToInt32(TempData["Score"]) : 0,
                    PassingScore = TempData["PassingScore"] != null ? Convert.ToInt32(TempData["PassingScore"]) : 0,
                    Passed = TempData["Passed"] != null && Convert.ToBoolean(TempData["Passed"]),
                    CourseId = TempData["CourseId"] != null ? Convert.ToInt32(TempData["CourseId"]) : 0
                };

                Console.WriteLine($"[DEBUG] Created model with Score: {model.Score}, Passed: {model.Passed}");
                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Results action exception: {ex.Message}");
                Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[ERROR] Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"[ERROR] Inner stack trace: {ex.InnerException.StackTrace}");
                }
                
                TempData["Error"] = $"Error displaying quiz results: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        // DIAGNOSTIC PAGE: List quizzes and lessons for linking
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> LinkQuizzesToLessons()
        {
            // Get all quizzes
            var quizzes = await _quizManager.FindAllAsync();
            
            // Get all lessons
            var lessons = await _lessonManager.FindAllAsync();
            
            ViewBag.Lessons = lessons;
            return View(quizzes);
        }
        
        // API to link a quiz to a lesson
        [HttpPost]
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> LinkQuizToLesson([FromBody] LinkQuizLessonRequest request)
        {
            try
            {
                // Get the lesson
                var lesson = await _lessonManager.FindAsync(request.LessonId);
                if (lesson == null)
                {
                    return Json(new { success = false, message = "Lesson not found" });
                }
                
                // Get the quiz
                var quiz = await _quizManager.FindAsync(request.QuizId);
                if (quiz == null)
                {
                    return Json(new { success = false, message = "Quiz not found" });
                }
                
                // Update the lesson's Quiz_ID
                var lessonRequest = new BusinessLogicLayer.DTOs.LessonDtos.LessonRequest
                {
                    Title = lesson.Title,
                    Description = lesson.Description,
                    Duration = lesson.Duration,
                    VideoUri = lesson.VideoUri,
                    LessonOrder = lesson.LessonOrder,
                    Course_ID = lesson.Course_ID,
                    Quiz_ID = request.QuizId
                };
                
                var success = await _lessonManager.EditLessonAsync(request.LessonId, lessonRequest);
                
                // Also update the quiz's Lesson_ID
                var quizRequest = new BusinessLogicLayer.DTOs.QuizDtos.QuizRequest
                {
                    Title = quiz.Title,
                    Instructions = quiz.Instructions,
                    Passing_score = quiz.Passing_score, 
                    Lesson_ID = request.LessonId
                };
                
                await _quizManager.EditQuizAsync(request.QuizId, quizRequest);
                
                return Json(new { success = success, message = success ? "Quiz linked to lesson successfully" : "Failed to link quiz to lesson" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        public class LinkQuizLessonRequest
        {
            public int QuizId { get; set; }
            public int LessonId { get; set; }
        }

        // Direct database level diagnostic and fix for quiz-lesson relationships
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> DirectFixRelationships()
        {
            var results = new Dictionary<string, object>();
            try
            {
                // Get all quizzes
                var quizzes = await _unitOfWork.Quizzes.FindAllAsync(
                    q => true,
                    include: q => q.Include(q => q.Lesson)
                );
                results["1_QuizzesCount"] = quizzes.Count();
                
                // Get all lessons
                var lessons = await _unitOfWork.GetRepository<Lesson>().FindAllAsync(
                    l => true,
                    include: l => l.Include(l => l.Quiz)
                );
                results["2_LessonsCount"] = lessons.Count();
                
                // Check for lessons with quiz_id = null that have a quiz pointing to them
                var issueCount = 0;
                foreach (var quiz in quizzes)
                {
                    var lessonId = quiz.Lesson_ID;
                    var lesson = lessons.FirstOrDefault(l => l.Lesson_ID == lessonId);
                    
                    if (lesson != null && lesson.Quiz_ID == null)
                    {
                        // Fix the relationship - update the lesson
                        lesson.Quiz_ID = quiz.Quiz_ID;
                        _unitOfWork.GetRepository<Lesson>().Update(lesson);
                        issueCount++;
                    }
                }
                
                // Save changes if any were made
                if (issueCount > 0)
                {
                    await _unitOfWork.SaveAsync();
                    results["3_FixedRelationships"] = issueCount;
                }
                else
                {
                    results["3_FixedRelationships"] = "No issues found";
                }
                
                // Now get an updated list of lessons to verify
                var updatedLessons = await _unitOfWork.GetRepository<Lesson>().FindAllAsync(
                    l => true,
                    include: l => l.Include(l => l.Quiz)
                );
                
                results["4_UpdatedLessons"] = updatedLessons.Select(l => new 
                {
                    l.Lesson_ID,
                    l.Title,
                    l.Quiz_ID,
                    HasQuiz = l.Quiz != null,
                    QuizTitle = l.Quiz?.Title
                }).ToList();
                
                return Json(results);
            }
            catch (Exception ex)
            {
                results["Error"] = ex.Message;
                results["StackTrace"] = ex.StackTrace;
                return Json(results);
            }
        }

        // Direct assignment of quiz to lesson (for diagnostics)
        [Authorize(Roles = "Teacher,Admin")]
        public async Task<IActionResult> DirectAssign(int quizId, int lessonId)
        {
            try
            {
                // Get the lesson entity directly
                var lesson = await _unitOfWork.GetRepository<Lesson>().FindByCondition(
                    l => l.Lesson_ID == lessonId
                ).FirstOrDefaultAsync();
                
                if (lesson == null)
                {
                    return Json(new { success = false, message = "Lesson not found" });
                }
                
                // Get the quiz entity directly
                var quiz = await _unitOfWork.Quizzes.FindByCondition(
                    q => q.Quiz_ID == quizId
                ).FirstOrDefaultAsync();
                
                if (quiz == null)
                {
                    return Json(new { success = false, message = "Quiz not found" });
                }
                
                // Update both sides of the relationship
                lesson.Quiz_ID = quizId;
                quiz.Lesson_ID = lessonId;
                
                // Save both entities
                _unitOfWork.GetRepository<Lesson>().Update(lesson);
                _unitOfWork.Quizzes.Update(quiz);
                await _unitOfWork.SaveAsync();
                
                return Json(new { 
                    success = true, 
                    message = $"Successfully linked Quiz {quizId} to Lesson {lessonId}",
                    lesson = new { lesson.Lesson_ID, lesson.Title, lesson.Quiz_ID },
                    quiz = new { quiz.Quiz_ID, quiz.Title, quiz.Lesson_ID }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message, stackTrace = ex.StackTrace });
            }
        }
    }
} 