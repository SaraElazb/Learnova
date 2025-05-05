using BusinessLogicLayer.Services.AccountServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly IAccountService _accountService;

        public StudentController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public IActionResult Dashboard()
        {
            return View();
        }
    }
} 