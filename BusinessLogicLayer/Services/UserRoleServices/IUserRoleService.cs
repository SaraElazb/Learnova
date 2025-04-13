using BusinessLogicLayer.DTOs.UserRolesDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.UserRoleServices
{
    public interface IUserRoleService
    {
        Task<(List<UserRolesDto> Users, int TotalUsers)> GetUsersWithRolesAsync(string searchTerm, int page, int pageSize);
        Task<bool> UpdateUserRolesAsync(EditUserRolesDto request);
    }
}
