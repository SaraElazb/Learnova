using BusinessLogicLayer.DTOs.RoleDto;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.RoleServices
{
    public interface IRoleService
    {
        Task<List<RoleDto>> GetAllRolesAsync();
        Task<(bool Succeeded, IEnumerable<IdentityError> Errors)> CreateRoleAsync(string roleName);
    }

}
