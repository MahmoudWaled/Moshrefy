using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moshrefy.Domain.Entities;

namespace Moshrefy.infrastructure.Configuration
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.ToTable("Enrollments");

            builder.HasOne(e => e.Student)
                   .WithMany(s => s.Enrollments)
                   .HasForeignKey(e => e.StudentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Course)
                     .WithMany(c => c.Enrollments)
                     .HasForeignKey(e => e.CourseId)
                     .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();



        }
    }
}