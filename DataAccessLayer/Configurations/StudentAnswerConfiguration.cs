using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccessLayer.Configurations
{
    public class StudentAnswerConfiguration : IEntityTypeConfiguration<StudentAnswer>
    {
        public void Configure(EntityTypeBuilder<StudentAnswer> builder)
        {
            builder.HasKey(sa => sa.ID);

            builder.HasOne(sa => sa.Submission)
                   .WithMany()
                   .HasForeignKey(sa => sa.Submission_ID)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sa => sa.Question)
                   .WithMany()
                   .HasForeignKey(sa => sa.Question_ID)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sa => sa.Answer)
                   .WithMany()
                   .HasForeignKey(sa => sa.Answer_ID)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 