using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Identity;
using System.Reflection;

namespace Moshrefy.infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        DbSet<AcademicYear> AcademicYears { get; set; }
        DbSet<Attendance> Attendances { get; set; }
        DbSet<Center> Centers { get; set; }
        DbSet<Classroom> Classrooms { get; set; }
        DbSet<Course> Courses { get; set; }
        DbSet<Enrollment> Enrollments { get; set; }
        DbSet<Exam> Exams { get; set; }
        DbSet<ExamResult> ExamResults { get; set; }
        DbSet<Invoice> Invoices { get; set; }
        DbSet<Item> Items { get; set; }
        DbSet<Payment> Payments { get; set; }
        DbSet<Session> Sessions { get; set; }
        DbSet<Student> Students { get; set; }
        DbSet<Teacher> Teachers { get; set; }
        DbSet<TeacherCourse> TeacherCourses { get; set; }
        DbSet<TeacherItem> TeacherItems { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        }
    }
}
