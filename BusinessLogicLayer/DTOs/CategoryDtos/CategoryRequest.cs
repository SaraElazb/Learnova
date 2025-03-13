using BusinessLogicLayer.Validators;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.CategoryDtos
{
    public class CategoryRequest
    {
        [Required]
        [MaxLength(100)]
        public string Category_Name { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [AllowedImageFile(6)]
        public IFormFile? Image { get; set; }
    }
}
