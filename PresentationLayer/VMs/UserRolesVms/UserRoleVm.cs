using BusinessLogicLayer.DTOs.UserRolesDtos;

namespace PresentationLayer.VMs.UserRolesVms
{
    public class UserRoleVm
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        public List<string> Roles { get; set; }
        public string Email { get; set; }
    }
    public static class UserRoleExtention
    {
        public static UserRoleVm ToVm(this UserRolesDto dto)
        {
            return new UserRoleVm()
            {
                UserId = dto.UserId,
                Roles = dto.Roles,
                UserName = dto.UserName,
                Email = dto.Email
            };
        }
    }
}
