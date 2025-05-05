using BusinessLogicLayer.DTOs.CourseDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.StudentServices
{
    public class AssignmentDto
    {
        public string Title { get; set; }
        public string CourseName { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
    }

    public class ActivityDto
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string DisplayDate { get; set; }
    }

    public interface IStudentDashboardService
    {
        Task<IEnumerable<CourseDTO>> GetEnrolledCoursesAsync(string userId);
        Task<IEnumerable<AssignmentDto>> GetUpcomingAssignmentsAsync(string userId);
        Task<IEnumerable<ActivityDto>> GetRecentActivitiesAsync(string userId);
        Task<int> GetOverallProgressAsync(string userId);
        Task<Dictionary<string, int>> GetCourseProgressAsync(string userId);
    }
} 