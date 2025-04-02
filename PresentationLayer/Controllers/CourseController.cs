using AutoMapper;
using BusinessLogicLayer.DTOs.CourseDtos;
using BusinessLogicLayer.Helpers;
using BusinessLogicLayer.Manager.CategoryManager;
using BusinessLogicLayer.Manager.CourseManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace PresentationLayer.Controllers
{
    public class CourseController : Controller
    {
        
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICourseManager _courseManager;

        private ICategoryManager _categoryManager { get; }

        public CourseController( 
                             IMapper mapper, IWebHostEnvironment webHostEnvironment  , 
                             ICategoryManager categoryManager ,
                             ICourseManager courseManager)
        {
            
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _categoryManager = categoryManager;
             _courseManager = courseManager;
        }
        private async Task<IEnumerable<SelectListItem>> GetCategoriesAsync()
        {
            var categories = await _categoryManager.GetCategoriesAsync();
            return categories.Select(c => new SelectListItem
            {
                Value = c.Category_ID.ToString(),
                Text = c.Category_Name
            });
        }


        public async Task<IActionResult> AdminIndex()
        {
            var courseDTOs = await _courseManager.FindAllAsync();
            return View(courseDTOs);
        }

        public async Task<IActionResult> UserIndex(int? categoryId)
        {
            var categories = await _categoryManager.GetCategoriesAsync();
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Category_ID.ToString(),
                Text = c.Category_Name
            });

            var courseDTOs = await _courseManager.GetCoursesAsync(categoryId);
            return View(courseDTOs);
        }

        public async Task<IActionResult> Details(int id)
        {
            var courseDTO = await _courseManager.FindAsync(id);
            if (courseDTO == null) return NotFound();

             return View(courseDTO);
        }

        public async Task<IActionResult> Create()
        {
            var model = new CourseRequest
            {
                CategorySelectList = await GetCategoriesAsync()
            };
            return View(model);
        }

         
        [HttpPost]
        public async Task<IActionResult> Create(CourseRequest model)
        {
            if (!ModelState.IsValid)
            {
                model.CategorySelectList = await GetCategoriesAsync();
                return View(model);
            }

            await _courseManager.CreateCourseAsync(model);
            return RedirectToAction(nameof(AdminIndex));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var course = await _courseManager.GetByIdAsync(id);
            if (course == null) return NotFound();

            var model = _mapper.Map<CourseRequest>(course);
            model.CategorySelectList = await GetCategoriesAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CourseRequest model)
        {
            if (!ModelState.IsValid)
            {
                model.CategorySelectList = await GetCategoriesAsync();
                return View(model);
            }

            var success = await _courseManager.EditCourseAsync(id, model);
            if (!success) return NotFound();

            return RedirectToAction(nameof(AdminIndex));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var course = await _courseManager.GetByIdAsync(id);
            if (course == null) return NotFound();

            await _courseManager.SoftDelete(course);

            return RedirectToAction(nameof(AdminIndex));
        }
    }
}
