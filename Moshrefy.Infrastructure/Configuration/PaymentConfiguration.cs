using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moshrefy.Domain.Entities;

namespace Moshrefy.infrastructure.Configuration
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {

            builder.HasOne(p => p.Invoice)
                   .WithMany(i => i.Payments)
                   .HasForeignKey(p => p.InvoiceId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Student)
                   .WithMany(s => s.Payments)
                   .HasForeignKey(p => p.StudentId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Session)
                   .WithMany(s => s.Payments)
                   .HasForeignKey(p => p.SessionId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Exam)
                     .WithMany(e => e.Payments)
                     .HasForeignKey(p => p.ExamId)
                     .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.TeacherItem)
                     .WithMany(ti => ti.Payments)
                     .HasForeignKey(p => p.TeacherItemId)
                     .OnDelete(DeleteBehavior.Restrict);



        }
    }
}