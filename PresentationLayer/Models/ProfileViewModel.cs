using System;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models
{
    public class ProfileViewModel
    {
        public string Id { get; set; }
        
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        
        public string UserName { get; set; }
        
        public string Email { get; set; }
        
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        
        [Display(Name = "Member Since")]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        public DateTime DateJoined { get; set; }
        
        // Instructor-specific properties
        public bool IsInstructor { get; set; }
        
        [Display(Name = "Total Courses")]
        public int TotalCourses { get; set; }
        
        [Display(Name = "Total Students")]
        public int TotalStudents { get; set; }
        
        // Student-specific properties
        public bool IsStudent { get; set; }
        
        [Display(Name = "Enrolled Courses")]
        public int EnrolledCourses { get; set; }
        
        [Display(Name = "Completed Lessons")]
        public int CompletedLessons { get; set; }
        
        [Display(Name = "Quizzes Completed")]
        public int QuizzesCompleted { get; set; }
        
        [Display(Name = "Quizzes Passed")]
        public int QuizzesPassed { get; set; }
        
        // Helper properties for the view
        public string FullName => $"{FirstName} {LastName}";
        
        public string QuizSuccessRate => QuizzesCompleted > 0 
            ? $"{(float)QuizzesPassed / QuizzesCompleted * 100:0}%" 
            : "N/A";
    }
} 