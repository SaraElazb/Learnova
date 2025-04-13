using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.DTOs.QuizDtos
{
    public class QuestionRequest
    {
        public int QuestionID { get; set; }
        
        [Required(ErrorMessage = "Quiz is required")]
        public int QuizID { get; set; }
        
        [Required(ErrorMessage = "Question text is required")]
        [MaxLength(500, ErrorMessage = "Question text cannot be longer than 500 characters")]
        public string QuestionText { get; set; }
        
        [Required(ErrorMessage = "Score is required")]
        [Range(1, 100, ErrorMessage = "Score must be between 1 and 100")]
        public int Score { get; set; }
        
        [Required(ErrorMessage = "Right answer is required")]
        [MaxLength(200, ErrorMessage = "Right answer cannot be longer than 200 characters")]
        public string RightAns { get; set; }
        
        public List<string> Answers { get; set; } = new List<string>();
    }
} 