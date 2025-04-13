using BusinessLogicLayer.DTOs.QuizDtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Manager.QuizManager
{
    public interface IQuizManager
    {
        Task<IEnumerable<QuizDto>> FindAllAsync();
        Task<QuizDto> FindAsync(int id);
        Task<QuizDto> GetByIdAsync(int id);
        Task<bool> CreateQuizAsync(QuizRequest model);
        Task<bool> EditQuizAsync(int id, QuizRequest model);
        Task<bool> SoftDelete(QuizDto quiz);
        Task<IEnumerable<QuizDto>> GetQuizzesByLessonAsync(int lessonId);
    }
} 