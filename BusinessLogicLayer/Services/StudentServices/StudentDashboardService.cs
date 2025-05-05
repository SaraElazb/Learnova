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

                var enrolledCourses = enrollments.Select(e => new CourseDTO
                {
                    Course_ID = e.Course.Course_ID,
                    Title = e.Course.Title,
                    Description = e.Course.Description,
                    Category = e.Course.Category?.Category_Name,
                    CategoryId = e.Course.Category_ID,
                    EnrollmentDate = e.Enrollment_date,
                    Price = e.Course.Price,
                    ThumbnailUrl = e.Course.ImagePath,
                    LessonCount = e.Course.Lessons?.Count ?? 0,
                    // Calculate progress (for demo purposes, we'll use a random number)
                    // In a real application, you would calculate based on completed lessons
                    Progress = new Random().Next(0, 101)
                }).ToList();

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
            // In a real application, you would fetch assignments from a database
            // For this demo, we'll return mock data
            await Task.Delay(1); // Simulate async operation

            return new List<AssignmentDto>
            {
                new AssignmentDto
                {
                    Title = "C# Exercise: Object-Oriented Programming",
                    CourseName = "Introduction to C#",
                    DueDate = DateTime.Now.AddDays(1),
                    Status = "Due Tomorrow"
                },
                new AssignmentDto
                {
                    Title = "Web Project: Responsive Design",
                    CourseName = "Advanced Web Development",
                    DueDate = DateTime.Now.AddDays(3),
                    Status = "Due in 3 days"
                },
                new AssignmentDto
                {
                    Title = "Quiz: JavaScript Fundamentals",
                    CourseName = "Advanced Web Development",
                    DueDate = DateTime.Now.AddDays(7),
                    Status = "Due in 1 week"
                }
            };
        }

        public async Task<IEnumerable<ActivityDto>> GetRecentActivitiesAsync(string userId)
        {
            // In a real application, you would fetch activities from a database
            // For this demo, we'll return mock data
            await Task.Delay(1); // Simulate async operation

            return new List<ActivityDto>
            {
                new ActivityDto
                {
                    Type = "completion",
                    Description = "Completed lesson: Variables & Data Types",
                    Date = DateTime.Now.AddDays(-1),
                    DisplayDate = "Yesterday"
                },
                new ActivityDto
                {
                    Type = "submission",
                    Description = "Submitted assignment: HTML/CSS Layout",
                    Date = DateTime.Now.AddDays(-2),
                    DisplayDate = "2 days ago"
                },
                new ActivityDto
                {
                    Type = "enrollment",
                    Description = "Enrolled in course: Database Design",
                    Date = DateTime.Now.AddDays(-3),
                    DisplayDate = "3 days ago"
                }
            };
        }

        public async Task<int> GetOverallProgressAsync(string userId)
        {
            // Calculate the overall progress across all courses
            var enrolledCourses = await GetEnrolledCoursesAsync(userId);
            if (!enrolledCourses.Any())
                return 0;

            // Calculate average progress
            return (int)enrolledCourses.Average(c => c.Progress);
        }

        public async Task<Dictionary<string, int>> GetCourseProgressAsync(string userId)
        {
            var enrolledCourses = await GetEnrolledCoursesAsync(userId);
            return enrolledCourses.ToDictionary(c => c.Title, c => c.Progress);
        }
    }
} 