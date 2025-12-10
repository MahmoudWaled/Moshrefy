using Microsoft.EntityFrameworkCore.Storage;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Application.Interfaces.IUnitOfWork;
using Moshrefy.infrastructure.Data;

namespace Moshrefy.infrastructure.UnitOfWork
{
    public class UnitOfWork(
        AppDbContext context,
        IAcademicYearRepository academicYears,
        IAttendanceRepository attendances,
        ICenterRepository centers,
        IClassroomRepository classrooms,
        ICourseRepository courses,
        IEnrollmentRepository enrollments,
        IExamRepository exams,
        IExamResultRepository examResults,
        IInvoiceRepository invoices,
        IItemRepository items,
        IPaymentRepository payments,
        ISessionRepository sessions,
        IStudentRepository students,
        ITeacherRepository teachers,
        ITeacherCourseRepository teacherCourses,
        ITeacherItemRepository teacherItems,
        IRefreshTokenRepository refreshTokens) : IUnitOfWork
    {
        private readonly AppDbContext _context = context;
        private IDbContextTransaction? _transaction;

        public IAcademicYearRepository AcademicYears { get; } = academicYears;
        public IAttendanceRepository Attendances { get; } = attendances;
        public ICenterRepository Centers { get; } = centers;
        public IClassroomRepository Classrooms { get; } = classrooms;
        public ICourseRepository Courses { get; } = courses;
        public IEnrollmentRepository Enrollments { get; } = enrollments;
        public IExamRepository Exams { get; } = exams;
        public IExamResultRepository ExamResults { get; } = examResults;
        public IInvoiceRepository Invoices { get; } = invoices;
        public IItemRepository Items { get; } = items;
        public IPaymentRepository Payments { get; } = payments;
        public ISessionRepository Sessions { get; } = sessions;
        public IStudentRepository Students { get; } = students;
        public ITeacherRepository Teachers { get; } = teachers;
        public ITeacherCourseRepository TeacherCourses { get; } = teacherCourses;
        public ITeacherItemRepository TeacherItems { get; } = teacherItems;
        public IRefreshTokenRepository RefreshTokens { get; } = refreshTokens;

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}