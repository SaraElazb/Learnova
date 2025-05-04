using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Question
{
    [Key] public int QuestionID { get; set; }

    [Required] public int QuizID { get; set; }

    [Required]
    public string QuestionText { get; set; } = string.Empty;

    public int Score { get; set; }

    public string RightAns { get; set; } = string.Empty;

    [ForeignKey("QuizID")] public Quiz Quiz { get; set; }

    public ICollection<Answer> Answers { get; set; } = new List<Answer>();
}