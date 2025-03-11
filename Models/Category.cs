using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Category
{
    [Key]
    public int Category_ID { get; set; }

    [Required]
    public string Category_Name { get; set; }

    public string Description { get; set; }

    public ICollection<Course> Courses { get; set; }
}