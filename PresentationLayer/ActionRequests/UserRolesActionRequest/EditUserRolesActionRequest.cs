using BusinessLogicLayer.DTOs.UserRolesDtos;

namespace PresentationLayer.ActionRequests.UserRolesActionRequest
{
    public class EditUserRolesActionRequest
    {
        public string UserId { get; set; }
        public List<string> SelectedRoles { get; set; } = new List<string>();
    }
    public static class EditUserRolesExtentions
    {
        public static EditUserRolesDto ToDto(this EditUserRolesActionRequest request)
        {
            return new EditUserRolesDto()
            {
                SelectedRoles = request.SelectedRoles,
                UserId = request.UserId
            };
        }
    }
}
