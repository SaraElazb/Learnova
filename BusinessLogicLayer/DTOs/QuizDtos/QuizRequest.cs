using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BusinessLogicLayer.DTOs.QuizDtos
{
    public class QuizRequest
    {
        public int Quiz_ID { get; set; }
        
        public int Lesson_ID { get; set; }
        
        public string Title { get; set; }
        
        public int Passing_score { get; set; }
        
        public string Instructions { get; set; } = string.Empty;
        
        public IEnumerable<SelectListItem> LessonSelectList { get; set; }
    }
} 