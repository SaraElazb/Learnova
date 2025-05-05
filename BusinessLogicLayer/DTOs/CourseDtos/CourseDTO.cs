using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer.DTOs.LessonDtos;

namespace BusinessLogicLayer.DTOs.CourseDtos
{
    public class CourseDTO
    {
        public int Course_ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Rating { get; set; }
        public string? ImagePath { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public string CategoryName { get; set; }
        public int EnrollmentCount { get; set; }
        public string InstructorId { get; set; }
        public string InstructorName { get; set; }
        public List<LessonDto> Lessons { get; set; } = new List<LessonDto>();
    }
}
