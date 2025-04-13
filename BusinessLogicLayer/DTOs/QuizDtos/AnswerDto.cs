using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.DTOs.QuizDtos
{
    public class AnswerDto
    {
        public int AnswerID { get; set; }
        
        public int QuestionID { get; set; }
        
        public string Answers { get; set; }
        
        public bool IsCorrect { get; set; }
    }
} 