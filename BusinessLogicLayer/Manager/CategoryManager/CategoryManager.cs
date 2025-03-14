using AutoMapper;
using BusinessLogicLayer.DTOs.CategoryDtos;
using BusinessLogicLayer.Helpers;
using BusinessLogicLayer.Manager.CategoryManager;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Manager.CategoryManager
{
    public class CategoryManager : ICategoryManager
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoryManager(IUnitOfWork  unitOfWork , IMapper mapper , IWebHostEnvironment webHostEnvironment)
        {
             _unitOfWork = unitOfWork;
              _mapper = mapper;
              _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IEnumerable<CategoryDTO>> GetCategoriesAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }
        public async Task<CategoryRequest?> GetCategoryForEditAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            return category == null ? null : _mapper.Map<CategoryRequest>(category);
        }

        public async Task<bool> CreateCategoryAsync(CategoryRequest request)
        {
            var existingCategory = (await _unitOfWork.Categories.GetAllAsync())
                .FirstOrDefault(c => c.Category_Name.ToLower() == request.Category_Name.ToLower());

            if (existingCategory != null)
                return false; // Category name already exists

            var category = _mapper.Map<Category>(request);

            if (request.Image != null)
            {
                category.ImagePath = ImageHelper.SaveImage(request.Image, "categories", _webHostEnvironment);
            }

            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> UpdateCategoryAsync(int id, CategoryRequest request)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return false;

            _mapper.Map(request, category);

            if (request.Image != null)
            {
                category.ImagePath = ImageHelper.SaveImage(request.Image, "categories", _webHostEnvironment);
            }

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null)
                return false;

            _unitOfWork.Categories.SoftDelete(category);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
