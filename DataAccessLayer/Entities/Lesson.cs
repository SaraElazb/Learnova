using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



public class Lesson
{
    [Key]
    public int Lesson_ID { get; set; }

    public int LessonOrder { get; set; }

    public string VideoUri { get; set; } // URL or file path

    public int Duration { get; set; } // In minutes, perhaps

    public string Description { get; set; }

    [Required]
    public string Title { get; set; }

    public int? Quiz_ID { get; set; } // Nullable if no quiz

    [Required]
    public int Course_ID { get; set; }

    [ForeignKey("Quiz_ID")]
    public Quiz Quiz { get; set; }

    [ForeignKey("Course_ID")]
    public Course Course { get; set; }

    public ICollection<Studies> Studies { get; set; }
}