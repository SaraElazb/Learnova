using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models
{
    public class EditProfileViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "First name must be between {2} and {1} characters long.", MinimumLength = 2)]
        public string FirstName { get; set; }
        
        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "Last name must be between {2} and {1} characters long.", MinimumLength = 2)]
        public string LastName { get; set; }
        
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
} 