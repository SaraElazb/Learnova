using BusinessLogicLayer.DTOs.LessonDtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Manager.LessonManager
{
    public interface ILessonManager
    {
        Task<IEnumerable<LessonDto>> FindAllAsync();
        Task<LessonDto> FindAsync(int id);
        Task<LessonDto> GetByIdAsync(int id);
        Task<bool> CreateLessonAsync(LessonRequest model);
        Task<bool> EditLessonAsync(int id, LessonRequest model);
        Task<bool> SoftDelete(LessonDto lesson);
        Task<IEnumerable<LessonDto>> GetLessonsByCourseAsync(int courseId);
    }
} 