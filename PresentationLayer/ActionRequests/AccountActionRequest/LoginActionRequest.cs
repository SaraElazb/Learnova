using BusinessLogicLayer.DTOs.AccountDto;
using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.ActionRequests.AccountActionRequest
{
    public class LoginActionRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
    public static class LogInExtensions
    {
        public static LogInDto ToLogInDto(this LoginActionRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Request cannot be null");
            }

            return new LogInDto
            {
                UserName = request.Username,
                Password = request.Password,
                RememberMe = request.RememberMe
            };
        }
    }

}
