using BusinessLogicLayer.DTOs.UserRolesDtos;

namespace PresentationLayer.ActionRequests.UserRolesActionRequest
{
    public class UserRolesActionRequest
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        public List<string> Roles { get; set; }
    }
    public static class UserRolesExtentions
    {
        public static UserRolesDto ToDto(this UserRolesActionRequest request)
        {
            return new UserRolesDto()
            {
                 
                Roles = request.Roles,
                UserId = request.UserId,
                 UserName = request.UserName
            };
        }
    }
}

