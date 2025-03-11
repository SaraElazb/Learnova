using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
   [Key] 
   public int User_ID { get; set; } 
    
    [Required] 
    public int Role_ID { get; set; }
    
    [Required, MaxLength(50)] 
    public string First_name { get; set; }

    [Required, MaxLength(50)] 
    public string Last_name { get; set; }

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; }

    public string Profile_picture { get; set; }
    
    [Required, MaxLength(100)] 
    public string Password { get; set; }

    public DateTime Registration_date { get; set; }

    [ForeignKey("Role_ID")] public Role Role { get; set; }

    public ICollection<Submission> Submissions { get; set; }
    public ICollection<Review> Reviews { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; }
    public ICollection<Studies> Studies { get; set; } 
    public ICollection<Receive> Receives { get; set; }
}