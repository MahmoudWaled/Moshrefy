using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moshrefy.Domain.Entities;

namespace Moshrefy.infrastructure.Configuration
{
    public class TeacherCourseConfiguration : IEntityTypeConfiguration<TeacherCourse>
    {
        public void Configure(EntityTypeBuilder<TeacherCourse> builder)
        {
            builder.ToTable("TeacherCourses");

            // Unique constraint: Only one assignment per Teacher + Course + Center (for non-deleted records)
            builder.HasIndex(tc => new { tc.TeacherId, tc.CourseId, tc.CenterId })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0")
                .HasDatabaseName("IX_TeacherCourse_Unique_Teacher_Course_Center");

            builder.HasOne(tc => tc.Teacher)
                   .WithMany(t => t.TeacherCourses)
                   .HasForeignKey(tc => tc.TeacherId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(tc => tc.Course)
                     .WithMany(c => c.TeacherCourses)
                     .HasForeignKey(tc => tc.CourseId)
                     .OnDelete(DeleteBehavior.Restrict);
        }
    }
}