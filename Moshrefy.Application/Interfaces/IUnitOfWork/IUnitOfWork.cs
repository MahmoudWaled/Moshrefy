using Moshrefy.Application.Interfaces.IRepositories;

namespace Moshrefy.Application.Interfaces.IUnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IAcademicYearRepository AcademicYears { get; }
        IAttendanceRepository Attendances { get; }
        ICenterRepository Centers { get; }
        IClassroomRepository Classrooms { get; }
        ICourseRepository Courses { get; }
        IEnrollmentRepository Enrollments { get; }
        IExamRepository Exams { get; }
        IExamResultRepository ExamResults { get; }
        IInvoiceRepository Invoices { get; }
        IItemRepository Items { get; }
        IPaymentRepository Payments { get; }
        ISessionRepository Sessions { get; }
        IStudentRepository Students { get; }
        ITeacherRepository Teachers { get; }
        ITeacherCourseRepository TeacherCourses { get; }
        ITeacherItemRepository TeacherItems { get; }
        IRefreshTokenRepository RefreshTokens { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}