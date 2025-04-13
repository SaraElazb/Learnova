using BusinessLogicLayer.DTOs.AccountDto;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.AccountServices
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterStudentAsync(StudentRegisterDto request);
        Task<IdentityResult> RegisterTeacherAsync(TeacherRegisterDto request);
        Task<(bool Succeeded, string Message)> LoginAsync(LogInDto logInDto);
        Task LogOutAsync();
    }
}
