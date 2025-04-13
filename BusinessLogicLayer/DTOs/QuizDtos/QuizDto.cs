using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.DTOs.QuizDtos
{
    public class QuizDto
    {
        public int Quiz_ID { get; set; }
        
        public int Lesson_ID { get; set; }
        
        public string? User_ID { get; set; }
        
        public int Passing_score { get; set; }
        
        public string Title { get; set; }
        
        public string Instructions { get; set; }
        
        public string LessonTitle { get; set; }
        
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
    }
} 