using Microsoft.EntityFrameworkCore;
using Moshrefy.Application.Interfaces.IRepositories;
using Moshrefy.Domain.Entities;
using Moshrefy.infrastructure.Data;

namespace Moshrefy.infrastructure.Repositories
{
    public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
    {
        public async Task AddAsync(RefreshToken refreshToken)
        {
            await context.RefreshTokens.AddAsync(refreshToken);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token && !r.IsRevoked);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task DeleteByUserIdAsync(string userId)
        {
            var tokens = await context.RefreshTokens
                .Where(r => r.UserId == userId)
                .ToListAsync();

            context.RefreshTokens.RemoveRange(tokens);
        }

        public async Task DeleteByTokenAsync(string token)
        {
            var refreshToken = await context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token);
            if (refreshToken != null)
            {
                context.RefreshTokens.Remove(refreshToken);
            }
        }
    }
}
