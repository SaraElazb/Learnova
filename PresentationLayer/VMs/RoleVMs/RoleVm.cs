using BusinessLogicLayer.DTOs.RoleDto;

namespace PresentationLayer.VMs.RoleVMs
{
    public class RoleVm
    {
        public string RoleName { get; set; }
    }
    public static class RoleExtention
    {
        public static RoleVm ToVm(this RoleDto dto)
        {
            return new()
            {
                RoleName = dto.RoleName
            };
        }
    }
}
