using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.HasOne(e => e.Certificate)
                .WithOne(c => c.Enrollment)
                .HasForeignKey<Certificate>(c => c.EnrollmentID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.Payment)
                .WithOne(p => p.Enrollment)
                .HasForeignKey<Payment>(p => p.Enrollment_ID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Property(e => e.Enrollment_date)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
