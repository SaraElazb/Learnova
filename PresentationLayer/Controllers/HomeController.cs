using BusinessLogicLayer; 
using DataAccessLayer;
using DataAccessLayer.Repositories;
using Grad_Project.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AutoMapper;
using BusinessLogicLayer.DTOs.CourseDtos;
using BusinessLogicLayer.Helpers;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace PresentationLayer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [Route("/")]
        public async Task<IActionResult> Index(int? categoryId)
        {
            ViewBag.SelectedCategoryId = categoryId;
            var categories = await _unitOfWork.Categories.FindAllAsync(c => c.IsActive);
            var categorySelectList = categories.Select(c => new SelectListItem
            {
                Value = c.Category_ID.ToString(),
                Text = c.Category_Name
            }).ToList();
            ViewBag.Categories = categorySelectList;
            var coursesQuery = await _unitOfWork.Courses.FindAllAsync(
                c => c.IsActive,
                include: q => q.Include(x => x.Category)
            );

            if (categoryId.HasValue)
            {
                coursesQuery = coursesQuery.Where(c => c.Category_ID == categoryId.Value);
            }

            var courseDTOs = _mapper.Map<IEnumerable<CourseDTO>>(coursesQuery);

            return View(courseDTOs);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
