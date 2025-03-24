using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasMany(c => c.Courses)
             .WithOne(c => c.Category)
             .HasForeignKey(c => c.Category_ID)
             .OnDelete(DeleteBehavior.Restrict);

            builder.Property(c => c.IsActive)
                .HasDefaultValue(true);
        }
    }
}
