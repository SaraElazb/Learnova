using BusinessLogicLayer.DTOs.UserRolesDtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.UserRoleServices
{
    public class UserRoleService : IUserRoleService
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;

        public UserRoleService(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<(List<UserRolesDto> Users, int TotalUsers)> GetUsersWithRolesAsync(string searchTerm, int page, int pageSize)
        {
            var query = userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.UserName.ToLower().Contains(searchTerm.ToLower()) || u.Email.ToLower().Contains(searchTerm.ToLower()));
            }

            int totalUsers = await query.CountAsync();
            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userRoles = new List<UserRolesDto>();
            foreach (var user in users)
            {
                var userRole = new UserRolesDto
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Roles = (List<string>)await userManager.GetRolesAsync(user)
                };
                userRoles.Add(userRole);
            }

            return (userRoles, totalUsers);
        }

        public async Task<bool> UpdateUserRolesAsync(EditUserRolesDto request)
        {
            var user = await userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return false; 
            }

            var currentRoles = await userManager.GetRolesAsync(user);
            var newRoles = request.SelectedRoles ?? new List<string>();

            var addRoles = newRoles.Except(currentRoles).ToList();
            var deleteRoles = currentRoles.Except(newRoles).ToList();

            if (addRoles.Any())
                await userManager.AddToRolesAsync(user, addRoles);

            if (deleteRoles.Any())
                await userManager.RemoveFromRolesAsync(user, deleteRoles);

            return true;
        }
    }

}
