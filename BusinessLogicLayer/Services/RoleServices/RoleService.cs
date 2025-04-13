using BusinessLogicLayer.DTOs.RoleDto;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.RoleServices
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<Role> _roleManager;

        public RoleService(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

       
        public async Task<(bool Succeeded, IEnumerable<IdentityError> Errors)> CreateRoleAsync(string roleName)
        {
            bool exists = await _roleManager.RoleExistsAsync(roleName);
            if (exists)
            {
                return (false, new List<IdentityError> {
                new IdentityError { Description = "Role already exists." }
            });
            }

            var result = await _roleManager.CreateAsync(new Role { Name = roleName });
            return (result.Succeeded, result.Errors);
        }

        public async Task<List<RoleDto>> GetAllRolesAsync()
        {
             var roles = await _roleManager.Roles.ToListAsync();
            return roles.Select(r => new RoleDto() { RoleName = r.Name }).ToList();
        }
    }

}
