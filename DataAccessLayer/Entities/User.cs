using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User:IdentityUser
{
 
    
    [Required, MaxLength(50)] 
    public string First_name { get; set; }

    [Required, MaxLength(50)] 
    public string Last_name { get; set; }

 

    public string? Profile_picture { get; set; }
    
  
    public DateTime Registration_date { get; set; }



    public ICollection<Submission> Submissions { get; set; }
    public ICollection<Review> Reviews { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; }
    public ICollection<Studies> Studies { get; set; } 
    public ICollection<Receive> Receives { get; set; }
}