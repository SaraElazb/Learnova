using System;

namespace PresentationLayer.Models
{
    public class QuizResultViewModel
    {
        public int QuizId { get; set; }
        public string QuizTitle { get; set; }
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
        public int Score { get; set; }
        public int PassingScore { get; set; }
        public bool Passed { get; set; }
        public int CourseId { get; set; }
    }
} 