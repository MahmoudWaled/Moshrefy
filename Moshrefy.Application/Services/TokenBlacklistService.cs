using Microsoft.Extensions.Caching.Memory;

namespace Moshrefy.Application.Services
{
    public class TokenBlacklistService(IMemoryCache _cache) : ITokenBlacklistService
    {
        private const string BlacklistKeyPrefix = "blacklist_token_";

        public Task BlacklistTokenAsync(string token, TimeSpan expiration)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            var key = GetBlacklistKey(token);

            // Store token in cache with expiration time
            _cache.Set(key, true, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration > TimeSpan.Zero ? expiration : TimeSpan.FromMinutes(10)
            });

            return Task.CompletedTask;
        }

        public Task<bool> IsTokenBlacklistedAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return Task.FromResult(false);

            var key = GetBlacklistKey(token);
            var isBlacklisted = _cache.TryGetValue(key, out _);

            return Task.FromResult(isBlacklisted);
        }

        private static string GetBlacklistKey(string token)
        {
            // Use hash to avoid storing full token in cache key
            var hash = Convert.ToBase64String(
                System.Security.Cryptography.SHA256.HashData(
                    System.Text.Encoding.UTF8.GetBytes(token)
                )
            );
            return $"{BlacklistKeyPrefix}{hash}";
        }
    }
}