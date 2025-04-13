using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasMany(u => u.Enrollments)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.User_ID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.User_ID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Submissions)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.User_ID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany<Quiz>()
                .WithOne(q => q.User)
                .HasForeignKey(q => q.User_ID)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

      
            builder.Property(u => u.Registration_date)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
