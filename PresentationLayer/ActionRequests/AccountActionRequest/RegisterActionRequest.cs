using BusinessLogicLayer.DTOs.AccountDto;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.ActionRequests.AccountActionRequest
{
    public class RegisterActionRequest
    {
        [Required(ErrorMessage = "Username is required.")]
        [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmedPassword { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }

        [Url(ErrorMessage = "Invalid image URL.")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; }

        // Student-specific properties
        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be 14 digits.")]
        public string? NationalId { get; set; }

        [MaxLength(20, ErrorMessage = "Grade must be less than 20 characters.")]
        public string? Grade { get; set; }

        // Teacher-specific properties
        [MaxLength(100, ErrorMessage = "Specialization cannot be longer than 100 characters.")]
        public string? Specialization { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Years of experience cannot be negative.")]
        public int? YearsOfExperience { get; set; }

        public StudentRegisterDto ToStudentDto()
        {
            return new StudentRegisterDto
            {
                Username = Username,
                FirstName = FirstName,
                LastName = LastName,
                Password = Password,
                Email = Email,
                ImageUrl = ImageUrl,
                NationalId = NationalId,
                Grade = Grade
            };
        }

        public TeacherRegisterDto ToTeacherDto()
        {
            return new TeacherRegisterDto
            {
                Username = Username,
                FirstName = FirstName,
                LastName = LastName,
                Password = Password,
                Email = Email,
                ImageUrl = ImageUrl,
                Specialization = Specialization ?? string.Empty,
                YearsOfExperience = YearsOfExperience ?? 0
            };
        }
    }
} 