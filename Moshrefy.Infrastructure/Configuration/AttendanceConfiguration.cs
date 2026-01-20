using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moshrefy.Domain.Entities;

namespace Moshrefy.infrastructure.Configuration
{
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.ToTable("Attendances");

            // Unique constraint: Only one attendance per Student + Session + Center
            // Note: Attendance doesn't implement ISoftDeletable, so no filter needed
            builder.HasIndex(a => new { a.StudentId, a.SessionId, a.CenterId })
                .IsUnique()
                .HasDatabaseName("IX_Attendance_Unique_Student_Session_Center");

            builder.HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Session)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.SessionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Exam)
                .WithMany(e => e.Attendances)
                .HasForeignKey(a => a.ExamId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}