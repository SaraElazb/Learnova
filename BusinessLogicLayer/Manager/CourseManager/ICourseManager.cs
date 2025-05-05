using BusinessLogicLayer.DTOs.CategoryDtos;
using BusinessLogicLayer.DTOs.CourseDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Manager.CourseManager
{
    public interface ICourseManager
    {
        Task<IEnumerable<CourseDTO>> FindAllAsync();
        Task<CourseDTO> FindAsync(int id);
        Task<bool> EditCourseAsync(int id, CourseRequest model);
        Task<Course> GetByIdAsync(int id);
        Task CreateCourseAsync(CourseRequest model);
        Task SoftDelete(Course course);
        Task<IEnumerable<CourseDTO>> GetCoursesAsync(int? categoryId);
        Task<IEnumerable<CourseDTO>> GetInstructorCoursesAsync(string instructorId);
    }
}
