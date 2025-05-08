using AutoMapper;
using BusinessLogicLayer.DTOs.QuizDtos;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogicLayer.Manager.QuestionManager
{
    public class QuestionManager : IQuestionManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public QuestionManager(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<QuestionDto>> FindAllAsync()
        {
            var questions = await _unitOfWork.GetRepository<Question>()
                .FindAll()
                .Include(q => q.Quiz)
                .Include(q => q.Answers)
                .ToListAsync();

            return _mapper.Map<IEnumerable<QuestionDto>>(questions);
        }

        public async Task<QuestionDto> FindAsync(int id)
        {
            var question = await _unitOfWork.GetRepository<Question>()
                .FindByCondition(q => q.QuestionID == id)
                .Include(q => q.Quiz)
                .Include(q => q.Answers)
                .FirstOrDefaultAsync();

            return _mapper.Map<QuestionDto>(question);
        }

        public async Task<QuestionDto> GetByIdAsync(int id)
        {
            var question = await _unitOfWork.GetRepository<Question>()
                .FindByCondition(q => q.QuestionID == id)
                .Include(q => q.Quiz)
                .Include(q => q.Answers)
                .FirstOrDefaultAsync();

            return _mapper.Map<QuestionDto>(question);
        }

        public async Task<bool> CreateQuestionAsync(QuestionRequest model)
        {
            try
            {
                // Simple logging for debugging
                System.Diagnostics.Debug.WriteLine($"Creating question with QuizID: {model.QuizID}, RightAns: {model.RightAns ?? "null"}");
                System.Diagnostics.Debug.WriteLine($"Answer count: {model.Answers?.Count ?? 0}");
                
                // Ensure we have answer data
                if (model.Answers == null)
                {
                    model.Answers = new List<string>();
                }
                
                // Trim all answers before filtering
                for (int i = 0; i < model.Answers.Count; i++)
                {
                    if (model.Answers[i] != null)
                    {
                        model.Answers[i] = model.Answers[i].Trim();
                    }
                }
                
                // Filter out empty answers and enforce length limit
                var validAnswers = model.Answers
                    .Where(a => !string.IsNullOrWhiteSpace(a))
                    .Select(a => a.Length > 200 ? a.Substring(0, 200) : a)
                    .ToList();
                
                System.Diagnostics.Debug.WriteLine($"Valid answers count: {validAnswers.Count}");
                
                // If we have no valid answers, we can't create a question
                if (!validAnswers.Any())
                {
                    System.Diagnostics.Debug.WriteLine("No valid answers provided");
                    return false;
                }

                // Ensure RightAns is one of the valid answers
                string rightAns = model.RightAns?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(rightAns) || !validAnswers.Contains(rightAns))
                {
                    rightAns = validAnswers.First();
                    System.Diagnostics.Debug.WriteLine($"Setting RightAns to first valid answer: {rightAns}");
                }

                // Access DbContext directly for more control
                var dbContext = _unitOfWork.GetDbContext();
                
                try
                {
                    // Start a transaction
                    await dbContext.Database.BeginTransactionAsync();
                    
                    // Create the question 
                    var question = new Question
                    {
                        QuizID = model.QuizID,
                        Score = model.Score,
                        QuestionText = model.QuestionText?.Trim() ?? "",
                        RightAns = rightAns 
                    };
                    
                    System.Diagnostics.Debug.WriteLine($"Creating question: {question.QuestionText}, RightAns: {question.RightAns}");
                    
                    // Add the question directly to DbContext
                    dbContext.Questions.Add(question);
                    await dbContext.SaveChangesAsync();
                    
                    System.Diagnostics.Debug.WriteLine($"Question created with ID: {question.QuestionID}");
                    
                    // Create answers directly with DbContext
                    foreach (var answerText in validAnswers)
                    {
                        string cleanAnswerText = answerText;
                        if (cleanAnswerText.Length > 200)
                        {
                            cleanAnswerText = cleanAnswerText.Substring(0, 200);
                        }
                        
                        var answer = new Answer
                        {
                            QuestionID = question.QuestionID,
                            Answers = cleanAnswerText
                        };
                        
                        System.Diagnostics.Debug.WriteLine($"Creating answer: {cleanAnswerText}");
                        dbContext.Answers.Add(answer);
                    }
                    
                    // Save all answers at once
                    await dbContext.SaveChangesAsync();
                    System.Diagnostics.Debug.WriteLine("All answers saved successfully");
                    
                    // Commit the transaction
                    await dbContext.Database.CommitTransactionAsync();
                    System.Diagnostics.Debug.WriteLine("Transaction committed successfully");
                    
                    return true;
                }
                catch (Exception ex)
                {
                    // Rollback transaction in case of an error
                    await dbContext.Database.RollbackTransactionAsync();
                    
                    System.Diagnostics.Debug.WriteLine($"Transaction rolled back due to error: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }
                    
                    throw; // Re-throw to be caught by the outer catch
                }
            }
            catch (Exception ex)
            {
                // Log the exception details
                System.Diagnostics.Debug.WriteLine($"Error creating question: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                return false;
            }
        }

        public async Task<bool> EditQuestionAsync(int id, QuestionRequest model)
        {
            try
            {
                // Simple logging for debugging
                System.Diagnostics.Debug.WriteLine($"Editing question {id} with QuizID: {model.QuizID}, RightAns: {model.RightAns ?? "null"}");
                System.Diagnostics.Debug.WriteLine($"Answer count: {model.Answers?.Count ?? 0}");

                // Ensure we have answer data
                if (model.Answers == null)
                {
                    model.Answers = new List<string>();
                }
                
                // Trim all answers before filtering
                for (int i = 0; i < model.Answers.Count; i++)
                {
                    if (model.Answers[i] != null)
                    {
                        model.Answers[i] = model.Answers[i].Trim();
                    }
                }
                
                // Filter out empty answers and enforce length limit
                var validAnswers = model.Answers
                    .Where(a => !string.IsNullOrWhiteSpace(a))
                    .Select(a => a.Length > 200 ? a.Substring(0, 200) : a)
                    .ToList();
                
                System.Diagnostics.Debug.WriteLine($"Valid answers count: {validAnswers.Count}");

                // Access DbContext directly for more control
                var dbContext = _unitOfWork.GetDbContext();
                
                try
                {
                    // Start a transaction
                    await dbContext.Database.BeginTransactionAsync();
                    
                    // Get the question directly from the context
                    var question = await dbContext.Questions.FindAsync(id);
                    if (question == null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Question with ID {id} not found");
                        return false;
                    }

                    // Update question fields
                    question.Score = model.Score;
                    question.QuizID = model.QuizID;
                    question.QuestionText = model.QuestionText?.Trim() ?? "";

                    // Ensure RightAns is one of the valid answers or empty if no valid answers
                    if (validAnswers.Any())
                    {
                        string rightAns = model.RightAns?.Trim() ?? "";
                        if (string.IsNullOrWhiteSpace(rightAns) || !validAnswers.Contains(rightAns))
                        {
                            question.RightAns = validAnswers.First();
                            System.Diagnostics.Debug.WriteLine($"Setting RightAns to first valid answer: {question.RightAns}");
                        }
                        else
                        {
                            question.RightAns = rightAns;
                        }
                    }
                    else
                    {
                        // We must use an empty string instead of null because of the database constraint
                        question.RightAns = "";
                        System.Diagnostics.Debug.WriteLine("No valid answers, setting RightAns to empty string");
                    }

                    System.Diagnostics.Debug.WriteLine($"Updating question: {question.QuestionText}, RightAns: {question.RightAns}");
                    dbContext.Questions.Update(question);
                    await dbContext.SaveChangesAsync();

                    // Delete all existing answers
                    var existingAnswers = await dbContext.Answers
                        .Where(a => a.QuestionID == id)
                        .ToListAsync();

                    foreach (var answer in existingAnswers)
                    {
                        System.Diagnostics.Debug.WriteLine($"Deleting answer: {answer.Answers}");
                        dbContext.Answers.Remove(answer);
                    }
                    await dbContext.SaveChangesAsync();

                    // Create new answers
                    if (validAnswers.Any())
                    {
                        foreach (var answerText in validAnswers)
                        {
                            var answer = new Answer
                            {
                                QuestionID = question.QuestionID,
                                Answers = answerText
                            };
                            
                            System.Diagnostics.Debug.WriteLine($"Creating answer: {answerText}");
                            dbContext.Answers.Add(answer);
                        }
                    }

                    // Save all changes
                    await dbContext.SaveChangesAsync();
                    System.Diagnostics.Debug.WriteLine("Question and answers updated successfully");
                    
                    // Commit the transaction
                    await dbContext.Database.CommitTransactionAsync();
                    
                    return true;
                }
                catch (Exception ex)
                {
                    // Rollback transaction in case of an error
                    await dbContext.Database.RollbackTransactionAsync();
                    
                    System.Diagnostics.Debug.WriteLine($"Transaction rolled back due to error: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }
                    
                    throw; // Re-throw to be caught by the outer catch
                }
            }
            catch (Exception ex)
            {
                // Log the exception details
                System.Diagnostics.Debug.WriteLine($"Error editing question: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                return false;
            }
        }

        public async Task<bool> DeleteQuestion(int id)
        {
            try
            {
                var dbContext = _unitOfWork.GetDbContext();
                
                // Start a transaction
                await dbContext.Database.BeginTransactionAsync();
                
                try
                {
                    // Find the question
                    var question = await dbContext.Questions.FindAsync(id);
                    if (question == null)
                        return false;

                    // Find and delete all associated answers
                    var answers = await dbContext.Answers
                        .Where(a => a.QuestionID == id)
                        .ToListAsync();

                    foreach (var answer in answers)
                    {
                        dbContext.Answers.Remove(answer);
                    }
                    await dbContext.SaveChangesAsync();

                    // Remove the question
                    dbContext.Questions.Remove(question);
                    await dbContext.SaveChangesAsync();
                    
                    // Commit the transaction
                    await dbContext.Database.CommitTransactionAsync();
                    
                    return true;
                }
                catch (Exception ex)
                {
                    // Rollback transaction in case of an error
                    await dbContext.Database.RollbackTransactionAsync();
                    System.Diagnostics.Debug.WriteLine($"Error in DeleteQuestion: {ex.Message}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Outer error in DeleteQuestion: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<QuestionDto>> GetQuestionsByQuizAsync(int quizId)
        {
            var questions = await _unitOfWork.GetRepository<Question>()
                .FindByCondition(q => q.QuizID == quizId)
                .Include(q => q.Answers)
                .ToListAsync();

            return _mapper.Map<IEnumerable<QuestionDto>>(questions);
        }

        // Get unit of work for direct database access in debugging
        public IUnitOfWork GetUnitOfWork()
        {
            return _unitOfWork;
        }
    }
} 