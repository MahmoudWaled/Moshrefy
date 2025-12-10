using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moshrefy.Domain.Entities;

namespace Moshrefy.infrastructure.Configuration
{
    public class BaseEntityConfiguration : IEntityTypeConfiguration<BaseEntity>
    {
        public void Configure(EntityTypeBuilder<BaseEntity> builder)
        {
            builder.UseTpcMappingStrategy();

            builder.HasOne(be => be.CreatedBy)
                    .WithMany()
                    .HasForeignKey(be => be.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(be => be.ModifiedBy)
                   .WithMany()
                   .HasForeignKey(be => be.ModifiedById)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
