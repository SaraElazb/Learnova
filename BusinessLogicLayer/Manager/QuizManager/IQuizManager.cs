using BusinessLogicLayer.DTOs.QuizDtos;

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
        Task<IEnumerable<QuizDto>> GetQuizzesByInstructorAsync(string instructorId);
    }
} 