using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class Answer
    {
        [Key]
        public int AnswerID { get; set; }  

        [Required]
        public int QuestionID { get; set; }  

        [Required]
        public string Answers { get; set; }

        [ForeignKey("QuestionID")] 
        public Question Question { get; set; } = new Question();
    }