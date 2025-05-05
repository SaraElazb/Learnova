using BusinessLogicLayer.DTOs.AccountDto;
using BusinessLogicLayer.Services.AccountServices;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ActionRequests.AccountActionRequest;
using PresentationLayer.ActionRequests.TeacherActionRequest;

namespace PresentationLayer.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
           _accountService = accountService;
        }

        // New combined registration GET action
        public IActionResult Register()
        {
            return View();
        }

        // New combined registration POST action
        [HttpPost]
        public async Task<IActionResult> Register(RegisterActionRequest request)
        {
            if (ModelState.IsValid)
            {
                if (request.Role == "Student")
                {
                    var studentDto = request.ToStudentDto();
                    var result = await _accountService.RegisterStudentAsync(studentDto);

                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = "Registration successful! Welcome to LearnNova.";
                        return RedirectToAction("Dashboard", "Student");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else if (request.Role == "Instructor")
                {
                    var teacherDto = request.ToTeacherDto();
                    var result = await _accountService.RegisterTeacherAsync(teacherDto);

                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = "Registration successful! Welcome to LearnNova.";
                        return RedirectToAction("Dashboard", "Instructor");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid role selected.");
                }
            }

            return View(request);
        }

        // Original Teacher Registration methods
        public IActionResult RegisterTeacher()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterTeacher(TeacherRegisterActionRequest request)
        {
            if (ModelState.IsValid)
            {
                var dto = request.ToDto();

                var result = await _accountService.RegisterTeacherAsync(dto);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Registration successful! Welcome to LearnNova.";
                    return RedirectToAction("Dashboard", "Instructor");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(request);
        }
        public  IActionResult RegisterStudent()
        {
            return View();
        }
        public IActionResult LogIn()
        {
           
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LogIn(LoginActionRequest request)
        {
            if (ModelState.IsValid)
            {
                var dto = request.ToLogInDto();
                
                var (succeeded, message, role) = await _accountService.LoginAsync(dto);

                if (succeeded)
                {
                    // Redirect based on user role
                    if (role == "Admin")
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else if (role == "Teacher")
                    {
                        return RedirectToAction("Dashboard", "Instructor");
                    }
                    else if (role == "Student")
                    {
                        return RedirectToAction("Dashboard", "Student");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

               
                ModelState.AddModelError("", message);
            }

           
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterStudent(StudentRegisterActionRequest request)
        {
            if (ModelState.IsValid)
            {
                var dto = request.ToDto();

                var result = await _accountService.RegisterStudentAsync(dto);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Registration successful! Welcome to LearnNova.";
                    return RedirectToAction("Dashboard", "Student");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await _accountService.LogOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}
