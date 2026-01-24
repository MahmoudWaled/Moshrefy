using Moshrefy.Application.DTOs.Center;
using Moshrefy.Application.DTOs.Common;
using Moshrefy.Application.DTOs.Statistics;
using Moshrefy.Application.DTOs.User;
using Moshrefy.Domain.Entities;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IServices
{
    // Service for SuperAdmin to manage all centers and users across the system.
    public interface ISuperAdminService
    {
        // -------- User Management (All Centers) --------
        Task<UserResponseDTO> CreateAdminForCenterAsync(CreateUserDTO createUserDTO);
        Task<UserResponseDTO?> GetCenterAdminAsync(int centerId);

        // DataTables
        //Task<DataTableResponse<CenterResponseDTO>> GetCentersDataTableAsync(DataTableRequest request);
        Task<DataTableResponse<UserResponseDTO>> GetUsersDataTableAsync(DataTableRequest request);
        Task<UserResponseDTO> CreateCenterAdminAsync(int centerId, CreateUserDTO createUserDTO);
        Task<UserResponseDTO> CreateUserForCenterAsync(int centerId, CreateUserDTO createUserDTO);
        Task<List<UserResponseDTO>> GetAllUsersAsync(PaginationParameter paginationParamter);
        Task<List<UserResponseDTO>> GetActiveUsersAsync(PaginationParameter paginationParamter);
        Task<List<UserResponseDTO>> GetInactiveUsersAsync(PaginationParameter paginationParamter);
        Task<List<UserResponseDTO>> GetDeletedUsersAsync(PaginationParameter paginationParamter);
        Task<int> GetUsersByCenterIdCountAsync(int centerId);
        Task<int> GetTotalUsersCountAsync();
        Task<int> GetActiveUsersCountAsync();
        Task<int> GetInactiveUsersCountAsync();
        Task<int> GetDeletedUsersCountAsync();
        Task<PaginatedResult<UserResponseDTO>> GetUsersByCenterIdAsync(int centerId, PaginationParameter paginationParamter);
        Task<List<UserResponseDTO>> GetUsersByRoleAsync(string roleName, PaginationParameter paginationParamter);
        Task<UserResponseDTO> GetUserByIdAsync(string userId);
        Task<UserResponseDTO> GetUserByEmailAsync(string email);
        Task<UserResponseDTO> GetUserByUsernameAsync(string username);
        Task UpdateUserAsync(string userId, UpdateUserDTO updateUserDTO);
        Task UpdateUserInAnyCenterAsync(string userId, UpdateUserDTO updateUserDTO);
        Task DeleteUserAsync(string userId); // Hard delete
        Task SoftDeleteUserAsync(string userId);
        Task SoftDeleteUserForCenterAsync(int centerId, string userId);
        Task DeleteUserFromAnyCenterAsync(string userId);
        Task RestoreUserAsync(string userId);
        Task ActivateUserAsync(string userId);
        Task DeactivateUserAsync(string userId);
        Task UpdateUserRoleAsync(string userId, UpdateUserRoleDTO updateUserRoleDTO);

        // -------- System-Wide Monitoring (Read-Only) --------
        Task<int> GetTotalTeachersCountAsync();
        Task<int> GetTotalStudentsCountAsync();
        Task<int> GetTotalCoursesCountAsync();
        Task<int> GetTotalClassroomsCountAsync();
        Task<int> GetDeletedTeachersCountAsync();
        Task<int> GetDeletedStudentsCountAsync();
        Task<int> GetDeletedCoursesCountAsync();
        Task<int> GetDeletedClassroomsCountAsync();

        // -------- Teacher Management --------
        Task RestoreTeacherAsync(int teacherId);

        // -------- Course Management --------
        Task RestoreCourseAsync(int courseId);

        // -------- Classroom Management --------
        Task RestoreClassroomAsync(int classroomId);

        // -------- Session Management --------
        Task RestoreSessionAsync(int sessionId);

        // -------- Exam Management --------
        Task RestoreExamAsync(int examId);

        // -------- ExamResult Management --------
        Task RestoreExamResultAsync(int examResultId);

        // -------- Invoice Management --------
        Task RestoreInvoiceAsync(int invoiceId);

        // -------- Item Management --------
        Task RestoreItemAsync(int itemId);

        // -------- Statistics --------
        Task<SystemStatisticsDTO> GetSystemStatisticsAsync();

        // -------- TeacherCourse Management --------
        Task RestoreTeacherCourseAsync(int teacherCourseId);

        // -------- TeacherItem Management --------
        Task RestoreTeacherItemAsync(int teacherItemId);

        // -------- Enrollment Management --------
        Task RestoreEnrollmentAsync(int enrollmentId);

        // -------- Student Management --------
        Task RestoreStudentAsync(int studentId);
    }
}