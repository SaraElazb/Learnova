using BusinessLogicLayer.DTOs.QuizDtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Manager.QuestionManager
{
    public interface IQuestionManager
    {
        Task<IEnumerable<QuestionDto>> FindAllAsync();
        Task<QuestionDto> FindAsync(int id);
        Task<QuestionDto> GetByIdAsync(int id);
        Task<bool> CreateQuestionAsync(QuestionRequest model);
        Task<bool> EditQuestionAsync(int id, QuestionRequest model);
        Task<bool> DeleteQuestion(int id);
        Task<IEnumerable<QuestionDto>> GetQuestionsByQuizAsync(int quizId);
    }
} 