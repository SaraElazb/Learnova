using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



public class Review
{
    [Key]
    public int Review_ID { get; set; }

    [Required]
    public int User_ID { get; set; }

    [Required]
    public int Course_ID { get; set; }

    public int Rating { get; set; }

    public string Comment { get; set; }

    
    [ForeignKey("User_ID")]
    public User User { get; set; }

    [ForeignKey("Course_ID")]
    public Course Course { get; set; }
}