using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    public class StudentAnswer
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int Submission_ID { get; set; }

        [Required]
        public int Question_ID { get; set; }

        [Required]
        public int Answer_ID { get; set; }

        public bool IsCorrect { get; set; }

        [ForeignKey("Submission_ID")]
        public Submission Submission { get; set; }

        [ForeignKey("Question_ID")]
        public Question Question { get; set; }

        [ForeignKey("Answer_ID")]
        public Answer Answer { get; set; }
    }
} 