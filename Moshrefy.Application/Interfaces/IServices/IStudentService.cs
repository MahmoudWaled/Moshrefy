using Moshrefy.Application.DTOs.Student;
using Moshrefy.Domain.Enums;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IServices
{
    public interface IStudentService
    {
        Task<StudentResponseDTO> CreateAsync(CreateStudentDTO createStudentDTO);
        Task<StudentResponseDTO?> GetByIdAsync(int id);
        Task<List<StudentResponseDTO>> GetAllAsync(PaginationParameter paginationParamter);
        Task<List<StudentResponseDTO>> GetByNameAsync(string name);
        Task<StudentResponseDTO?> GetByPhoneNumberAsync(string phoneNumber);
        Task<List<StudentResponseDTO>> GetByStatusAsync(StudentStatus status);
        Task<List<StudentResponseDTO>> GetActiveStudentsAsync(PaginationParameter paginationParamter);
        Task<List<StudentResponseDTO>> GetInactiveStudentsAsync(PaginationParameter paginationParamter);
        Task<List<StudentResponseDTO>> GetSuspendedStudentsAsync(PaginationParameter paginationParamter);
        Task UpdateAsync(int id, UpdateStudentDTO updateStudentDTO);
        Task DeleteAsync(int id);
        Task SoftDeleteAsync(int id);
        Task RestoreAsync(int id);
        Task ActivateAsync(int id);
        Task DeactivateAsync(int id);
        Task SuspendAsync(int id);
        Task<int> GetTotalCountAsync();
        Task<Moshrefy.Application.DTOs.Common.DataTableResponse<StudentResponseDTO>> GetStudentsDataTableAsync(Moshrefy.Application.DTOs.Common.DataTableRequest request);
    }
}
