using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BusinessLogicLayer.DTOs.QuizDtos;

namespace BusinessLogicLayer.DTOs.LessonDtos
{
    public class LessonDto
    {
        public int Lesson_ID { get; set; }
        
        public int LessonOrder { get; set; }
        
        public string VideoUri { get; set; }
        
        public int Duration { get; set; }
        
        public string Description { get; set; }
        
        public string Title { get; set; }
        
        public int? Quiz_ID { get; set; }
        
        public int Course_ID { get; set; }
        
        public string CourseName { get; set; }
        
        public QuizDto Quiz { get; set; }
    }
} 