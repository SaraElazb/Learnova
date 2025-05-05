using AutoMapper;
using BusinessLogicLayer.Helpers;
using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayer.DTOs.CategoryDtos;
using BusinessLogicLayer.Manager.CategoryManager;
using Microsoft.AspNetCore.Authorization;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICategoryManager _categoryManager;

        public CategoryController(
                                    IMapper mapper, IWebHostEnvironment webHostEnvironment,
                                    ICategoryManager categoryManager)
        {
           
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _categoryManager = categoryManager;
        }
        [Route("/categories-list")]
        public async Task<IActionResult> Index()
        {
            var categoryDTOs = await _categoryManager.GetCategoriesAsync();
            return View(categoryDTOs);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            bool isCreated = await _categoryManager.CreateCategoryAsync(request);
            if (!isCreated)
            {
                ModelState.AddModelError("Category_Name", "Category name already exists.");
                return View(request);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var categoryRequest = await _categoryManager.GetCategoryForEditAsync(id);
            if (categoryRequest == null)
                return NotFound();

            return View(categoryRequest);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CategoryRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            bool isUpdated = await _categoryManager.UpdateCategoryAsync(id, request);
            if (!isUpdated)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            bool isDeleted = await _categoryManager.DeleteCategoryAsync(id);
            if (!isDeleted)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}
