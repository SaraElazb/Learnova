using AutoMapper;
using BusinessLogicLayer.Helpers;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayer.DTOs.CategoryDtos;

namespace PresentationLayer.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoryController(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            var categoryDTOs = _mapper.Map<IEnumerable<CategoryDTO>>(categories);
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
            var existingCategory = (await _unitOfWork.Categories.GetAllAsync())
               .FirstOrDefault(c => c.Category_Name.ToLower() == request.Category_Name.ToLower());

            if (existingCategory != null)
            {
                ModelState.AddModelError("Category_Name", "Category name already exists.");
                return View(request); // Return the same view with the error message
            }

            var category = _mapper.Map<Category>(request);

            if (request.Image != null)
            {
                category.ImagePath = ImageHelper.SaveImage(request.Image, "categories", _webHostEnvironment);
            }

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            var categoryRequest = _mapper.Map<CategoryRequest>(category);
            return View(categoryRequest);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CategoryRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            _mapper.Map(request, category);

            if (request.Image != null)
            {
                category.ImagePath = ImageHelper.SaveImage(request.Image, "categories", _webHostEnvironment);
            }

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            _unitOfWork.Categories.SoftDelete(category);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));

        }
    }
}
