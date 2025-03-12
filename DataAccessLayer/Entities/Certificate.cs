using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Certificate
{
    [Key]
    public int CertificateID { get; set; }

    [Required]
    public int EnrollmentID { get; set; }  
    public Enrollment Enrollment { get; set; }
}