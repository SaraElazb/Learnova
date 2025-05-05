using BusinessLogicLayer.DTOs.AccountDto;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.AccountServices
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        public async Task<(bool Succeeded, string Message, string Role)> LoginAsync(LogInDto logInDto)
        {
            var result = await _signInManager.PasswordSignInAsync(
                logInDto.UserName,      
                logInDto.Password,      
                logInDto.RememberMe,    
                lockoutOnFailure: true  
            );

           
            if (result.Succeeded)
            {
                // Get the user to determine their role
                var user = await _userManager.FindByNameAsync(logInDto.UserName);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var role = roles.FirstOrDefault() ?? "User";
                    return (true, "Login successful", role);
                }
                return (true, "Login successful", "User");
            }

            if (result.IsLockedOut)
            {
                return (false, "Your account is locked. Please try again later.", string.Empty);
            }

           
            return (false, "Invalid username or password", string.Empty);
        }


        public async Task LogOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> RegisterStudentAsync(StudentRegisterDto request)
        {
            Student student = new Student()
            {
                 Id = Guid.NewGuid().ToString(),
                 Email = request.Email,
                 First_name = request.FirstName,
                 Last_name = request.LastName,
                 Grade = request.Grade,
                 NationalId = request.NationalId,
                 Profile_picture = request.ImageUrl,
                 UserName = request.Username, 
            };
            var result = await _userManager.CreateAsync(student, request.Password);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByIdAsync(student.Id);
                await _userManager.AddToRoleAsync(user, "Student");
                await _signInManager.SignInAsync(student, false);
            }

            return result;
        }

        public async Task<IdentityResult> RegisterTeacherAsync(TeacherRegisterDto request)
        {
            Teacher teacher = new Teacher()
            {
                Id = Guid.NewGuid().ToString(),
                Email = request.Email,
                First_name = request.FirstName,
                Last_name = request.LastName,
                 Specialization = request.Specialization,
                 YearsOfExperience = request.YearsOfExperience,
                Profile_picture = request.ImageUrl,
                UserName = request.Username,
            };
            var result = await _userManager.CreateAsync(teacher, request.Password);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByIdAsync(teacher.Id);
                await _userManager.AddToRoleAsync(user, "Teacher");
                await _signInManager.SignInAsync(teacher, false);
            }

            return result;
        }
    }
}
