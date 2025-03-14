using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasMany(c => c.Enrollments)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.Course_ID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(c => c.Reviews)
                .WithOne(r => r.Course)
                .HasForeignKey(r => r.Course_ID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(c => c.Lessons)
                .WithOne(l => l.Course)
                .HasForeignKey(l => l.Course_ID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Property(c => c.Price)
                .HasColumnType("decimal(18,2)");

            builder.Property(c => c.CreatedDate)
                .HasDefaultValueSql("GETDATE()");

            builder.Property(c => c.Rating)
                .HasPrecision(18, 2);
            builder.Property(c => c.IsActive)
             .HasDefaultValue(true);

        }
    }
}
