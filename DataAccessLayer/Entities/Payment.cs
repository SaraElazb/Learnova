using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Payment
{
    [Key]
    public int Payment_ID { get; set; }

    [Required]
    public int Enrollment_ID { get; set; }

    public decimal Amount { get; set; }

    public string Payment_method { get; set; }

    public DateTime Transaction_date { get; set; }

    [ForeignKey("Enrollment_ID")]
    public Enrollment Enrollment { get; set; }
}