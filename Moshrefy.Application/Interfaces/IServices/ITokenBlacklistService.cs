public interface ITokenBlacklistService
{
    Task BlacklistTokenAsync(string token, TimeSpan expiration);
    Task<bool> IsTokenBlacklistedAsync(string token);
}