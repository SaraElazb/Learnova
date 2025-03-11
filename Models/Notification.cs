using System.ComponentModel.DataAnnotations;



public class Notification
{
    [Key]
    public int Notification_ID { get; set; }

    [Required]
    public string Message { get; set; }

    public string Type { get; set; }

    public ICollection<Receive> Receives { get; set; }
}