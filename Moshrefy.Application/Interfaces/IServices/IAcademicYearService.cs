using Moshrefy.Application.DTOs.AcademicYear;
using Moshrefy.Domain.Paramter;

namespace Moshrefy.Application.Interfaces.IServices
{
    public interface IAcademicYearService
    {

        Task AddAcademicYearAsync(CreateAcademicYearDTO createAcademicYearDTO);
        Task UpdateAcademicYearAsync(int id, UpdateAcademicYearDTO updateAcademicYearDTO);
        Task DeleteAcademicYearAsync(int id);
        Task<List<AcademicYearResponseDTO>> GetAllAcademicYearsAsync(PaginationParamter paginationParamter);
        Task<AcademicYearResponseDTO> GetAcademicYearByIdAsync(int id);
        Task<List<AcademicYearResponseDTO>> GetAcademicYearsByNameAsync(string name);
        Task<int> GetTotalCountAsync();
        Task<Moshrefy.Application.DTOs.Common.DataTableResponse<AcademicYearResponseDTO>> GetAcademicYearsDataTableAsync(Moshrefy.Application.DTOs.Common.DataTableRequest request);
    }
}
