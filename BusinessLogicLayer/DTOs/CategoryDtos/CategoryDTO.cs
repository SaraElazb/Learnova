using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.CategoryDtos
{
    public class CategoryDTO
    {
        public int Category_ID { get; set; }
        public string Category_Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
