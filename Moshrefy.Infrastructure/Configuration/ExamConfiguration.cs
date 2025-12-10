using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moshrefy.Domain.Entities;

namespace Moshrefy.infrastructure.Configuration
{
    public class ExamConfiguration : IEntityTypeConfiguration<Exam>
    {
        public void Configure(EntityTypeBuilder<Exam> builder)
        {
            builder.ToTable("Exams");


            builder.HasOne(e => e.TeacherCourse)
                   .WithMany(tc => tc.Exams)
                   .HasForeignKey(e => e.TeacherCourseId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Classroom)
                   .WithMany(c => c.Exams)
                   .HasForeignKey(e => e.ClassroomId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Course)
                     .WithMany(c => c.Exams)
                     .HasForeignKey(e => e.CourseId)
                     .OnDelete(DeleteBehavior.Restrict);


        }
    }
}