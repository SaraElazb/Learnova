using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.ActionRequests.RoleActionRequest
{
    public class CreateRoleActionRequest
    {
        [Required]
        public string RoleName { get; set; }
    }
}
