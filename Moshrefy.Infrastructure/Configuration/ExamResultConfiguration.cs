using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moshrefy.Domain.Entities;

namespace Moshrefy.infrastructure.Configuration
{
    public class ExamResultConfiguration : IEntityTypeConfiguration<ExamResult>
    {
        public void Configure(EntityTypeBuilder<ExamResult> builder)
        {
            builder.ToTable("ExamResults");

            // Unique constraint: Only one result per Student + Exam + Center (for non-deleted records)
            builder.HasIndex(er => new { er.StudentId, er.ExamId, er.CenterId })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0")
                .HasDatabaseName("IX_ExamResult_Unique_Student_Exam_Center");

            builder.HasOne(er => er.Exam)
                   .WithMany(e => e.ExamResults)
                   .HasForeignKey(er => er.ExamId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(er => er.Student)
                .WithMany(s => s.ExamResults)
                .HasForeignKey(er => er.StudentId)
                .OnDelete(DeleteBehavior.Restrict);



        }
    }
}