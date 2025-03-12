using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



public class Enrollment
{
    [Key] public int Enrollment_ID { get; set; }

    public DateTime Enrollment_date { get; set; }

    [Required] public int Course_ID { get; set; }

    [Required] public int User_ID { get; set; }

    [ForeignKey("Course_ID")] public Course Course { get; set; }

    [ForeignKey("User_ID")] public User User { get; set; }
    public Certificate Certificate { get; set; } 
    public Payment Payment { get; set; }
}