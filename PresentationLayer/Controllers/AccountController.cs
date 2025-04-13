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
                    return RedirectToAction("Index", "Home");
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
                
                var (succeeded, message) = await _accountService.LoginAsync(dto);

                if (succeeded)
                {
                    return RedirectToAction("Index", "Home"); 
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
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(request);
        }



    }
}
