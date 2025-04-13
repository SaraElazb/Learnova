using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BusinessLogicLayer.DTOs.LessonDtos
{
    public class LessonRequest
    {
        public int Lesson_ID { get; set; }
        
        public int Course_ID { get; set; }
        
        public string Title { get; set; }
        
        public int LessonOrder { get; set; }
        
        public string VideoUri { get; set; }
        
        public int Duration { get; set; }
        
        public string Description { get; set; }
        
        public IEnumerable<SelectListItem> CourseSelectList { get; set; }
    }
} 