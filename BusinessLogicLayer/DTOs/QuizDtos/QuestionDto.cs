using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.DTOs.QuizDtos
{
    public class QuestionDto
    {
        public int QuestionID { get; set; }
        
        public int QuizID { get; set; }
        
        public int Score { get; set; }
        
        public string QuestionText { get; set; }
        
        public string RightAns { get; set; }
        
        public List<AnswerDto> Answers { get; set; } = new List<AnswerDto>();
    }
} 