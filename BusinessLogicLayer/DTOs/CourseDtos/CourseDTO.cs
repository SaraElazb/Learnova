using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.DTOs.CourseDtos
{
    public class CourseDTO
    {
        public int Course_ID { get; set; }
        public string Title { get; set; }
        public string Course_Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Rating { get; set; }
        public string? ImagePath { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public string CategoryName { get; set; }
    }
}
