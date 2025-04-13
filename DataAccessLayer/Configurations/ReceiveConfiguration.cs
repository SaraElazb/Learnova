using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Configurations
{
    public class ReceiveConfiguration : IEntityTypeConfiguration<Receive>
    {
        public void Configure(EntityTypeBuilder<Receive> builder)
        {
            builder.HasKey(r => new { r.User_ID, r.Notification_ID, r.Datetime });

            builder.HasOne(r => r.User)
                .WithMany(u => u.Receives)
                .HasForeignKey(r => r.User_ID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(r => r.Notification)
                .WithMany(n => n.Receives)
                .HasForeignKey(r => r.Notification_ID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
