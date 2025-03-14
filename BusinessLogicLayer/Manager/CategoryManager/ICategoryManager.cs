using BusinessLogicLayer.DTOs.CategoryDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Manager.CategoryManager
{
    public interface ICategoryManager
    {
        Task<IEnumerable<CategoryDTO>> GetCategoriesAsync();
        
        Task<CategoryRequest?> GetCategoryForEditAsync(int id);
        Task<bool> CreateCategoryAsync(CategoryRequest request);
        Task<bool> UpdateCategoryAsync(int id, CategoryRequest request);
        Task<bool> DeleteCategoryAsync(int id);

    }
}
