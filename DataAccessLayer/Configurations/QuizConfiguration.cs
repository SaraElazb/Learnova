using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations
{

    public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
    {
        public void Configure(EntityTypeBuilder<Quiz> builder)
        {
            builder.HasMany(q => q.Submissions)
                .WithOne(s => s.Quiz)
                .HasForeignKey(s => s.Quiz_ID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(q => q.Questions)
                .WithOne(q => q.Quiz)
                .HasForeignKey(q => q.QuizID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
