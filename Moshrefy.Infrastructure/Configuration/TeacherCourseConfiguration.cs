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

            builder.HasIndex(tc => new { tc.TeacherId, tc.CourseId }).IsUnique();

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