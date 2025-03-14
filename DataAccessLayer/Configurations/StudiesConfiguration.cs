using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations
{
    public class StudiesConfiguration : IEntityTypeConfiguration<Studies>
    {
        public void Configure(EntityTypeBuilder<Studies> builder)
        {
            builder.HasKey(s => new { s.User_ID, s.Lesson_ID });

            builder.HasOne(s => s.User)
                .WithMany(u => u.Studies)
                .HasForeignKey(s => s.User_ID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Lesson)
                .WithMany(l => l.Studies)
                .HasForeignKey(s => s.Lesson_ID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
