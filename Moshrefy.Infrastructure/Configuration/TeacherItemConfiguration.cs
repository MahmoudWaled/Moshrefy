using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moshrefy.Domain.Entities;

namespace Moshrefy.infrastructure.Configuration
{
    public class TeacherItemConfiguration : IEntityTypeConfiguration<TeacherItem>
    {
        public void Configure(EntityTypeBuilder<TeacherItem> builder)
        {
            builder.ToTable("TeacherItems");

            builder.HasIndex(ti => new { ti.TeacherId, ti.ItemId }).IsUnique();

            builder.HasOne(ti => ti.Teacher)
                   .WithMany(t => t.TeacherItems)
                   .HasForeignKey(ti => ti.TeacherId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ti => ti.Item)
                     .WithMany(i => i.TeacherItems)
                     .HasForeignKey(ti => ti.ItemId)
                     .OnDelete(DeleteBehavior.Restrict);

        }
    }
}