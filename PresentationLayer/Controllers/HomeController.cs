using BusinessLogicLayer; // تأكدي إن namespace الصح مضاف
using DataAccessLayer; // لو محتاجة التعامل مع الكيانات
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var courses = _unitOfWork.CourseRepository.GetAllAsync(); // جلب كل الكورسات
            return View(courses);
        }
    }
}
