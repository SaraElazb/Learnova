using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.CourseDtos
{
    public class CourseRequest
    {
        public string Title { get; set; } = string.Empty;   
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Category_ID { get; set; }
        public IFormFile? Image { get; set; }
        public string InstructorId { get; set; }

        public IEnumerable<SelectListItem>? CategorySelectList { get; set; }
    }
}
