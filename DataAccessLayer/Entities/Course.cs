using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Course
{
    [Key]
    public int Course_ID { get; set; }

    public decimal Rating { get; set; }

    [Required]
    public string Title { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }
    public string? ImagePath { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool IsActive { get; set; } = true;

    public int Category_ID { get; set; }

    public string Overview { get; set; }

    public string Benefit { get; set; }

    public string Section { get; set; }

    [ForeignKey("Category_ID")]
    public Category Category { get; set; }

    public int Instructor_ID { get; set; }

    [ForeignKey("Instructor_ID")]
    public User Instructor { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; }
    public ICollection<Review> Reviews { get; set; }
    public ICollection<Lesson> Lessons { get; set; }
}



