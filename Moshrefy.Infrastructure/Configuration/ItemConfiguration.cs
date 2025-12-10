using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moshrefy.Domain.Entities;

namespace Moshrefy.infrastructure.Configuration
{
    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable("Items");


            builder.HasOne(i => i.ReservedByStudent)
                   .WithMany(s => s.ReservedItems)
                   .HasForeignKey(i => i.ReservedByStudentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(i => i.Price).HasPrecision(18, 2);
        }
    }
}