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

            // Unique constraint: Only one enrollment per Student + Course + Center (for non-deleted records)
            builder.HasIndex(e => new { e.StudentId, e.CourseId, e.CenterId })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0")
                .HasDatabaseName("IX_Enrollment_Unique_Student_Course_Center");



        }
    }
}