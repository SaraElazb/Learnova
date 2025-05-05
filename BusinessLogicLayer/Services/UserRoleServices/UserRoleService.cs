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
        private readonly ELearningDbContext dbContext;

        public UserRoleService(UserManager<User> userManager, RoleManager<Role> roleManager, ELearningDbContext dbContext)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.dbContext = dbContext;
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
                    Email = user.Email,
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
        
        public async Task<(bool CanDelete, List<string> AssociatedData)> CheckUserAssociationsAsync(string userId)
        {
            var associatedData = new List<string>();
            
            // Check for courses created by the user (as instructor)
            var coursesCreated = await dbContext.Courses
                .Where(c => c.InstructorId == userId)
                .Select(c => c.Title)
                .ToListAsync();
                
            if (coursesCreated.Any())
            {
                associatedData.Add($"Instructor for {coursesCreated.Count} course(s): {string.Join(", ", coursesCreated.Take(3))}");
                if (coursesCreated.Count > 3)
                    associatedData[associatedData.Count - 1] += $" and {coursesCreated.Count - 3} more";
            }
            
            // Check for course enrollments
            var enrollments = await dbContext.Enrollments
                .Include(e => e.Course)
                .Where(e => e.User_ID == userId)
                .Select(e => e.Course.Title)
                .ToListAsync();
                
            if (enrollments.Any())
            {
                associatedData.Add($"Enrolled in {enrollments.Count} course(s): {string.Join(", ", enrollments.Take(3))}");
                if (enrollments.Count > 3)
                    associatedData[associatedData.Count - 1] += $" and {enrollments.Count - 3} more";
            }
            
            // Check for course reviews
            var reviews = await dbContext.Reviews
                .Where(r => r.User_ID == userId)
                .CountAsync();
                
            if (reviews > 0)
            {
                associatedData.Add($"Authored {reviews} course review(s)");
            }
            
            // Check for quiz submissions
            var submissions = await dbContext.Submissions
                .Where(s => s.User_ID == userId)
                .CountAsync();
                
            if (submissions > 0)
            {
                associatedData.Add($"Has {submissions} quiz submission(s)");
            }
            
            return (associatedData.Count == 0, associatedData);
        }
    }
}
