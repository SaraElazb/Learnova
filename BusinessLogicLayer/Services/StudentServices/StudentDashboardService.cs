using BusinessLogicLayer.DTOs.CourseDtos;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.StudentServices
{
    public class StudentDashboardService : IStudentDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentDashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<CourseDTO>> GetEnrolledCoursesAsync(string userId)
        {
            try
            {
                var enrollments = await _unitOfWork.Enrollments
                    .FindAllAsync(
                        e => e.User_ID == userId,
                        include: q => q.Include(e => e.Course)
                            .ThenInclude(c => c.Category)
                            .Include(e => e.Course)
                            .ThenInclude(c => c.Lessons)
                    );

                var enrolledCourses = new List<CourseDTO>();
                
                foreach(var enrollment in enrollments)
                {
                    var course = enrollment.Course;
                    if (course == null) continue;
                    
                    // Get all lessons for this course
                    var lessonIds = course.Lessons?.Select(l => l.Lesson_ID).ToList() ?? new List<int>();
                    int totalLessons = lessonIds.Count;
                    
                    // No lessons means no progress to track
                    if (totalLessons == 0)
                    {
                        enrolledCourses.Add(new CourseDTO
                        {
                            Course_ID = course.Course_ID,
                            Title = course.Title,
                            Description = course.Description,
                            Category = course.Category?.Category_Name,
                            CategoryId = course.Category_ID,
                            EnrollmentDate = enrollment.Enrollment_date,
                            Price = course.Price,
                            ThumbnailUrl = course.ImagePath,
                            LessonCount = 0,
                            Progress = 0
                        });
                        continue;
                    }
                    
                    // Get completed lessons for this user in this course
                    var completedLessons = await _unitOfWork.Studies.FindAllAsync(
                        s => s.User_ID == userId && 
                             lessonIds.Contains(s.Lesson_ID) && 
                             s.Status == "Completed"
                    );
                    
                    int completedCount = completedLessons.Count();
                    int progressPercentage = (int)Math.Round((double)completedCount / totalLessons * 100);
                    
                    enrolledCourses.Add(new CourseDTO
                    {
                        Course_ID = course.Course_ID,
                        Title = course.Title,
                        Description = course.Description,
                        Category = course.Category?.Category_Name,
                        CategoryId = course.Category_ID,
                        EnrollmentDate = enrollment.Enrollment_date,
                        Price = course.Price,
                        ThumbnailUrl = course.ImagePath,
                        LessonCount = totalLessons,
                        Progress = progressPercentage
                    });
                }

                return enrolledCourses;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetEnrolledCoursesAsync: {ex.Message}");
                return new List<CourseDTO>();
            }
        }

        public async Task<IEnumerable<AssignmentDto>> GetUpcomingAssignmentsAsync(string userId)
        {
            try
            {
                // Get all enrollments for the user
                var enrollments = await _unitOfWork.Enrollments
                    .FindAllAsync(
                        e => e.User_ID == userId,
                        include: q => q.Include(e => e.Course)
                    );
                
                var courseIds = enrollments.Select(e => e.Course_ID).ToList();
                
                // Get lessons with quizzes from enrolled courses
                var lessonsWithQuizzes = await _unitOfWork.Lessons
                    .FindAllAsync(
                        l => courseIds.Contains(l.Course_ID) && l.Quiz_ID != null,
                        include: q => q.Include(l => l.Course)
                                      .Include(l => l.Quiz)
                    );
                
                // Get completed submissions
                var completedSubmissions = await _unitOfWork.Submissions
                    .FindAllAsync(
                        s => s.User_ID == userId && 
                             (s.Status == "Completed" || s.Status == "Passed" || s.Status == "Failed")
                    );
                
                var completedQuizIds = completedSubmissions.Select(s => s.Quiz_ID).ToList();
                
                // Filter out lessons whose quizzes have been completed
                var pendingQuizLessons = lessonsWithQuizzes
                    .Where(l => l.Quiz_ID.HasValue && !completedQuizIds.Contains(l.Quiz_ID.Value))
                    .OrderBy(l => l.Course.Title)
                    .ThenBy(l => l.LessonOrder)
                    .Take(3); // Limit to 3 upcoming assignments
                
                // Convert to DTOs
                var assignments = pendingQuizLessons.Select(l => new AssignmentDto
                {
                    Title = $"Quiz: {l.Title}",
                    CourseName = l.Course.Title,
                    DueDate = DateTime.Now.AddDays(7), // Default due date
                    Status = "Pending"
                }).ToList();
                
                return assignments;
            }
            catch (Exception)
            {
                // If there's an error, return empty list
                return new List<AssignmentDto>();
            }
        }

        public async Task<IEnumerable<ActivityDto>> GetRecentActivitiesAsync(string userId)
        {
            try
            {
                var activities = new List<ActivityDto>();
                
                // Get recent completed lessons
                var recentStudies = await _unitOfWork.Studies
                    .FindAllAsync(
                        s => s.User_ID == userId && s.Status == "Completed",
                        include: q => q.Include(s => s.Lesson)
                                     .ThenInclude(l => l.Course)
                    );
                
                foreach (var study in recentStudies.OrderByDescending(s => s.CompletionDate).Take(3))
                {
                    activities.Add(new ActivityDto
                    {
                        Type = "lesson_completion",
                        Description = $"Completed lesson: {study.Lesson.Title} in {study.Lesson.Course.Title}",
                        Date = study.CompletionDate ?? DateTime.Now,
                        DisplayDate = GetDisplayDate(study.CompletionDate ?? DateTime.Now)
                    });
                }
                
                // Get recent quiz submissions
                var recentSubmissions = await _unitOfWork.Submissions
                    .FindAllAsync(
                        s => s.User_ID == userId,
                        include: q => q.Include(s => s.Quiz)
                    );
                
                foreach (var submission in recentSubmissions.OrderByDescending(s => s.SubmissionDate).Take(3))
                {
                    activities.Add(new ActivityDto
                    {
                        Type = "quiz_submission",
                        Description = $"Submitted quiz: {submission.Quiz.Title} with score {submission.Score}%",
                        Date = submission.SubmissionDate,
                        DisplayDate = GetDisplayDate(submission.SubmissionDate)
                    });
                }
                
                return activities.OrderByDescending(a => a.Date).Take(5);
            }
            catch (Exception)
            {
                return new List<ActivityDto>();
            }
        }

        public async Task<int> GetOverallProgressAsync(string userId)
        {
            try
            {
                // Get all enrolled courses
                var enrollments = await _unitOfWork.Enrollments.FindAllAsync(e => e.User_ID == userId);
                var courseIds = enrollments.Select(e => e.Course_ID).ToList();
                
                if (!courseIds.Any())
                    return 0;
                
                // Get all lessons from enrolled courses
                var lessons = await _unitOfWork.Lessons.FindAllAsync(l => courseIds.Contains(l.Course_ID));
                var lessonIds = lessons.Select(l => l.Lesson_ID).ToList();
                
                int totalLessons = lessonIds.Count;
                
                if (totalLessons == 0)
                    return 0;
                
                // Get completed lessons
                var completedLessons = await _unitOfWork.Studies.FindAllAsync(
                    s => s.User_ID == userId && 
                         lessonIds.Contains(s.Lesson_ID) && 
                         s.Status == "Completed"
                );
                
                int completedCount = completedLessons.Count();
                
                return (int)Math.Round((double)completedCount / totalLessons * 100);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<Dictionary<string, int>> GetCourseProgressAsync(string userId)
        {
            try
            {
                var result = new Dictionary<string, int>();
                
                // Get all enrolled courses with their lessons
                var enrollments = await _unitOfWork.Enrollments
                    .FindAllAsync(
                        e => e.User_ID == userId,
                        include: q => q.Include(e => e.Course)
                                     .ThenInclude(c => c.Lessons)
                    );
                
                foreach (var enrollment in enrollments)
                {
                    var course = enrollment.Course;
                    
                    if (course == null || course.Lessons == null || !course.Lessons.Any())
                    {
                        result[course?.Title ?? "Unknown Course"] = 0;
                        continue;
                    }
                    
                    // Get lesson IDs for this course
                    var lessonIds = course.Lessons.Select(l => l.Lesson_ID).ToList();
                    
                    // Get completed lessons for this course
                    var completedLessons = await _unitOfWork.Studies.FindAllAsync(
                        s => s.User_ID == userId && 
                             lessonIds.Contains(s.Lesson_ID) && 
                             s.Status == "Completed"
                    );
                    
                    int totalLessons = lessonIds.Count;
                    int completedCount = completedLessons.Count();
                    
                    int progressPercentage = totalLessons > 0 
                        ? (int)Math.Round((double)completedCount / totalLessons * 100) 
                        : 0;
                    
                    result[course.Title] = progressPercentage;
                }
                
                return result;
            }
            catch (Exception)
            {
                return new Dictionary<string, int>();
            }
        }
        
        private string GetDisplayDate(DateTime date)
        {
            var diff = DateTime.Now - date;
            
            if (diff.TotalDays < 1)
                return "Today";
            else if (diff.TotalDays < 2)
                return "Yesterday";
            else if (diff.TotalDays < 7)
                return $"{(int)diff.TotalDays} days ago";
            else if (diff.TotalDays < 30)
                return $"{(int)(diff.TotalDays / 7)} weeks ago";
            else
                return date.ToString("MMMM dd, yyyy");
        }
    }
} 