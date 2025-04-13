namespace PresentationLayer.ActionRequests.TeacherActionRequest
{
    using BusinessLogicLayer.DTOs.AccountDto;
    using System.ComponentModel.DataAnnotations;

    public class StudentRegisterActionRequest
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
        [Required]
        [Compare(nameof(Password))]
        public string ConfirmedPassword { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }

        [Url(ErrorMessage = "Invalid image URL.")]
        public string? ImageUrl { get; set; }

        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID must be 14 digits.")]
        public string? NationalId { get; set; }

        [MaxLength(20, ErrorMessage = "Grade must be less than 20 characters.")]
        public string? Grade { get; set; }
    }
    public static class StudentRegisterExtensions
    {
        public static StudentRegisterDto ToDto(this StudentRegisterActionRequest request)
        {
            return new StudentRegisterDto
            {
                Username = request.Username,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Password = request.Password,
                Email = request.Email,
                ImageUrl = request.ImageUrl,
                NationalId = request.NationalId,
                Grade = request.Grade
            };
        }
    }
}
