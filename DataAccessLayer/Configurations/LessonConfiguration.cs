using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations
{
    public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            builder.HasOne(l => l.Quiz)
                .WithOne(q => q.Lesson)
                .HasForeignKey<Quiz>(q => q.Lesson_ID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
