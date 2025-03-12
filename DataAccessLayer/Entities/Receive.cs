using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



public class Receive
{
    [Key]
    public int User_ID { get; set; }

    [Key]
    public int Notification_ID { get; set; }

    [Key]
    public DateTime Datetime { get; set; }

    [ForeignKey("User_ID")]
    public User User { get; set; }

    [ForeignKey("Notification_ID")]
    public Notification Notification { get; set; }
}