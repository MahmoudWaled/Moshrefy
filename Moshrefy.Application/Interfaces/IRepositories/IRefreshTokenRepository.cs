using Moshrefy.Domain.Entities;

namespace Moshrefy.Application.Interfaces.IRepositories
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task SaveChangesAsync();
        Task DeleteByUserIdAsync(string userId);
        Task DeleteByTokenAsync(string token);
    }
}
