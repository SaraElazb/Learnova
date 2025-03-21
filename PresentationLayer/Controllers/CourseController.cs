using AutoMapper;
using BusinessLogicLayer.DTOs.CourseDtos;
using BusinessLogicLayer.Helpers;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace PresentationLayer.Controllers
{
    public class CourseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CourseController(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }
        private async Task<IEnumerable<SelectListItem>> GetCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.FindAllAsync(c => c.IsActive);
            return categories.Select(c => new SelectListItem
            {
                Value = c.Category_ID.ToString(),
                Text = c.Category_Name
            });
        }


        public async Task<IActionResult> AdminIndex()
        {
            var courses = await _unitOfWork.Courses.FindAllAsync(c => c.IsActive, include: q => q.Include(c => c.Category));
            var activeCourses = courses.Where(c => c.IsActive);
            var courseDTOs = _mapper.Map<IEnumerable<CourseDTO>>(activeCourses);
            return View(courseDTOs);
        }

      [Route("/Courses")]
        public async Task<IActionResult> UserIndex(int? categoryId)
        {
            var categories = await _unitOfWork.Categories.FindAllAsync(c => c.IsActive);
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Category_ID.ToString(),
                Text = c.Category_Name
            });

            // Fetch courses based on selected category (or all active courses)
            var coursesQuery = _unitOfWork.Courses.FindAllAsync(c => c.IsActive, q => q.Include(c => c.Category));
            var courses = await coursesQuery;

            if (categoryId.HasValue)
            {
                courses = courses.Where(c => c.Category_ID == categoryId.Value);
            }

            var courseDTOs = _mapper.Map<IEnumerable<CourseDTO>>(courses);
            return View(courseDTOs);
        }


        public async Task<IActionResult> Details(int id)
        {
            var course = await _unitOfWork.Courses.FindAsync(c => c.Course_ID == id, include: q => q.Include(c => c.Category));
            if (course == null) return NotFound();

            var courseDTO = _mapper.Map<CourseDTO>(course);
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
        [HttpPost]
        public async Task<IActionResult> Create(CourseRequest model)
        {
            if (!ModelState.IsValid)
            {
                model.CategorySelectList = await GetCategoriesAsync();
                return View(model);
            }

            var course = _mapper.Map<Course>(model);

            if (model.Image != null)
            {
                course.ImagePath = ImageHelper.SaveImage(model.Image, "CourseImages", _webHostEnvironment);
            }

            await _unitOfWork.Courses.AddAsync(course);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(AdminIndex));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
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

            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null) return NotFound();

            _mapper.Map(model, course);

            if (model.Image != null)
            {
                course.ImagePath = ImageHelper.SaveImage(model.Image, "CourseImages", _webHostEnvironment);
            }

            _unitOfWork.Courses.Update(course);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(AdminIndex));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null) return NotFound();

            _unitOfWork.Courses.SoftDelete(course); 
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(AdminIndex));
        }
    }
}
