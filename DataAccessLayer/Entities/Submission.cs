using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class Submission
{
    [Key]
    public int ID { get; set; }

    [Required]
    public int Quiz_ID { get; set; }

    [Required]
    public string User_ID { get; set; }

    public int Score { get; set; }

    public DateTime SubmissionDate { get; set; }

    public string Status { get; set; } 

    [ForeignKey("Quiz_ID")]
    public Quiz Quiz { get; set; }

    [ForeignKey("User_ID")]
    public User User { get; set; }
}