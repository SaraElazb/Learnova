using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Quiz
{
    [Key]
    public int Quiz_ID { get; set; }

    [Required]
    public int Lesson_ID { get; set; }

    public int? User_ID { get; set; }

    public int Passing_score { get; set; }

    [Required]
    public string Title { get; set; }

    public string Instructions { get; set; } = string.Empty;

    [ForeignKey("Lesson_ID")]
    public Lesson Lesson { get; set; }

    [ForeignKey("User_ID")]
    public User User { get; set; }

    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}