using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class Studies
{
    [Key, Column(Order = 0)]
    public int User_ID { get; set; }

    [Key, Column(Order = 1)]
    public int Lesson_ID { get; set; }

    public string Status { get; set; } // Could be an enum

    public int Score { get; set; }

    public DateTime? CompletionDate { get; set; }

    [ForeignKey("User_ID")]
    public User User { get; set; }

    [ForeignKey("Lesson_ID")]
    public Lesson Lesson { get; set; }
}