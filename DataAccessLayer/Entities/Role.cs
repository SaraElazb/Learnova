using System.ComponentModel.DataAnnotations;



public class Role
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string RoleName { get; set; }


    public ICollection<User> Users { get; set; }
}