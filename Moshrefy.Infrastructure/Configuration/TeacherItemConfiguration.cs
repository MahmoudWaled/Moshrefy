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

            // Unique constraint: Only one assignment per Teacher + Item + Center (for non-deleted records)
            builder.HasIndex(ti => new { ti.TeacherId, ti.ItemId, ti.CenterId })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0")
                .HasDatabaseName("IX_TeacherItem_Unique_Teacher_Item_Center");

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