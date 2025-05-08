using AutoMapper;
using BusinessLogicLayer.DTOs.QuizDtos;
using BusinessLogicLayer.Manager.QuestionManager;
using BusinessLogicLayer.Manager.QuizManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using Microsoft.Extensions.Configuration;

namespace PresentationLayer.Controllers
{
    public class QuestionController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IQuestionManager _questionManager;
        private readonly IQuizManager _quizManager;
        private readonly IConfiguration _configuration;

        public QuestionController(
            IMapper mapper,
            IQuestionManager questionManager,
            IQuizManager quizManager,
            IConfiguration configuration)
        {
            _mapper = mapper;
            _questionManager = questionManager;
            _quizManager = quizManager;
            _configuration = configuration;
        }

        private async Task<IEnumerable<SelectListItem>> GetQuizzesAsync()
        {
            var quizzes = await _quizManager.FindAllAsync();
            return quizzes.Select(q => new SelectListItem
            {
                Value = q.Quiz_ID.ToString(),
                Text = q.Title
            });
        }

        public async Task<IActionResult> Index(int quizId)
        {
            var questionDtos = await _questionManager.GetQuestionsByQuizAsync(quizId);
            ViewBag.QuizId = quizId;
            var quiz = await _quizManager.FindAsync(quizId);
            if (quiz != null)
            {
                ViewBag.QuizTitle = quiz.Title;
            }
            return View(questionDtos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var questionDto = await _questionManager.FindAsync(id);
            if (questionDto == null) return NotFound();

            return View(questionDto);
        }

        public async Task<IActionResult> Create(int quizId)
        {
            var model = new QuestionRequest
            {
                QuizID = quizId,
                Answers = new List<string> { "", "", "", "" }
            };
            
            ViewBag.Quizzes = await GetQuizzesAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(QuestionRequest model)
        {
            // Add logging to see what's being received
            var answerCount = model.Answers?.Count ?? 0;
            var validAnswers = model.Answers?.Where(a => !string.IsNullOrWhiteSpace(a)).ToList() ?? new List<string>();
            var validAnswerCount = validAnswers.Count;
            
            TempData["Debug"] = $"Received {answerCount} answers, {validAnswerCount} valid. RightAns: {model.RightAns ?? "null"}, QuizID: {model.QuizID}";
            
            try
            {
                // Validate input
                if (model.QuizID <= 0)
                {
                    TempData["ErrorMessage"] = "Invalid Quiz ID. Please try again.";
                    model.Answers = model.Answers ?? new List<string> { "", "", "", "" };
                    ViewBag.Quizzes = await GetQuizzesAsync();
                    return View(model);
                }
                
                // Validate answers
                if (validAnswers.Count == 0)
                {
                    TempData["ErrorMessage"] = "At least one answer option is required.";
                    model.Answers = model.Answers ?? new List<string> { "", "", "", "" };
                    ViewBag.Quizzes = await GetQuizzesAsync();
                    return View(model);
                }

                // Make sure RightAns is set to a valid answer
                if (string.IsNullOrWhiteSpace(model.RightAns) || !validAnswers.Contains(model.RightAns))
                {
                    model.RightAns = validAnswers.First();
                    TempData["Debug"] += $"; Auto-set RightAns to '{model.RightAns}'";
                }

                // Trim and limit answers to 200 chars
                for (int i = 0; i < validAnswers.Count; i++)
                {
                    validAnswers[i] = validAnswers[i].Trim();
                    if (validAnswers[i].Length > 200)
                    {
                        validAnswers[i] = validAnswers[i].Substring(0, 200);
                    }
                }

                // Get database context
                var dbContext = _questionManager.GetUnitOfWork().GetDbContext();
                
                // Check if quiz actually exists directly in the database
                var quizCheck = await dbContext.Quizzes.FirstOrDefaultAsync(q => q.Quiz_ID == model.QuizID);
                if (quizCheck == null)
                {
                    TempData["ErrorMessage"] = $"Direct DB check failed: Quiz with ID {model.QuizID} doesn't exist in database.";
                    model.Answers = model.Answers ?? new List<string> { "", "", "", "" };
                    ViewBag.Quizzes = await GetQuizzesAsync();
                    return View(model);
                }
                
                TempData["Debug"] += $"; Direct DB check: Quiz found with ID {quizCheck.Quiz_ID}, Title: {quizCheck.Title}";
                
                // Try raw SQL approach with explicit transaction
                var connection = dbContext.Database.GetDbConnection();
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // First verify quiz exists using raw SQL
                        int questionId = 0;
                        using (var checkCmd = connection.CreateCommand())
                        {
                            checkCmd.Transaction = transaction;
                            checkCmd.CommandText = "SELECT COUNT(*) FROM Quizzes WHERE Quiz_ID = @QuizID";
                            
                            var param = checkCmd.CreateParameter();
                            param.ParameterName = "@QuizID";
                            param.Value = model.QuizID;
                            checkCmd.Parameters.Add(param);
                            
                            var quizCount = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());
                            if (quizCount == 0)
                            {
                                await transaction.RollbackAsync();
                                TempData["ErrorMessage"] = $"Raw SQL check failed: Quiz with ID {model.QuizID} doesn't exist.";
                                model.Answers = model.Answers ?? new List<string> { "", "", "", "" };
                                ViewBag.Quizzes = await GetQuizzesAsync();
                                return View(model);
                            }
                        }
                        
                        // Insert question
                        using (var questionCmd = connection.CreateCommand())
                        {
                            questionCmd.Transaction = transaction;
                            questionCmd.CommandText = @"
                                INSERT INTO Questions (QuizID, QuestionText, Score, RightAns)
                                VALUES (@QuizID, @QuestionText, @Score, @RightAns);
                                SELECT SCOPE_IDENTITY();";
                                
                            var quizIdParam = questionCmd.CreateParameter();
                            quizIdParam.ParameterName = "@QuizID";
                            quizIdParam.Value = model.QuizID;
                            questionCmd.Parameters.Add(quizIdParam);
                            
                            var questionTextParam = questionCmd.CreateParameter();
                            questionTextParam.ParameterName = "@QuestionText";
                            questionTextParam.Value = model.QuestionText?.Trim() ?? "";
                            questionCmd.Parameters.Add(questionTextParam);
                            
                            var scoreParam = questionCmd.CreateParameter();
                            scoreParam.ParameterName = "@Score";
                            scoreParam.Value = model.Score;
                            questionCmd.Parameters.Add(scoreParam);
                            
                            var rightAnsParam = questionCmd.CreateParameter();
                            rightAnsParam.ParameterName = "@RightAns";
                            rightAnsParam.Value = model.RightAns?.Trim() ?? validAnswers.First();
                            questionCmd.Parameters.Add(rightAnsParam);
                            
                            // Get the new question ID
                            var result = await questionCmd.ExecuteScalarAsync();
                            questionId = Convert.ToInt32(result);
                        }
                        
                        // Insert answers
                        foreach (var answerText in validAnswers)
                        {
                            using (var answerCmd = connection.CreateCommand())
                            {
                                answerCmd.Transaction = transaction;
                                answerCmd.CommandText = "INSERT INTO Answers (QuestionID, Answers) VALUES (@QuestionID, @AnswerText)";
                                
                                var questionIdParam = answerCmd.CreateParameter();
                                questionIdParam.ParameterName = "@QuestionID";
                                questionIdParam.Value = questionId;
                                answerCmd.Parameters.Add(questionIdParam);
                                
                                var answerTextParam = answerCmd.CreateParameter();
                                answerTextParam.ParameterName = "@AnswerText";
                                answerTextParam.Value = answerText;
                                answerCmd.Parameters.Add(answerTextParam);
                                
                                await answerCmd.ExecuteNonQueryAsync();
                            }
                        }
                        
                        // Commit transaction if everything worked
                        await transaction.CommitAsync();
                        
                        TempData["SuccessMessage"] = "Question created successfully!";
                        return RedirectToAction(nameof(Index), new { quizId = model.QuizID });
                    }
                    catch (Exception ex)
                    {
                        // Roll back transaction if anything fails
                        await transaction.RollbackAsync();
                        TempData["ErrorMessage"] = $"SQL Error: {ex.Message}";
                        TempData["Debug"] += $"; SQL exception: {ex.ToString()}";
                        model.Answers = model.Answers ?? new List<string> { "", "", "", "" };
                        ViewBag.Quizzes = await GetQuizzesAsync();
                        return View(model);
                    }
                    finally
                    {
                        if (connection.State == System.Data.ConnectionState.Open)
                        {
                            await connection.CloseAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                TempData["Debug"] += $"; Exception: {ex.ToString()}";
                model.Answers = model.Answers ?? new List<string> { "", "", "", "" };
                ViewBag.Quizzes = await GetQuizzesAsync();
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var question = await _questionManager.GetByIdAsync(id);
            if (question == null) return NotFound();

            var model = _mapper.Map<QuestionRequest>(question);
            
            if (question.Answers != null && question.Answers.Any())
            {
                model.Answers = question.Answers.Select(a => a.Answers).ToList();
            }
            else
            {
                model.Answers = new List<string> { "", "", "", "" };
            }
            
            ViewBag.Quizzes = await GetQuizzesAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, QuestionRequest model)
        {
            // Add logging to see what's being received
            var answerCount = model.Answers?.Count ?? 0;
            var validAnswers = model.Answers?.Where(a => !string.IsNullOrWhiteSpace(a)).ToList() ?? new List<string>();
            var validAnswerCount = validAnswers.Count;
            
            TempData["Debug"] = $"Edit: Received {answerCount} answers, {validAnswerCount} valid. RightAns: {model.RightAns ?? "null"}";
            
            try
            {
                // Validate input
                if (model.QuizID <= 0)
                {
                    TempData["ErrorMessage"] = "Invalid Quiz ID. Please try again.";
                    model.Answers = model.Answers ?? new List<string> { "", "", "", "" };
                    ViewBag.Quizzes = await GetQuizzesAsync();
                    return View(model);
                }
                
                // Validate answers
                if (validAnswers.Count == 0)
                {
                    TempData["ErrorMessage"] = "At least one answer option is required.";
                    model.Answers = model.Answers ?? new List<string> { "", "", "", "" };
                    ViewBag.Quizzes = await GetQuizzesAsync();
                    return View(model);
                }

                // Make sure RightAns is set to a valid answer
                if (string.IsNullOrWhiteSpace(model.RightAns) || !validAnswers.Contains(model.RightAns))
                {
                    model.RightAns = validAnswers.First();
                    TempData["Debug"] += $"; Auto-set RightAns to '{model.RightAns}'";
                }

                // Trim and limit answers to 200 chars
                for (int i = 0; i < validAnswers.Count; i++)
                {
                    validAnswers[i] = validAnswers[i].Trim();
                    if (validAnswers[i].Length > 200)
                    {
                        validAnswers[i] = validAnswers[i].Substring(0, 200);
                    }
                }

                // Get database context
                var dbContext = _questionManager.GetUnitOfWork().GetDbContext();
                
                // Check if quiz actually exists directly in the database
                var quizCheck = await dbContext.Quizzes.FirstOrDefaultAsync(q => q.Quiz_ID == model.QuizID);
                if (quizCheck == null)
                {
                    TempData["ErrorMessage"] = $"Direct DB check failed: Quiz with ID {model.QuizID} doesn't exist in database.";
                    model.Answers = model.Answers ?? new List<string> { "", "", "", "" };
                    ViewBag.Quizzes = await GetQuizzesAsync();
                    return View(model);
                }
                
                // Check if the question exists
                var questionCheck = await dbContext.Questions.FirstOrDefaultAsync(q => q.QuestionID == id);
                if (questionCheck == null)
                {
                    TempData["ErrorMessage"] = $"Question with ID {id} not found.";
                    model.Answers = model.Answers ?? new List<string> { "", "", "", "" };
                    ViewBag.Quizzes = await GetQuizzesAsync();
                    return View(model);
                }
                
                TempData["Debug"] += $"; Direct DB check: Quiz found with ID {quizCheck.Quiz_ID}, Title: {quizCheck.Title}";
                
                // Try raw SQL approach with explicit transaction
                var connection = dbContext.Database.GetDbConnection();
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Update the question
                        using (var updateCmd = connection.CreateCommand())
                        {
                            updateCmd.Transaction = transaction;
                            updateCmd.CommandText = @"
                                UPDATE Questions 
                                SET QuizID = @QuizID, 
                                    QuestionText = @QuestionText, 
                                    Score = @Score, 
                                    RightAns = @RightAns
                                WHERE QuestionID = @QuestionID";
                            
                            var questionIdParam = updateCmd.CreateParameter();
                            questionIdParam.ParameterName = "@QuestionID";
                            questionIdParam.Value = id;
                            updateCmd.Parameters.Add(questionIdParam);
                            
                            var quizIdParam = updateCmd.CreateParameter();
                            quizIdParam.ParameterName = "@QuizID";
                            quizIdParam.Value = model.QuizID;
                            updateCmd.Parameters.Add(quizIdParam);
                            
                            var questionTextParam = updateCmd.CreateParameter();
                            questionTextParam.ParameterName = "@QuestionText";
                            questionTextParam.Value = model.QuestionText?.Trim() ?? "";
                            updateCmd.Parameters.Add(questionTextParam);
                            
                            var scoreParam = updateCmd.CreateParameter();
                            scoreParam.ParameterName = "@Score";
                            scoreParam.Value = model.Score;
                            updateCmd.Parameters.Add(scoreParam);
                            
                            var rightAnsParam = updateCmd.CreateParameter();
                            rightAnsParam.ParameterName = "@RightAns";
                            rightAnsParam.Value = model.RightAns?.Trim() ?? validAnswers.First();
                            updateCmd.Parameters.Add(rightAnsParam);
                            
                            await updateCmd.ExecuteNonQueryAsync();
                        }
                        
                        // Delete existing answers
                        using (var deleteCmd = connection.CreateCommand())
                        {
                            deleteCmd.Transaction = transaction;
                            deleteCmd.CommandText = "DELETE FROM Answers WHERE QuestionID = @QuestionID";
                            
                            var questionIdParam = deleteCmd.CreateParameter();
                            questionIdParam.ParameterName = "@QuestionID";
                            questionIdParam.Value = id;
                            deleteCmd.Parameters.Add(questionIdParam);
                            
                            await deleteCmd.ExecuteNonQueryAsync();
                        }
                        
                        // Insert new answers
                        foreach (var answerText in validAnswers)
                        {
                            using (var answerCmd = connection.CreateCommand())
                            {
                                answerCmd.Transaction = transaction;
                                answerCmd.CommandText = "INSERT INTO Answers (QuestionID, Answers) VALUES (@QuestionID, @AnswerText)";
                                
                                var questionIdParam = answerCmd.CreateParameter();
                                questionIdParam.ParameterName = "@QuestionID";
                                questionIdParam.Value = id;
                                answerCmd.Parameters.Add(questionIdParam);
                                
                                var answerTextParam = answerCmd.CreateParameter();
                                answerTextParam.ParameterName = "@AnswerText";
                                answerTextParam.Value = answerText;
                                answerCmd.Parameters.Add(answerTextParam);
                                
                                await answerCmd.ExecuteNonQueryAsync();
                            }
                        }
                        
                        // Commit transaction if everything worked
                        await transaction.CommitAsync();
                        
                        TempData["SuccessMessage"] = "Question updated successfully!";
                        return RedirectToAction(nameof(Index), new { quizId = model.QuizID });
                    }
                    catch (Exception ex)
                    {
                        // Roll back transaction if anything fails
                        await transaction.RollbackAsync();
                        TempData["ErrorMessage"] = $"SQL Error: {ex.Message}";
                        TempData["Debug"] += $"; SQL exception: {ex.ToString()}";
                        model.Answers = model.Answers ?? new List<string> { "", "", "", "" };
                        ViewBag.Quizzes = await GetQuizzesAsync();
                        return View(model);
                    }
                    finally
                    {
                        if (connection.State == System.Data.ConnectionState.Open)
                        {
                            await connection.CloseAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                TempData["Debug"] += $"; Exception: {ex.ToString()}";
                model.Answers = model.Answers ?? new List<string> { "", "", "", "" };
                ViewBag.Quizzes = await GetQuizzesAsync();
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var question = await _questionManager.GetByIdAsync(id);
            if (question == null) return NotFound();

            var quizId = question.QuizID;
            await _questionManager.DeleteQuestion(id);

            return RedirectToAction(nameof(Index), new { quizId });
        }

        // Completely new approach using raw SQL for diagnostic purposes
        [HttpGet]
        public async Task<IActionResult> AddQuestionWithSQL(int quizId)
        {
            try
            {
                var dbContext = _questionManager.GetUnitOfWork().GetDbContext();
                var connection = dbContext.Database.GetDbConnection();
                
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    // Create a question with diagnostic name and timestamp
                    command.CommandText = @"
                        INSERT INTO Questions (QuizID, QuestionText, Score, RightAns)
                        VALUES (@QuizID, @QuestionText, @Score, @RightAns);
                        SELECT SCOPE_IDENTITY();
                    ";

                    // Add parameters
                    var questionText = $"SQL Test Question - {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";
                    
                    var quizIdParam = command.CreateParameter();
                    quizIdParam.ParameterName = "@QuizID";
                    quizIdParam.Value = quizId;
                    command.Parameters.Add(quizIdParam);
                    
                    var questionTextParam = command.CreateParameter();
                    questionTextParam.ParameterName = "@QuestionText";
                    questionTextParam.Value = questionText;
                    command.Parameters.Add(questionTextParam);
                    
                    var scoreParam = command.CreateParameter();
                    scoreParam.ParameterName = "@Score";
                    scoreParam.Value = 10;
                    command.Parameters.Add(scoreParam);
                    
                    var rightAnsParam = command.CreateParameter();
                    rightAnsParam.ParameterName = "@RightAns";
                    rightAnsParam.Value = "Test Answer";
                    command.Parameters.Add(rightAnsParam);
                    
                    // Execute and get new ID
                    var questionId = Convert.ToInt32(await command.ExecuteScalarAsync());
                    
                    // Create some test answers
                    var answerTexts = new[] { "Test Answer", "Wrong Answer 1", "Wrong Answer 2" };
                    
                    foreach (var answerText in answerTexts)
                    {
                        using (var answerCommand = connection.CreateCommand())
                        {
                            answerCommand.CommandText = @"
                                INSERT INTO Answers (QuestionID, Answers)
                                VALUES (@QuestionID, @AnswerText);
                            ";
                            
                            var questionIdParam = answerCommand.CreateParameter();
                            questionIdParam.ParameterName = "@QuestionID";
                            questionIdParam.Value = questionId;
                            answerCommand.Parameters.Add(questionIdParam);
                            
                            var answerTextParam = answerCommand.CreateParameter();
                            answerTextParam.ParameterName = "@AnswerText";
                            answerTextParam.Value = answerText;
                            answerCommand.Parameters.Add(answerTextParam);
                            
                            await answerCommand.ExecuteNonQueryAsync();
                        }
                    }
                }
                
                TempData["SuccessMessage"] = "Test question created using raw SQL commands!";
                return RedirectToAction(nameof(Index), new { quizId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"SQL Error: {ex.Message}";
                return RedirectToAction(nameof(Index), new { quizId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DebugCreateQuestion(int quizId)
        {
            try
            {
                // Create a simplified question with minimal data for debugging
                var questionText = "Debug Question " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var dbContext = _questionManager.GetUnitOfWork().GetDbContext();
                
                // First create the question
                var question = new Question
                {
                    QuizID = quizId,
                    Score = 10,
                    QuestionText = questionText,
                    RightAns = "Debug Answer" 
                };
                
                // Add and save to get the ID
                dbContext.Questions.Add(question);
                await dbContext.SaveChangesAsync();
                
                // Then create an answer
                var answer = new Answer
                {
                    QuestionID = question.QuestionID,
                    Answers = "Debug Answer"
                };
                
                // Add and save the answer
                dbContext.Answers.Add(answer);
                await dbContext.SaveChangesAsync();
                
                TempData["SuccessMessage"] = "Debug question created successfully!";
                return RedirectToAction(nameof(Index), new { quizId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Debug error: {ex.Message}";
                return RedirectToAction(nameof(Index), new { quizId });
            }
        }

        // Add debug method to list available quizzes
        [HttpGet]
        public async Task<IActionResult> ListAvailableQuizzes()
        {
            try
            {
                var dbContext = _questionManager.GetUnitOfWork().GetDbContext();
                var quizzes = await dbContext.Quizzes
                    .Include(q => q.Lesson)
                    .ToListAsync();
                    
                var quizList = quizzes.Select(q => new
                {
                    QuizID = q.Quiz_ID,
                    Title = q.Title,
                    LessonID = q.Lesson_ID,
                    LessonTitle = q.Lesson?.Title ?? "Unknown"
                }).ToList();
                
                ViewBag.Quizzes = quizList;
                ViewBag.QuizCount = quizList.Count;
                
                return View("DebugQuizzes");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error listing quizzes: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }
    }
} 