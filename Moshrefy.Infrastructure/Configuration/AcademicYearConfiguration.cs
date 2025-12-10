using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moshrefy.Domain.Entities;

namespace Moshrefy.infrastructure.Configuration
{
    public class AcademicYearConfiguration : IEntityTypeConfiguration<AcademicYear>
    {
        public void Configure(EntityTypeBuilder<AcademicYear> builder)
        {
            builder.ToTable("AcademicYears");


            builder.HasOne<Center>()
                   .WithMany(c => c.AcademicYears)
                   .HasForeignKey(ay => ay.CenterId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}