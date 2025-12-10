using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moshrefy.Domain.Entities;

namespace Moshrefy.infrastructure.Configration
{
    public class CenterConfiguration : IEntityTypeConfiguration<Center>
    {
        public void Configure(EntityTypeBuilder<Center> builder)
        {
            builder.HasMany(c => c.AcademicYears)
                   .WithOne()
                   .HasForeignKey(ay => ay.CenterId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Attendances)
                   .WithOne()
                   .HasForeignKey(a => a.CenterId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Classrooms)
                   .WithOne()
                   .HasForeignKey(cl => cl.CenterId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Courses)
                   .WithOne()
                   .HasForeignKey(co => co.CenterId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Enrollments)
                   .WithOne()
                   .HasForeignKey(e => e.CenterId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Exams)
                   .WithOne()
                   .HasForeignKey(ex => ex.CenterId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.ExamResults)
                   .WithOne()
                   .HasForeignKey(er => er.CenterId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Invoices)
                   .WithOne()
                   .HasForeignKey(i => i.CenterId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Payments)
                   .WithOne()
                   .HasForeignKey(p => p.CenterId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Students)
                   .WithOne()
                   .HasForeignKey(s => s.CenterId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Teachers)
                   .WithOne()
                   .HasForeignKey(t => t.CenterId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.TeacherCourses)
                   .WithOne()
                   .HasForeignKey(tc => tc.CenterId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.TeacherItems)
                   .WithOne()
                   .HasForeignKey(ti => ti.CenterId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Items)
                   .WithOne()
                   .HasForeignKey(it => it.CenterId)
                   .OnDelete(DeleteBehavior.Restrict);

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