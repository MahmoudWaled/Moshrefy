using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moshrefy.Domain.Entities;

namespace Moshrefy.infrastructure.Configuration
{
    public class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.ToTable("Sessions");

            builder.Property(s => s.Price).HasPrecision(18, 2);

            builder.HasOne(s => s.Classroom)
                .WithMany(c => c.Sessions)
                .HasForeignKey(s => s.ClassroomId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.TeacherCourse)
                .WithMany(tc => tc.Sessions)
                .HasForeignKey(s => s.TeacherCourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.AcademicYear)
                .WithMany(ac => ac.Sessions)
                .HasForeignKey(ac => ac.AcademicYearId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}