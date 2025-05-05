using AutoMapper;
using BusinessLogicLayer.DTOs.CourseDtos;
using BusinessLogicLayer.Helpers;
using BusinessLogicLayer.Manager.CategoryManager;
using BusinessLogicLayer.Manager.CourseManager;
using BusinessLogicLayer.Manager.LessonManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PresentationLayer.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICourseManager _courseManager;
        private readonly ILessonManager _lessonManager;

        private ICategoryManager _categoryManager { get; }

        public CourseController( 
                             IMapper mapper, IWebHostEnvironment webHostEnvironment  , 
                             ICategoryManager categoryManager ,
                             ICourseManager courseManager,
                             ILessonManager lessonManager)
        {
            
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _categoryManager = categoryManager;
             _courseManager = courseManager;
            _lessonManager = lessonManager;
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

        public async Task<IActionResult> List()
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

            // Get lessons for this course
            var lessons = await _lessonManager.GetLessonsByCourseAsync(id);
            ViewBag.Lessons = lessons;

            return View(courseDTO);
        }

        // GET: Courses/Create
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create()
        {
            var model = new CourseRequest
            {
                CategorySelectList = await GetCategoriesAsync(),
                InstructorId = User.FindFirstValue(ClaimTypes.NameIdentifier) // Set current user as instructor
            };
            return View(model);
        }

        // POST: Courses/Create
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create(CourseRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.CategorySelectList = await GetCategoriesAsync();
                    return View(model);
                }

                // Set the instructor ID to the current user
                model.InstructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                await _courseManager.CreateCourseAsync(model);
                TempData["SuccessMessage"] = "Course created successfully!";
                return RedirectToAction("Dashboard", "Instructor"); // Redirect to instructor dashboard
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error creating course: " + ex.Message;
                model.CategorySelectList = await GetCategoriesAsync();
                return View(model);
            }
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _courseManager.GetByIdAsync(id);
            if (course == null) return NotFound();

            // Check if current user is the course instructor
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (course.InstructorId != currentUserId)
            {
                TempData["ErrorMessage"] = "You do not have permission to edit this course.";
                return RedirectToAction("Dashboard", "Instructor");
            }

            var model = _mapper.Map<CourseRequest>(course);
            model.CategorySelectList = await GetCategoriesAsync();
            model.InstructorId = course.InstructorId; // Preserve the instructor ID
            
            // Pass the current image path via ViewBag
            ViewBag.CurrentImage = course.ImagePath;
            
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int id, CourseRequest model)
        {
            if (!ModelState.IsValid)
            {
                model.CategorySelectList = await GetCategoriesAsync();
                ViewBag.CurrentImage = (await _courseManager.GetByIdAsync(id))?.ImagePath;
                return View(model);
            }

            // Set the instructor ID to the current user to verify ownership
            model.InstructorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var success = await _courseManager.EditCourseAsync(id, model);
            if (!success)
            {
                TempData["ErrorMessage"] = "You do not have permission to edit this course or the course was not found.";
                return RedirectToAction("Dashboard", "Instructor");
            }

            TempData["SuccessMessage"] = "Course updated successfully!";
            return RedirectToAction("Dashboard", "Instructor");
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _courseManager.GetByIdAsync(id);
            if (course == null) return NotFound();

            // Check if current user is the course instructor
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (course.InstructorId != currentUserId)
            {
                TempData["ErrorMessage"] = "You do not have permission to delete this course.";
                return RedirectToAction("Dashboard", "Instructor");
            }

            await _courseManager.SoftDelete(course);
            
            TempData["SuccessMessage"] = "Course deleted successfully!";
            return RedirectToAction("Dashboard", "Instructor");
        }
    }
}
