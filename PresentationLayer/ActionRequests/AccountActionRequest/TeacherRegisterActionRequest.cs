namespace PresentationLayer.ActionRequests.TeacherActionRequest
{
    using BusinessLogicLayer.DTOs.AccountDto;
    using System.ComponentModel.DataAnnotations;

    public class TeacherRegisterActionRequest
    {
        [Required(ErrorMessage = "Username is required.")]
        [MaxLength(50, ErrorMessage = "Username cannot be longer than 50 characters.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(50, ErrorMessage = "Last name cannot be longer than 50 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        public string Password { get; set; }

        [Required]
        [Compare(nameof(Password))]
        public string ConfirmedPassword { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Url(ErrorMessage = "Invalid URL format.")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Specialization is required.")]
        [MaxLength(100, ErrorMessage = "Specialization cannot be longer than 100 characters.")]
        public string Specialization { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Years of experience cannot be negative.")]
        public int YearsOfExperience { get; set; }
    }


    public static class TeacherRegisterExtensions
    {
        public static TeacherRegisterDto ToDto(this TeacherRegisterActionRequest request)
        {
            return new TeacherRegisterDto
            {
                Username = request.Username,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Password = request.Password,
                Email = request.Email,
                ImageUrl = request.ImageUrl,
                Specialization = request.Specialization,
                YearsOfExperience = request.YearsOfExperience
            };
        }
    }


}
