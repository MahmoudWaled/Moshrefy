using Moshrefy.Application.DTOs.AcademicYear;
using Moshrefy.Application.DTOs.Common;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IServices
{
    public interface IAcademicYearService
    {

        Task AddAcademicYearAsync(CreateAcademicYearDTO createAcademicYearDTO);
        Task UpdateAcademicYearAsync(int id, UpdateAcademicYearDTO updateAcademicYearDTO);
        Task DeleteAcademicYearAsync(int id);
        Task<List<AcademicYearResponseDTO>> GetAllAcademicYearsAsync(PaginationParameter paginationParamter);
        Task<AcademicYearResponseDTO> GetAcademicYearByIdAsync(int id);
        Task<List<AcademicYearResponseDTO>> GetAcademicYearsByNameAsync(string name);
        Task<int> GetTotalCountAsync();
        Task<PaginatedResult<AcademicYearResponseDTO>> GetAcademicYearsPagedAsync(PaginationParameter paginationParameter);
    }
}
